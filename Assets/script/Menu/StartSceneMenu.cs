using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI; // 用於按鈕

public class StartSceneMenu : MonoBehaviour
{
    // 按鈕的引用
    public Button startButton;
    public Button exitButton;

    void Start()
    {
        // 確保按鈕事件已經綁定
        startButton.onClick.AddListener(OnStartClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    // 開始遊戲，載入 OderScene
    void OnStartClicked()
    {
        // 加載 Scene1
        GlobalSceneManager.Instance.LoadScene(SceneEnum.OrderScene);
    }

    // 退出遊戲
    void OnExitButtonClicked()
    {
        // 在編輯器中無法退出，會顯示警告
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 停止遊戲模式
#else
        Application.Quit(); // 退出遊戲
#endif
    }
}