using UnityEngine;

public class TeamManager : MonoBehaviour
{
     public static TeamManager Instance { get; private set; }
     public Team playerTeam {get;set;}
     public Team enemyTeam {get;set;}

    public TeamManager(){
    }

    private void Awake()
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
    public void InitializeTeams()
    {
        if(playerTeam == null) this.playerTeam =  new Team(Team.TeamType.Player, "Player");
        if(enemyTeam == null) this.enemyTeam = new Team(Team.TeamType.Enemy, "Enemy");
    }
}
