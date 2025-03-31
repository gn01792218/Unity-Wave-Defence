using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class Unit : MonoBehaviour
{
    [Header("Unit Setup")]
    public Team team;  // 單位所屬的隊伍

    [SerializeField]
    private string layerName = "Unit";

    [Header("UI Elements")]
    private GameObject uiCanvasInstance;
    private Slider healthBar;
    private TMP_Text healthText;
    private Slider attackCooldownBar;

    [Header("Unit Stats")]
    public float Health { get; set; } // 默認會在 Awake 中初始化
    public abstract float MaxHealth { get; set; }
    public abstract float MoveSpeed { get; set; }
    public abstract float AttackRange { get; set; }
    public abstract float AttackCooldown { get; set; }
    public abstract float Damage { get; set; }
    public abstract float DetectionRange { get; set; }
    public abstract BulletType BulletType { get; set; }


    private NavMeshAgent agent;
    private float lastAttackTime;
    private Vector3 attackTargetPosition;


    private bool isPlayerControl = false; // 標誌是否正在被玩家控制

    private Vector3 playerTargetPosition; // 玩家指定的右鍵目標位置

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent 沒有附加到此物件。");
            return;
        }
        agent.speed = MoveSpeed;

        //設置layer
        gameObject.layer = LayerMask.NameToLayer(layerName);

        // 確保物件一開始就放置在地板上
        Vector3 startPosition = transform.position;

        // 嘗試在附近找到有效的 NavMesh 位置
        if (NavMesh.SamplePosition(startPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position; // 設置物件的位置為有效的 NavMesh 點
        }
        else
        {
            Debug.LogError("物件未能放置在有效的 NavMesh 上，請檢查地板的 NavMesh 烘焙設置。");
        }
        // 初始化 Health，設為 MaxHealth
        Health = MaxHealth;

        // 檢查是否與其他單位重疊，如果重疊則將位置微調
        CheckForOverlappingUnits();
    }
    void Start()
    {
        BulletFactory.Instance.RegisterBullet(BulletType);
        InitializeUI();
    }
    void Update()
    {
        UpdateUI();
        HandleWeaponSwitch(); // 檢查武器切換
        // 當到達右鍵目標位置後
        if (Vector3.Distance(transform.position, playerTargetPosition) <= agent.stoppingDistance)
        {
            SetPlayControl(false);
        }
        // 如果不是正在移動到右鍵目標，則進行自動尋敵邏輯
        if (!isPlayerControl)
        {
            SearchForEnemies();
        }
    }
    private void InitializeUI()
    {
        // 使用 UIHelper 创建 Canvas
        uiCanvasInstance = UIHelper.CreateCanvas("UnitUI", transform, new Vector3(0, 2, 0)); // 设置 Canvas 在 Unit 顶部

        // 创建血条
        healthBar = UIHelper.CreateSlider("HealthBar", new Vector3(0, 0, 0), uiCanvasInstance.transform, Color.green);
        healthBar.value = 1; // 初始血量为满

        // 创建攻击冷却条
        attackCooldownBar = UIHelper.CreateSlider("AttackCooldownBar", new Vector3(0, 10, 0), uiCanvasInstance.transform, Color.blue);
        attackCooldownBar.value = 0; // 初始冷却为 0
    }
    private void UpdateUI()
    {
        if (healthBar != null)
        {
            // 更新血条值，设置为当前血量占最大血量的比例
            healthBar.value = Health / MaxHealth;
            // 更新血条文本显示
            if (DEVTool.Instance.IsDevelopmentMode)
            {
                if (healthText == null) healthText = UIHelper.CreateText(healthBar.transform, "HealthText", new Vector3(0, 12, 0));
                else healthText.text = $"{Health}/{MaxHealth}";
            }
        }

        if (attackCooldownBar != null)
        {
            attackCooldownBar.value = Mathf.Clamp01((Time.time - lastAttackTime) / AttackCooldown);
        }

        // // 让 UI 朝向摄像机
        UIHelper.CanvasLookAtCamera(uiCanvasInstance.transform);
    }
    // ✅ 允許玩家或 AI 讓單位移動到指定位置
    public void MoveTo(Vector3 destination)
    {
        if (agent != null)
        {
            agent.SetDestination(destination);
            // Debug.Log($"單位移動到{destination}");
        }
    }
    public void MoveToPlayerSpceficPosition(Vector3 destination)
    {
        // Debug.Log("移動到玩家指定位置");
        SetPlayControl(true);
        SetPlayerTargetPosition(destination);
        MoveTo(destination);
    }

    public void StopMove()
    {
        if (agent != null)
        {
            agent.ResetPath(); // 取消當前的路徑
        }
    }

    public void SetPlayControl(Boolean isControl)
    {
        isPlayerControl = isControl;
    }
    public void SetPlayerTargetPosition(Vector3 position)
    {
        playerTargetPosition = position;
    }
    public void SetTeam(Team team)
    {
        this.team = team;
    }
    public void SetAttackTargetPosition(Vector3 position)
    {
        attackTargetPosition = position;
    }
    // ✅ 受傷機制
    public void TakeDamage(float amount)
    {
        Health -= amount;
        // Debug.Log(gameObject.name + " 受到傷害：" + amount + "，剩餘血量：" + Health);
        if (Health <= 0)
        {
            Die();
        }
    }
    private void AttackTarget(Vector3 position)
    {
        lastAttackTime = Time.time;
        attackTargetPosition = position;
        SpawnBullet();
    }

    private void SpawnBullet()
    {
        // 從工廠獲取子彈
        BulletBase bullet = BulletFactory.Instance.GetBullet(BulletType);

        // 設定子彈初始位置與方向
        bullet.Launch(attackTargetPosition, transform.position, gameObject);
    }

    private void Die()
    {
        if (UnitSelection.Instance != null)
        {
            UnitSelection.Instance.RemoveUnitFromSelection(this);
        }
        Destroy(gameObject);
    }

    // ✅ 自動尋找敵人並移動
    private void SearchForEnemies()
    {
        // 如果正在被玩家控制，則不進行自動尋敵
        if (isPlayerControl)
        {
            return; // 玩家控制下的單位不進行自動尋敵
        }

        // 假設敵人是標記為不同隊伍的單位
        Collider[] enemies = Physics.OverlapSphere(transform.position, DetectionRange);
        Unit closestEnemy = null;
        float closestDistance = float.MaxValue;

        // 先找出最近的敵人
        foreach (var enemy in enemies)
        {
            Unit enemyUnit = enemy.GetComponent<Unit>();
            if (enemyUnit == null || enemyUnit.team == this.team) continue;

            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestEnemy = enemyUnit;
                closestDistance = distanceToEnemy;
            }
        }

        // 對最近的敵人執行動作
        if (closestEnemy != null)
        {
            // 只有當敵人超出攻擊範圍時才追蹤
            if (closestDistance > AttackRange)
            {
                // 計算移動目標位置
                Vector3 directionToEnemy = (closestEnemy.transform.position - transform.position).normalized;
                Vector3 targetPosition = closestEnemy.transform.position - directionToEnemy * AttackRange;
                MoveTo(targetPosition); // 移動到攻擊範圍內
            }

            // 當距離小於等於攻擊範圍，且攻擊冷卻結束，則攻擊敵人
            if (closestDistance <= AttackRange && Time.time - lastAttackTime >= AttackCooldown)
            {
                AttackTarget(closestEnemy.transform.position);
            }
        }
    }
    private void CheckForOverlappingUnits()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1.0f, LayerMask.GetMask("Unit"));
        foreach (Collider col in colliders)
        {
            Unit otherUnit = col.GetComponent<Unit>();
            if (otherUnit != null && otherUnit != this && IsOverlapping(otherUnit))
            {
                // 如果重疊，將位置向外移動
                // 稍微移動物體來確保碰撞檢測
                Vector3 offset = new Vector3(0.01f, 0.01f, 0.01f); // 小的偏移量
                transform.position += offset; // 移動物體
            }
        }
    }
    bool IsOverlapping(Unit otherUnit)
    {
        // 檢查當前物體和其他 Unit 的碰撞器是否重疊
        Collider otherCollider = otherUnit.GetComponent<Collider>();
        if (otherCollider != null)
        {
            return GetComponent<Collider>().bounds.Intersects(otherCollider.bounds);
        }
        return false;
    }
    // 🚀 允許玩家使用鍵盤切換武器
    private void HandleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) //數字鍵1
        {
            BulletType = BulletType.NormalBullet;
            Debug.Log("切換武器：普通子彈");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) //數字鍵2
        {
            BulletType = BulletType.RocketBullet;
            Debug.Log("切換武器：火箭彈");
        }
    }
}