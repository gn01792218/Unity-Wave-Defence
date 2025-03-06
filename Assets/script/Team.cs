using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    public enum TeamType
    {
        Player,      // 玩家隊伍
        Enemy,       // 敵人隊伍
        Ally,        // 同盟隊伍
        Neutral      // 中立隊伍
    }

    [Header("Team Setup")]
    public string teamName;
    public TeamType teamType;

    [Header("Allied and Enemy Teams")]
    public List<Team> alliedTeams = new List<Team>();  // 同盟隊伍
    public List<Team> enemyTeams = new List<Team>();   // 敵對隊伍

    private void Awake()
    {
        // 確保所有隊伍都初始化
        if (alliedTeams == null) alliedTeams = new List<Team>();
        if (enemyTeams == null) enemyTeams = new List<Team>();
    }

    // 判斷兩隊是否為敵人
    public bool IsEnemy(Team otherTeam)
    {
        return enemyTeams.Contains(otherTeam);
    }

    // 判斷兩隊是否為同盟
    public bool IsAlly(Team otherTeam)
    {
        return alliedTeams.Contains(otherTeam);
    }
}
