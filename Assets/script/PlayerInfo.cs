using UnityEngine;
using System;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo Instance { get; private set; }

    public event Action OnValueChanged;

    [SerializeField] private float money = 1000; // 初始金錢
    [SerializeField] private float tacticalPoints = 10; // 初始戰術點數

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 遊戲切換場景時不銷毀
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float Money
    {
        get { return money; }
        private set
        {
            money = value;
            OnValueChanged?.Invoke(); // 變更時通知 UI
        }
    }

    public float TacticalPoints
    {
        get { return tacticalPoints; }
        private set
        {
            tacticalPoints = value;
            OnValueChanged?.Invoke();
        }
    }

    public void AddMoney(float amount)
    {
        Money += amount;
    }

    public void AddTacticalPoints(float amount)
    {
        TacticalPoints += amount;
    }
}
