using UnityEngine;
using System.Collections;

public class NormalBullet : BulletBase
{
    public override float Damage { get; set; } = 2f; // 初始傷害
    public override float Speed { get; set; } = 5f;   // 初始速度

    protected override BulletType GetBulletType()
    {
        return BulletType.NormalBullet;
    }
}