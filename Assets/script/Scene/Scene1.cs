using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Scene1 : MonoBehaviour
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
        Init();
    }

    private void Init(){
        // 初始化隊伍
        TeamManager.Instance.InitializeTeams();

        //產生玩家購買的軍隊
        UnitManager.Instance.SpawnPurchasedUnits();

        //產生敵方隊伍
        UnitManager.Instance.AddEnemyUnit(UnitType.Mech);
        UnitManager.Instance.SpawnEnemyUnits();

        // 開啟DEV模式
        DEVTool.Instance.ToggleDevelopmentMode();

        // 技能列表初始化
        TacticalSkillManager.Instance.InitializeSkillBar();
        Debug.Log($"技能列表{TacticalSkillManager.Instance.GetPlayerTacticalSkills().Count}");
    }
    

    // 處理場景加載完成的邏輯
    private void OnSceneLoadedHandler(Scene scene, LoadSceneMode mode)
    {
        // 解除監聽，避免每次都觸發
        SceneManager.sceneLoaded -= OnSceneLoadedHandler;

        // 通知場景已經加載完成
        OnSceneLoaded?.Invoke();
    }
}