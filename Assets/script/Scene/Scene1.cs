using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class Scene1 : Singleton<Scene1>
{
    // 用來儲存當前場景的名稱或 ID
    private SceneEnum currentScene;

    // 在場景加載完成後執行一些邏輯
    public delegate void OnSceneLoadedDelegate();
    public event OnSceneLoadedDelegate OnSceneLoaded;


    void Start()
    {
        Init();
    }

    private void Init()
    {
        // 初始化隊伍
        TeamManager.Instance.InitializeTeams();
        BaseStation playerBase = GameObject.Find("PlayerBase").GetComponent<BaseStation>();
        BaseStation enemyStation = GameObject.Find("EnemyBase").GetComponent<BaseStation>();

        //產生玩家購買的軍隊
        UnitManager.Instance.SpawnPurchasedUnits(playerBase.GetCenterPosition());

        //產生敵方隊伍
        //之後要在UnitManager裡面創一個GenerateEnemyUnits的方法
        //參數 : 1.軍隊型態列表 2.軍隊出生地點
        UnitManager.Instance.AddEnemyUnit(UnitType.Mech);
        UnitManager.Instance.SpawnEnemyUnits(enemyStation.GetCenterPosition());

        // 開啟DEV模式
        DEVTool.Instance.ToggleDevelopmentMode();

        // 技能列表初始化
        TacticalSkillManager.Instance.GenerateSkillButtons();
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