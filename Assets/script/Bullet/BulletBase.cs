using UnityEngine;
using System.Collections;

public abstract class BulletBase : MonoBehaviour
{
    //強制子類別定義的屬性
    public abstract float Damage { get; set; }
    public abstract float Speed { get; set; }

    // 新增生命週期時間屬性
    public float lifeTime = 5f; // 子彈存在的最大時間，預設5秒

    protected Vector3 targetPosition;
    private Coroutine moveCoroutine;
    private Coroutine lifetimeCoroutine;
    private bool hasHit = false; // 避免多次觸發
    private GameObject Shooter;

    public void Launch(Vector3 targetPos, Vector3 unitPosition,GameObject shooter)
    {
        // 重置狀態
        hasHit = false;

        // 確保物件被激活
        gameObject.SetActive(true);
        targetPosition = targetPos;
        transform.position = unitPosition;
        Shooter = shooter;

        // 停止任何可能正在運行的協程
        StopAllCoroutines();

        // 啟動移動和生命週期協程
        moveCoroutine = StartCoroutine(MoveToTarget());
        lifetimeCoroutine = StartCoroutine(LifetimeCountdown());
    }

    private IEnumerator MoveToTarget()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f && !hasHit)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Speed * Time.deltaTime);
            yield return null;
        }

        // 如果到達目標位置且尚未觸發碰撞，則自動銷毀
        if (!hasHit)
        {
            ReturnToPool();
        }
    }

    private IEnumerator LifetimeCountdown()
    {
        // 等待設定的生命週期時間
        yield return new WaitForSeconds(lifeTime);

        // 如果子彈還存在且尚未觸發碰撞，則回收
        if (gameObject.activeInHierarchy && !hasHit)
        {
            ReturnToPool();
        }
    }

    // 使用OnTriggerEnter來處理碰撞
    private void OnTriggerEnter(Collider other)
    {
        // 避免子弹撞到自己或发射者
        if (hasHit || (Shooter != null && other.gameObject == Shooter)) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Unit"))
        {
            Unit enemy = other.GetComponent<Unit>();
            if (enemy != null)
            {
                hasHit = true;
                // Debug.Log($"Hit enemy: {enemy.name}, Dealing {Damage} damage.");
                enemy.TakeDamage(Damage);

                // 處理爆炸效果
                HandleExplosion(other.transform.position);

                // 返回物件池
                ReturnToPool();
            }
        }
    }

    // 處理爆炸效果和範圍傷害（如果需要）
    protected virtual void HandleExplosion(Vector3 hitPosition)
    {
        // 子類可以覆寫此方法來實現範圍傷害或特殊效果
        // 預設情況下不做任何事
    }

    private void ReturnToPool()
    {
        // 標記已觸發碰撞
        hasHit = true;

        // 停止所有正在運行的協程
        StopAllCoroutines();

        // 將子彈回收到物件池中
        BulletFactory.Instance.ReturnBullet(GetBulletType(), gameObject);
    }

    // 子類需要實現這個方法來返回正確的子彈類型
    protected abstract BulletType GetBulletType();

    // 當物件被禁用時確保停止所有協程
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}