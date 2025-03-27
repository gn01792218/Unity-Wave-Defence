using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // 用於按鈕

public class OrderMenu : MonoBehaviour
{
    // 單例模式
    public static OrderMenu Instance { get; private set; }

    public Button buyTank1Button;
    public Button buyRocketCarButton;
    public Button buyMechButton;
    public Button battleButton;
    public Button backToInitSceneButton;

    // 用來顯示單位的 UI 文本
    public TextMeshProUGUI unitsDisplayText;  // 這是 TextMeshPro 的文本組件，你可以在 Canvas 中添加一個 TextMeshPro 字段來顯示購買單位的情況

    void Awake()
    {
        // 確保單例只有一個實例
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        // 確保按鈕事件已經綁定
        buyTank1Button.onClick.AddListener(OnBuyTank1ButtonClicked);
        buyRocketCarButton.onClick.AddListener(OnBuyRocketButtonClicked);
        buyMechButton.onClick.AddListener(OnBuyMechButtonClicked);
        battleButton.onClick.AddListener(OnBattleButtonClicked);
        backToInitSceneButton.onClick.AddListener(OnBackToInitSceneButtonClicked);

        // 初始化顯示
        UpdateUnitDisplay();
    }

    void OnBattleButtonClicked()
    {
        // 進入下一關
        GlobalSceneManager.Instance.NextScene();
    }

    void OnBuyTank1ButtonClicked()
    {
        // var unit = new TankUnit();
        UnitManager.Instance.PurchaseUnit(UnitType.Tank1);
        // PlayerInfo.Instance.AddMoney(-unit.Cost);
    }

    void OnBuyRocketButtonClicked()
    {
        // var unit = new RocketCarUnit();
        UnitManager.Instance.PurchaseUnit(UnitType.RocketCar);
        // PlayerInfo.Instance.AddMoney(-unit.Cost);
    }

    void OnBuyMechButtonClicked()
    {
        // var unit = new MechUnit();
        UnitManager.Instance.PurchaseUnit(UnitType.Mech);
        // PlayerInfo.Instance.AddMoney(-unit.Cost);
    }

    void OnBackToInitSceneButtonClicked()
    {
        GlobalSceneManager.Instance.LoadScene(SceneEnum.StartScene);
    }

    // 更新UI顯示已購買的單位及其數量
    public void UpdateUnitDisplay()
    {
        List<UnitType> types = UnitManager.Instance.GetPlayerUnitTypes();

        // ✅ 改用 Dictionary 統計每種 Unit 的數量
        Dictionary<UnitType, int> unitCounts = new Dictionary<UnitType, int>();

        foreach (UnitType type in types)
        {
            if (unitCounts.ContainsKey(type))
            {
                unitCounts[type]++;
            }
            else
            {
                unitCounts[type] = 1;
            }
        }

        // ✅ 更新 UI 顯示
        string displayText = "Purchased Units:\n";
        foreach (var unit in unitCounts)
        {
            displayText += $"{unit.Key} x {unit.Value}\n";
        }

        unitsDisplayText.text = displayText;
    }
}