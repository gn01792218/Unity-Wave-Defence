using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [Header("Unit Setup")]
    public Team team;  // 單位所屬的隊伍

    [SerializeField]
    private string layerName = "Unit";

    [Header("Unit Stats")]
    public BulletType bulletType = BulletType.NormalBullet; // 默認是普通子彈

    public float health = 100f;
    public float moveSpeed = 3.5f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public float damage = 10f;

    private NavMeshAgent agent;
    private float lastAttackTime;
    private Vector3 attackTarget;

    public float detectionRange = 10f;  // 偵測敵人範圍

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
        agent.speed = moveSpeed;

        // 確保物件一開始就放置在地板上
        Vector3 startPosition = transform.position;
        //設置layer
        gameObject.layer = LayerMask.NameToLayer(layerName);

        // 嘗試在附近找到有效的 NavMesh 位置
        if (NavMesh.SamplePosition(startPosition, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position; // 設置物件的位置為有效的 NavMesh 點
        }
        else
        {
            Debug.LogError("物件未能放置在有效的 NavMesh 上，請檢查地板的 NavMesh 烘焙設置。");
        }
    }

    void Update()
    {
        HandleWeaponSwitch(); // 檢查武器切換
        // 當到達右鍵目標位置後
        if (Vector3.Distance(transform.position, playerTargetPosition) <= agent.stoppingDistance)
        {
            SetPlayControl(false);
            SearchForEnemies(); // 啟動自動尋敵
        }
        // 如果不是正在移動到右鍵目標，則進行自動尋敵邏輯
        if (!isPlayerControl)
        {
            SearchForEnemies();
        }

        // 攻擊邏輯
        if (Vector3.Distance(transform.position, attackTarget) <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            AttackTarget();
        }
    }

    // ✅ 允許玩家或 AI 讓單位移動到指定位置
    public void MoveTo(Vector3 destination)
    {
        if (agent != null)
        {
            agent.SetDestination(destination);
            Debug.Log($"單位移動到{destination}");
        }
    }
    public void MoveToPlayerSpceficPosition(Vector3 destination)
    {
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

    // ✅ 設定攻擊座標，讓子彈朝該位置發射
    public void SetAttackTarget(Vector3 target)
    {
        attackTarget = target;
    }
    public void SetPlayControl(Boolean isControl)
    {
        isPlayerControl = isControl;
    }
    public void SetPlayerTargetPosition(Vector3 position)
    {
        playerTargetPosition = position;
    }

    private void AttackTarget()
    {
        lastAttackTime = Time.time;
        SpawnBullet();
    }

    private void SpawnBullet()
    {
        if (BulletFactory.Instance == null)
        {
            Debug.LogError("BulletFactory 未初始化！");
            return;
        }

        // 從工廠獲取子彈
        GameObject bullet = BulletFactory.Instance.GetBullet(bulletType);

        if (bullet == null)
        {
            Debug.LogError("無法生成子彈：" + bulletType);
            return;
        }

        // 設定子彈初始位置與方向
        bullet.transform.position = transform.position + Vector3.up * 1.5f; // 避免子彈直接穿過地面
        bullet.transform.LookAt(attackTarget); // 讓子彈朝向目標

        // 讓子彈開始移動
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (attackTarget - bullet.transform.position).normalized;
            rb.linearVelocity = direction * 10f; // 設定子彈速度
        }
    }

    // ✅ 受傷機制
    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log(gameObject.name + " 受到傷害：" + damage + "，剩餘血量：" + health);
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    // 🚀 允許玩家使用鍵盤切換武器
    private void HandleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) //數字鍵1
        {
            bulletType = BulletType.NormalBullet;
            Debug.Log("切換武器：普通子彈");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) //數字鍵2
        {
            bulletType = BulletType.RocketBullet;
            Debug.Log("切換武器：火箭彈");
        }
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
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var enemy in enemies)
        {
            Unit enemyUnit = enemy.GetComponent<Unit>();
            if (enemyUnit != null && enemyUnit.team != this.team)  // 檢查是不是敵方
            {
                // 更新目標並開始追蹤
                // 計算移動目標位置
                Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
                Vector3 targetPosition = enemy.transform.position - directionToEnemy * attackRange;
                MoveTo(targetPosition); // 移動到敵人位置
                SetAttackTarget(enemy.transform.position); // 設置攻擊目標
                break; // 找到敵人就停止尋找
            }
        }
    }
}