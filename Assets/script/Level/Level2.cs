using UnityEngine;
public class Level2 : BaseLevel
{
    public GameObject normalBulletPrefab;

    // // 覆寫註冊子彈的方法
    // public override void RegisterBullets()
    // {
    //     BulletFactory.Instance.RegisterBullet(BulletType.NormalBullet, normalBulletPrefab);
    // }

    public override void StartLevel()
    {
        base.StartLevel();
        Debug.Log("Level 1 has started");
    }
}