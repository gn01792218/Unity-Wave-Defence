using UnityEngine;
using System.Collections;

public class NormalBullet : MonoBehaviour, IBullet
{
    private Vector3 targetPosition;
    private float damage;
    public float speed = 10f;

    public void Initialize(Vector3 targetPos, float dmg)
    {
        targetPosition = targetPos;
        damage = dmg;
    }

    public void Launch()
    {
        // 實現子彈的飛行邏輯
        StartCoroutine(MoveToTarget());
    }

    private IEnumerator MoveToTarget()
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        OnHit();
    }

    public void OnHit()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.1f);
        foreach (var hit in hitColliders)
        {
            Unit enemy = hit.GetComponent<Unit>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        // 將子彈回收到物件池中
        BulletFactory.Instance.ReturnBullet(BulletType.NormalBullet, gameObject);
    }
}