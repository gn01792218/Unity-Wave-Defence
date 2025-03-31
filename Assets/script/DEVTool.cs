using UnityEngine;

public class DEVTool : MonoBehaviour
{
    // 單例實例
    private static DEVTool _instance;
    
    // 公開的靜態屬性，用於訪問實例
    public static DEVTool Instance
    {
        get => GetInstance();
    }
    
    // 獲取單例實例的方法
    private static DEVTool GetInstance()
    {
        if (_instance == null)
        {
            // 使用 FindAnyObjectByType 獲得最佳性能
            _instance = FindAnyObjectByType<DEVTool>();
            
            if (_instance == null)
            {
                GameObject devObject = new GameObject("DEV_Manager");
                _instance = devObject.AddComponent<DEVTool>();
                DontDestroyOnLoad(devObject);
            }
        }
        return _instance;
    }
    
    // 開發模式狀態
    [SerializeField] private bool _isDevelopmentMode = false;
    
    // 公開屬性，使用方法來實現邏輯
    public bool IsDevelopmentMode
    {
        get => GetDevelopmentMode();
        set => SetDevelopmentMode(value);
    }
    
    // 事件：當開發模式狀態改變時觸發
    public delegate void DevelopmentModeChangedHandler(bool isEnabled);
    public event DevelopmentModeChangedHandler OnDevelopmentModeChanged;
    
    // 獲取開發模式狀態的方法
    private bool GetDevelopmentMode()
    {
        return _isDevelopmentMode;
    }
    
    // 設置開發模式狀態的方法
    private void SetDevelopmentMode(bool value)
    {
        if (_isDevelopmentMode != value)
        {
            _isDevelopmentMode = value;
            OnDevelopmentModeChanged?.Invoke(_isDevelopmentMode);
            Debug.Log($"開發模式已{(_isDevelopmentMode ? "開啟" : "關閉")}");
        }
    }
    
    // 提供一個切換方法
    public void ToggleDevelopmentMode()
    {
        IsDevelopmentMode = !IsDevelopmentMode;
    }
    
    // 在Unity Inspector中添加一個按鈕來切換開發模式
    private void OnValidate()
    {
        if (Application.isPlaying && _instance == this)
        {
            NotifyDevelopmentModeChanged();
        }
    }
    
    // 通知開發模式變更的方法
    private void NotifyDevelopmentModeChanged()
    {
        OnDevelopmentModeChanged?.Invoke(_isDevelopmentMode);
    }
    
    // 在遊戲運行時，可以添加按鍵來切換開發模式
    private void Update()
    {
        CheckForToggleInput();
    }
    
    // 檢查切換輸入的方法
    private void CheckForToggleInput()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            ToggleDevelopmentMode();
        }
    }
}