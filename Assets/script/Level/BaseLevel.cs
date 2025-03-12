using UnityEngine;
public abstract class BaseLevel : MonoBehaviour
{
    // 用來註冊需要的子彈預製體，所有具體關卡都需要覆寫這個方法
    // public abstract void RegisterBullets();
    
    // 你可以添加其它通用的關卡邏輯方法
    public virtual void StartLevel()
    {
        Debug.Log("Level has started");
        // RegisterBullets();
    }
}