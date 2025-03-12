using UnityEngine;
using System.Collections;
public class RocketBullet : BulletBase
{
    public override float Damage { get; set; } = 20f; // 初始傷害
    public override float Speed { get; set; } = 3f;   // 初始速度

    public float explosionRadius = 5f;
    
    protected override BulletType GetBulletType()
    {
        return BulletType.RocketBullet;
    }
    protected override void HandleExplosion(Vector3 hitPosition)
    {
        // 爆炸效果
        // Instantiate(explosionEffect, hitPosition, Quaternion.identity);
        
        // 範圍傷害
        Collider[] hitColliders = Physics.OverlapSphere(hitPosition, explosionRadius);
        foreach (var hit in hitColliders)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Unit"))
            {
                Unit unit = hit.GetComponent<Unit>();
                if (unit != null)
                {
                    // 根據距離計算傷害衰減
                    float distance = Vector3.Distance(hitPosition, hit.transform.position);
                    float damagePercent = 1.0f - (distance / explosionRadius);
                    float actualDamage = Damage * Mathf.Max(0.1f, damagePercent);
                    unit.TakeDamage(actualDamage);
                }
            }
        }
    }

}
