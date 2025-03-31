public class MechUnit : Unit
{
    public override float MaxHealth { get; set; } = 20f;
    public override float MoveSpeed { get; set; } = 10f;
    public override float AttackRange { get; set; } = 10f;
    public override float AttackCooldown { get; set; } = 3f;
    public override float Damage { get; set; } = 5f;
    public override float DetectionRange { get; set; } = 10f;
    public override BulletType BulletType { get; set; } = BulletType.NormalBullet;
}

