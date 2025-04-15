using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    // List 來存儲所有購買的單位
    private List<Unit> playerUnits = new List<Unit>();
    private List<Unit> enemyUnits = new List<Unit>();
    private List<UnitType> playerUnitTypes = new List<UnitType>();
    private List<UnitType> enemyUnitTypes = new List<UnitType>();

    // **單位生成的參數**
    public float spacing = 2.0f; // 生成單位的間距

    void Start()
    {
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
    public void SpawnPurchasedUnits(Vector3 generatePosition)
    {
        foreach (UnitType type in playerUnitTypes)
        {
            SpawnUnit(TeamManager.Instance.playerTeam, type, generatePosition);
        }

    }
    public void SpawnEnemyUnits(Vector3 generatePosition)
    {
        foreach (UnitType type in enemyUnitTypes)
        {
            SpawnUnit(TeamManager.Instance.enemyTeam, type, generatePosition);
        }
    }

    private void SpawnUnit(Team team, UnitType unitType, Vector3 generatePosition)
    {
        // 生成單位物件
        GameObject unitObject;
        switch (unitType)
        {
            case UnitType.Tank1:
                unitObject = Instantiate(Resources.Load<GameObject>("prefabs/unit/Tank"), generatePosition, Quaternion.identity);
                break;
            case UnitType.RocketCar:
                unitObject = Instantiate(Resources.Load<GameObject>("prefabs/unit/RocketCar"), generatePosition, Quaternion.identity);
                break;
            case UnitType.Mech:
                unitObject = Instantiate(Resources.Load<GameObject>("prefabs/unit/Mech"), generatePosition, Quaternion.identity);
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
}
