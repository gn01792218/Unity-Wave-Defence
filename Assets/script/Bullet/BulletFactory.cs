using UnityEngine;
using System.Collections.Generic;

public class BulletFactory : MonoBehaviour

{
    public static BulletFactory Instance;
    // 這裡可以存儲每種類型的物件池
    private Dictionary<BulletType, Queue<GameObject>> bulletPools = new Dictionary<BulletType, Queue<GameObject>>();
    private Dictionary<BulletType, GameObject> bulletPrefabs = new Dictionary<BulletType, GameObject>();

    void Awake()
    {
        Instance = this;
    }
    public void RegisterBullet(BulletType bulletType)
    {
        if (bulletPrefabs.ContainsKey(bulletType)) return;
        // 根據 bulletType 動態選擇對應的預製體
        GameObject bulletPrefab = null;
        switch (bulletType)
        {
            case BulletType.NormalBullet:
                bulletPrefab = Resources.Load<GameObject>("prefabs/bullet/NormalBullet");
                break;
            case BulletType.RocketBullet:
                bulletPrefab = Resources.Load<GameObject>("prefabs/bullet/RocketBullet");
                break;
            case BulletType.TankGunBullet:
                bulletPrefab = Resources.Load<GameObject>("prefabs/bullet/TankGunBullet");
                break;
            // 可以繼續添加其他類型的子彈
            default:
                Debug.LogError($"Bullet type {bulletType} not supported.");
                return;
        }
        bulletPrefabs.Add(bulletType, bulletPrefab);
        bulletPools.Add(bulletType, new Queue<GameObject>());

    }

    public BulletBase GetBullet(BulletType bulletType)
    {
        if (!bulletPrefabs.ContainsKey(bulletType))
        {
            Debug.LogError($"Bullet type {bulletType} not registered.");
            return null;
        }

        GameObject bulletObj = null;

        // 从物件池中取出或者实例化新的
        if (bulletPools.ContainsKey(bulletType))
        {
            if (bulletPools[bulletType].Count > 0)
            {
                bulletObj = bulletPools[bulletType].Dequeue();
            }
            else
            {
                bulletObj = Instantiate(bulletPrefabs[bulletType]);
            }

            // 返回 BulletBase 类型的组件
            BulletBase bullet = bulletObj.GetComponent<BulletBase>();
            return bullet;
        }
        else
        {
            return null;
        }
    }

    public void ReturnBullet(BulletType bulletType, GameObject bullet)
    {
        if (bulletPools.ContainsKey(bulletType))
        {
            bullet.SetActive(false);
            bulletPools[bulletType].Enqueue(bullet);
        }
        else
        {
            Debug.LogError($"Bullet type {bulletType} not registered for return.");
        }
    }
}