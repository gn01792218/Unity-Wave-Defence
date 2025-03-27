using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    // List 來存儲所有購買的單位
    private List<Unit> playerUnits = new List<Unit>();
    private List<Unit> enemyUnits = new List<Unit>();
    private List<UnitType> playerUnitTypes = new List<UnitType>();
    private List<UnitType> enemyUnitTypes = new List<UnitType>();

    // **單位生成的參數**
    public Transform spawnPoint; // 單位生成的起始位置
    public float spacing = 2.0f; // 生成單位的間距

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 確保場景切換時不銷毀
        }
    }
    void Start()
    {
        InitSpawnPosition();
    }

    // ✅ 直接存入 Unit 實例
    public void PurchaseUnit(UnitType unitType)
    {
        playerUnitTypes.Add(unitType);
        OrderMenu.Instance.UpdateUnitDisplay(); // 更新 UI
    }
    public void AddEnemyUnit(UnitType unitType)
    {
        enemyUnitTypes.Add(unitType);
    }

    // ✅ 取得已購買單位列表
    public List<Unit> GetPlayerUnits()
    {
        return playerUnits;
    }
    public List<Unit> GetEnemyUnits()
    {
        return enemyUnits;
    }
    public List<UnitType> GetPlayerUnitTypes()
    {
        return playerUnitTypes;
    }
    public List<UnitType> GetEnemyUnitTypes()
    {
        return enemyUnitTypes;
    }

    // **✅ 生成已購買的單位**
    public void SpawnPurchasedUnits()
    {
        foreach (UnitType type in playerUnitTypes)
        {
            SpawnUnit(TeamManager.Instance.playerTeam, type);
        }

    }
    public void SpawnEnemyUnits()
    {
        foreach (UnitType type in enemyUnitTypes)
        {
            SpawnUnit(TeamManager.Instance.enemyTeam, type);
        }
    }

    private void SpawnUnit(Team team, UnitType unitType)
    {
        // 生成單位物件
        GameObject unitObject;
        switch (unitType)
        {
            case UnitType.Tank1:
                unitObject = Instantiate(Resources.Load<GameObject>("prefabs/unit/Tank"), spawnPoint.position, Quaternion.identity);
                break;
            case UnitType.RocketCar:
                unitObject = Instantiate(Resources.Load<GameObject>("prefabs/unit/RocketCar"), spawnPoint.position, Quaternion.identity);
                break;
            case UnitType.Mech:
                unitObject = Instantiate(Resources.Load<GameObject>("prefabs/unit/Mech"), spawnPoint.position, Quaternion.identity);
                break;
            default:
                unitObject = null;
                break;
        }

        // 確保生成的單位是 Unit 類型，並設置其隊伍為 Player
        if (unitObject != null)
        {
            Unit unitInstance = unitObject.GetComponent<Unit>();
            if (unitInstance != null)
            {
                unitInstance.SetTeam(team);
            }
            spawnPoint.position += new Vector3(spacing, 0, 0); // 依次排列單位
            AddUnitToList(team, unitInstance);
        }
    }

    private void AddUnitToList(Team team, Unit unit)
    {
        switch (team.teamType)
        {
            case Team.TeamType.Player:
                playerUnits.Add(unit);
                break;
            case Team.TeamType.Enemy:
                enemyUnits.Add(unit);
                break;
        }
    }

    // **抽象出計算生成點位置的邏輯**
    private Vector3 InitSpawnPosition()
    {
        if (spawnPoint == null)
        {
            spawnPoint = new GameObject("SpawnPoint").transform;
            spawnPoint.SetParent(this.transform);  // 確保 spawnPoint 的父物體不會隨著 UnitManager 被銷毀
        }
        // 計算螢幕的世界寬度
        float screenWidthInWorld = Camera.main.orthographicSize * 2 * Screen.width / Screen.height;

        // 設定 spawnPoint 的位置為場景的中間偏右
        spawnPoint.position = new Vector3(screenWidthInWorld * 0.25f, spawnPoint.position.y, spawnPoint.position.z);

        // 返回計算出來的生成點位置
        return spawnPoint.position;
    }
}
