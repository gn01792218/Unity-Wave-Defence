using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GlobalSceneManager : Singleton<GlobalSceneManager>
{
    // 在場景加載完成後執行的事件
    // 讓外部可以訂閱的方法
    public event Action OnSceneLoaded;

    public void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoadedHandler;
    }

    public void NextScene()
    {
        // 取得 Enum 所有的值
        SceneEnum[] values = (SceneEnum[])Enum.GetValues(typeof(SceneEnum));
        int currentIndex = GetCurrentSceneIndex();

        // 確保不超出範圍
        if (currentIndex < values.Length - 1)
        {
            SceneEnum nextScene = values[currentIndex + 1];
            Debug.Log($"場景列表{values.Length},當前場景{GetCurrentSceneIndex()}、index{currentIndex},將切換到{currentIndex + 1},{nextScene}");
            LoadScene(nextScene);
        }
    }

    // 加載新的場景
    public void LoadScene(SceneEnum scene)
    {
        // 如果場景名相同，就不做任何事情
        if (GetCurrentSceneIndex() == (int)scene)
        {
            Debug.Log("Already in the target scene.");
            return;
        }

        // 加載場景
        SceneManager.LoadScene((int)scene);
        Debug.Log($"切換場景到{scene}");
    }

    // 處理場景加載完成的邏輯
    private void OnSceneLoadedHandler(Scene scene, LoadSceneMode mode)
    {
        // 通知場景已經加載完成
        OnSceneLoaded?.Invoke();
    }

    public int GetCurrentSceneIndex(){
        return SceneManager.GetActiveScene().buildIndex;
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