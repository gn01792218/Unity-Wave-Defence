using UnityEngine;
public interface IBullet

{
    void Initialize(Vector3 targetPos, float damage);
    void Launch();
    void OnHit();
}

