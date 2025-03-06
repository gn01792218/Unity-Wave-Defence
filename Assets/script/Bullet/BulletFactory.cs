using UnityEngine;
using System.Collections.Generic;

public class BulletFactory : MonoBehaviour

{
    public static BulletFactory Instance;
    // 一開始就可以使用的子彈
    // 其他的可以透過程式來添加
    public GameObject normalBulletPrefab;
    public GameObject rocketBulletPrefab;

    // 這裡可以存儲每種類型的物件池
    private Dictionary<BulletType, Queue<GameObject>> bulletPools = new Dictionary<BulletType, Queue<GameObject>>();
    private Dictionary<BulletType, GameObject> bulletPrefabs = new Dictionary<BulletType, GameObject>();

    void Awake()
    {
        Instance = this;

        // // 註冊子彈預製體    
        bulletPrefabs.Add(BulletType.NormalBullet, normalBulletPrefab);
        bulletPrefabs.Add(BulletType.RocketBullet, rocketBulletPrefab);

        // 初始化物件池
        foreach (var prefab in bulletPrefabs)
        {
            bulletPools[prefab.Key] = new Queue<GameObject>();
        }
    }

    public GameObject GetBullet(BulletType bulletType)
    {
        if (bulletPools.ContainsKey(bulletType))
        {
            // 從物件池中取出子彈
            if (bulletPools[bulletType].Count > 0)
            {
                GameObject bullet = bulletPools[bulletType].Dequeue();
                bullet.SetActive(true);  // 激活該子彈
                return bullet;
            }
            else
            {
                // 如果池子裡沒有子彈，則實例化一個新的
                GameObject bullet = Instantiate(bulletPrefabs[bulletType]);
                return bullet;
            }
        }
        else
        {
            Debug.LogError($"Bullet type {bulletType} not registered!");
            return null;
        }
    }

    public void ReturnBullet(BulletType bulletType, GameObject bullet)
    {
        if (bulletPools.ContainsKey(bulletType))
        {
            bullet.SetActive(false);  // 停用該子彈
            bulletPools[bulletType].Enqueue(bullet);  // 放回物件池
        }
    }
}