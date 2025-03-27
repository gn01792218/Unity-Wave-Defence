using UnityEngine;

public class TankUnit : Unit
{
    
    public override float Cost { get; set;} = 150f;
    public override float Health { get; set; } = 50f;
    public override float MoveSpeed { get; set; } = 8f;
    public override float AttackRange { get; set; } = 15f;
    public override float AttackCooldown { get; set; } = 5f;
    public override float Damage { get; set; } = 30f;
    public override float DetectionRange { get; set; } = 20f;
    public override BulletType BulletType { get; set; } = BulletType.TankGunBullet;

    public override string PrefabPath => "prefabs/unit/Tank";
}

