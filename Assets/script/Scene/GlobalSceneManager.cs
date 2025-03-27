using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GlobalSceneManager : MonoBehaviour
{
    // 唯一的實例
    public static GlobalSceneManager Instance { get; private set; }

    // 用來儲存當前場景的名稱或 ID
    private SceneEnum currentScene;

    // 在場景加載完成後執行一些邏輯
    public delegate void OnSceneLoadedDelegate();
    public event OnSceneLoadedDelegate OnSceneLoaded;


    void Awake()
    {
        // 確保只會有一個 GlobalSceneManager 實例
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 確保在場景切換時這個物件不會被銷毀
        }
        else
        {
            Destroy(gameObject); // 若已經存在實例，則銷毀當前物件
        }
    }

   public void NextScene()
    {
        // 取得 Enum 所有的值
        SceneEnum[] values = (SceneEnum[])Enum.GetValues(typeof(SceneEnum));
        int currentIndex = Array.IndexOf(values, currentScene);
    
        // 確保不超出範圍
        if (currentIndex < values.Length - 1)
        {
            SceneEnum nextScene = values[currentIndex + 1];
            LoadScene(nextScene);
        }
    }

    // 加載新的場景
    public void LoadScene(SceneEnum scene)
    {
        // 如果場景名相同，就不做任何事情
        if (currentScene == scene)
        {
            Debug.Log("Already in the target scene.");
            return;
        }

        // 加載場景
        SceneManager.LoadScene((int)scene);
        currentScene = scene;

        // 監聽場景加載完成事件
        SceneManager.sceneLoaded += OnSceneLoadedHandler;
    }

    // 處理場景加載完成的邏輯
    private void OnSceneLoadedHandler(Scene scene, LoadSceneMode mode)
    {
        // // 確保這裡使用的場景名稱與 Enum 匹配
        // if (System.Enum.TryParse(scene.name, out SceneEnum sceneEnum))
        // {
        //     currentScene = sceneEnum;
        // }

        // 解除監聽，避免每次都觸發
        SceneManager.sceneLoaded -= OnSceneLoadedHandler;

        // 通知場景已經加載完成
        OnSceneLoaded?.Invoke();
    }

    // 退出遊戲（在編輯器中無法退出）
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}