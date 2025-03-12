using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public abstract class Unit : MonoBehaviour
{
    [Header("Unit Setup")]
    public Team team;  // 單位所屬的隊伍

    [SerializeField]
    private string layerName = "Unit";


    [Header("Unit Stats")]
    public abstract float Health { get; set; }
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

        // 檢查是否與其他單位重疊，如果重疊則將位置微調
        CheckForOverlappingUnits();
    }
    void Start()
    {
        BulletFactory.Instance.RegisterBullet(BulletType);
    }

    void Update()
    {
        HandleWeaponSwitch(); // 檢查武器切換
        // 當到達右鍵目標位置後
        if (Vector3.Distance(transform.position, playerTargetPosition) <= agent.stoppingDistance)
        {
            SetPlayControl(false);
            // SearchForEnemies(); // 啟動自動尋敵
        }
        // 如果不是正在移動到右鍵目標，則進行自動尋敵邏輯
        if (!isPlayerControl)
        {
            SearchForEnemies();
        }
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
        Debug.Log("移動到玩家指定位置");
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

    public void SetAttackTargetPosition(Vector3 position)
    {
        attackTargetPosition = position;
    }
    // ✅ 受傷機制
    public void TakeDamage(float amount)
    {
        Health -= amount;
        Debug.Log(gameObject.name + " 受到傷害：" + amount + "，剩餘血量：" + Health);
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
        foreach (var enemy in enemies)
        {
            Unit enemyUnit = enemy.GetComponent<Unit>();
            if (enemyUnit != null && enemyUnit.team != this.team)  // 檢查是不是敵方
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

                // 只有當敵人超出攻擊範圍時才追蹤
                if (distanceToEnemy > AttackRange)
                {
                    // 計算移動目標位置
                    Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
                    Vector3 targetPosition = enemy.transform.position - directionToEnemy * AttackRange;
                    MoveTo(targetPosition); // 移動到攻擊範圍內
                }

                // 當距離小於等於攻擊範圍，且攻擊冷卻結束，則攻擊敵人
                if (distanceToEnemy <= AttackRange && Time.time - lastAttackTime >= AttackCooldown)
                {
                    AttackTarget(enemy.transform.position);
                }

                break; // 找到敵人就停止尋找
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