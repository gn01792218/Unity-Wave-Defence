public class TankUnit : Unit
{
    public override float MaxHealth { get; set; } = 50f;
    public override float MoveSpeed { get; set; } = 5f;
    public override float AttackRange { get; set; } = 15f;
    public override float AttackCooldown { get; set; } = 5f;
    public override float Damage { get; set; } = 30f;
    public override float DetectionRange { get; set; } = 20f;
    public override BulletType BulletType { get; set; } = BulletType.TankGunBullet;
}

