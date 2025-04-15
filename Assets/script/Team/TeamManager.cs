using UnityEngine;

public class TeamManager : Singleton<TeamManager>
{
     public Team playerTeam {get;set;}
     public Team enemyTeam {get;set;}

    public TeamManager(){
    }
    public void InitializeTeams()
    {
        if(playerTeam == null) this.playerTeam =  new Team(Team.TeamType.Player, "Player");
        if(enemyTeam == null) this.enemyTeam = new Team(Team.TeamType.Enemy, "Enemy");
    }
}
