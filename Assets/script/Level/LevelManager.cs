using UnityEngine;
public class LevelManager : MonoBehaviour
{
    public BaseLevel currentLevel;
    
    void Start()
    {
        // 預設的開始關卡，可以在遊戲開始時設定
        SetLevel(1); 
    }

    // 外部可以呼叫這個方法來設定關卡
    public void SetLevel(int levelId)
    {
        // 停止當前關卡的邏輯
        if (currentLevel != null)
        {
            Destroy(currentLevel.gameObject); // 移除當前關卡
        }

        // 根據levelId動態加載關卡
        switch (levelId)
        {
            case 1:
                currentLevel = new GameObject("Level1").AddComponent<Level1>(); // 動態創建Level1
                break;
            case 2:
                currentLevel = new GameObject("Level2").AddComponent<Level2>(); // 動態創建Level2
                break;
            // 其他關卡...
            default:
                Debug.LogError("Level not found");
                return;
        }

        // 初始化並啟動新的關卡
        currentLevel.StartLevel();
    }
}