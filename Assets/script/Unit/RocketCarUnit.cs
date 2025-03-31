public class RocketCarUnit : Unit
{
    public override float MaxHealth { get; set; } = 15f;
    public override float MoveSpeed { get; set; } = 3f;
    public override float AttackRange { get; set; } = 25f;
    public override float AttackCooldown { get; set; } = 10f;
    public override float Damage { get; set; } = 10f;
    public override float DetectionRange { get; set; } = 30f;
    public override BulletType BulletType { get; set; } = BulletType.RocketBullet;
}

