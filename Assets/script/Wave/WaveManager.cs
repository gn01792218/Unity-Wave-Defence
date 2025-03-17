using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public BaseWave currentWave;

    // void Awake()
    // {
    //     // 確保只有一個 LevelManager 實例
    //     if (Instance == null)
    //     {
    //         Instance = this;
    //         DontDestroyOnLoad(gameObject); // 保證這個物件在場景切換時不會被銷毀
    //     }
    //     else
    //     {
    //         Destroy(gameObject); // 如果已經有實例，則銷毀當前的物件
    //     }
    // }

    public void SetWave(int waveId)
    {
        if (currentWave != null)
        {
            Destroy(currentWave.gameObject);
        }

        switch (waveId)
        {
            case 1:
                currentWave = new GameObject("Wave1").AddComponent<Wave1>();
                break;
            case 2:
                currentWave = new GameObject("Wave2").AddComponent<Wave2>();
                break;
            default:
                Debug.LogError("Level not found");
                return;
        }

        currentWave.StartWave();
    }
}