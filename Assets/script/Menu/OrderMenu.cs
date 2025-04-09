using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderMenu : Singleton<OrderMenu>
{
    public Button buyTank1Button;
    public Button buyRocketCarButton;
    public Button buyMechButton;
    public Button battleButton;
    public Button backToInitSceneButton;
    public Toggle attackBuffToggle;
    public Toggle attackSpeedBuffToggle;

    // 用來顯示單位的 UI 文本
    public TextMeshProUGUI unitsDisplayText;
    public TextMeshProUGUI skillDisplayText;

    void Start()
    {
        // 確保按鈕事件已經綁定
        buyTank1Button.onClick.AddListener(OnBuyTank1ButtonClicked);
        buyRocketCarButton.onClick.AddListener(OnBuyRocketButtonClicked);
        buyMechButton.onClick.AddListener(OnBuyMechButtonClicked);
        battleButton.onClick.AddListener(OnBattleButtonClicked);
        backToInitSceneButton.onClick.AddListener(OnBackToInitSceneButtonClicked);
        attackBuffToggle.onValueChanged.AddListener(onToggleAttackBuff);
        attackSpeedBuffToggle.onValueChanged.AddListener(onToggleAttackSpeedBuff);

        // 訂閱場景事件
        GlobalSceneManager.Instance.OnSceneLoaded += HandleSceneLoaded;

        // 初始化顯示
        UpdateUnitDisplay();
    }
    void Update()
    {
        // 實時檢查並更新 Toggle 的禁用狀態
        UpdateSkillToggles();
    }

    void OnDestroy()
    {

    }

    void OnBattleButtonClicked()
    {
        // 進入下一關
        GlobalSceneManager.Instance.NextScene();
    }

    void OnBuyTank1ButtonClicked()
    {
        UnitManager.Instance.PurchaseUnit(UnitType.Tank1);
        PlayerInfo.Instance.AddMoney(-UnitCostManager.GetCostForUnitType(UnitType.Tank1));
    }

    void OnBuyRocketButtonClicked()
    {
        UnitManager.Instance.PurchaseUnit(UnitType.RocketCar);
        PlayerInfo.Instance.AddMoney(-UnitCostManager.GetCostForUnitType(UnitType.RocketCar));
    }

    void OnBuyMechButtonClicked()
    {
        UnitManager.Instance.PurchaseUnit(UnitType.Mech);
        PlayerInfo.Instance.AddMoney(-UnitCostManager.GetCostForUnitType(UnitType.Mech));
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
    // 戰術指令購買
    private void onToggleAttackBuff(bool isChecked)
    {
        TacticalSkillManager.Instance.OnSkillToggle(isChecked, TacticalSkillType.AttackBuff);
    }
    private void onToggleAttackSpeedBuff(bool isChecked)
    {
        TacticalSkillManager.Instance.OnSkillToggle(isChecked, TacticalSkillType.AttackSpeedBuff);
    }
    private void UpdateSkillToggles()
    {
        // 如果達到上限，禁用未選中的 Toggle
        if (TacticalSkillManager.Instance.IsReachSkillSlotLimit())
        {
            // 禁用未選中的技能 Toggle
            if (!attackBuffToggle.isOn) attackBuffToggle.interactable = false;
            if (!attackSpeedBuffToggle.isOn) attackSpeedBuffToggle.interactable = false;
        }
        else
        {
            // 恢復 Toggle 可用
            attackBuffToggle.interactable = true;
            attackSpeedBuffToggle.interactable = true;
        }
    }
    public void UpdateSkillUI()
    {
        List<TacticalSkillType> skills = TacticalSkillManager.Instance.GetPlayerTacticalSkills();

        if (skills.Count == 0)
        {
            skillDisplayText.text = "not slected";
        }
        else
        {
            skillDisplayText.text = "Purchased Skills:" + string.Join("、", skills);
        }
    }
    private void HandleSceneLoaded()
    {
        // 根據場景名稱來決定是否顯示或隱藏
        if (GlobalSceneManager.Instance.GetCurrentSceneIndex() != (int)SceneEnum.OrderScene)
        {
            this.gameObject.SetActive(false); // 隱藏自己
        }
        else
        {
            this.gameObject.SetActive(true);  // 顯示自己
        }
    }
}