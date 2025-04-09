using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI; // 引入 UI 库

public class TacticalSkillManager : Singleton<TacticalSkillManager>
{
    public int skillSlot = 2;

    // 存放玩家已選擇的技能列表
    private HashSet<TacticalSkillType> playerTacticalSkillSet = new HashSet<TacticalSkillType>();

    // skillBar
    public Canvas skillBarCanvas;
    public Transform skillButtonContainer;  // SkillBar 上的容器，用來擺放技能按鈕


    void Start()
    {
    }


    // 獲取玩家技能列表
    public List<TacticalSkillType> GetPlayerTacticalSkills()
    {
        return playerTacticalSkillSet.ToList(); // ✅ 避免直接暴露 HashSet;
    }
    public bool IsReachSkillSlotLimit(){
        // Debug.Log($"是否已達上限{GetPlayerTacticalSkills().Count >= skillSlot}, 技能上限{skillSlot},當前技能列表:{GetPlayerTacticalSkills().Count}");
        return GetPlayerTacticalSkills().Count >= skillSlot;
    }
    // onToggl
    public void OnSkillToggle(bool isChecked, TacticalSkillType skillType)
    {
        if (isChecked)
        {
            if (playerTacticalSkillSet.Count >= skillSlot)
            {
                Debug.Log("已達技能上限");
                return;
            }
            AddSkill(skillType);
        }
        else
        {
            RemoveSkill(skillType);
        }
        OrderMenu.Instance.UpdateSkillUI();
    }

    // 添加技能
    public void AddSkill(TacticalSkillType skillType)
    {
        bool added = playerTacticalSkillSet.Add(skillType);
        if (added)
        {
            Debug.Log($"成功添加技能: {skillType}");
        }
        else
        {
            Debug.Log($"技能已經存在: {skillType}");
        }
    }

    // 移除技能
    public void RemoveSkill(TacticalSkillType skillType)
    {
        bool removed = playerTacticalSkillSet.Remove(skillType);
        if (removed)
        {
            Debug.Log($"成功移除技能: {skillType}");
        }
        else
        {
            Debug.Log($"技能不存在: {skillType}");
        }
    }
    // SkillBar相關
    public void InitializeSkillBar()
    {
        Debug.Log("執行技能列表初始化");
        // 清空所有技能按鈕
        foreach (Transform child in skillButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // 根據玩家選擇的技能列表創建技能按鈕
        foreach (var skillType in playerTacticalSkillSet)
        {
            // 根據技能類型創建對應的 TacticalSkill 實例
            TacticalSkill skillInstance = CreateSkillInstance(skillType);
            if (skillInstance == null) continue;

            // 創建技能按鈕
            GameObject skillButton = new GameObject(skillType.ToString()); // 創建一個新的技能按鈕
            skillButton.AddComponent<RectTransform>(); // 給技能按鈕添加 RectTransform（UI 元素必須有）
            Button button = skillButton.AddComponent<Button>(); // 添加 Button 組件
            RectTransform rectTransform = skillButton.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(100, 50); // 设置适当的按钮大小

            // 設置按鈕樣式，這裡你可以加入圖片、文字等
            Text buttonText = skillButton.AddComponent<Text>();
            buttonText.text = skillType.ToString(); // 按鈕顯示技能名稱

            // 註冊按鈕點擊事件
            button.onClick.AddListener(() => OnSkillClick(skillInstance)); // 使用具體技能實例作為參數

            // 設置按鈕的位置或樣式，這裡我們簡單地將按鈕加入到容器中
            skillButton.transform.SetParent(skillButtonContainer);
        }
    }

    // 根據 TacticalSkillType 創建相應的 TacticalSkill 子類實例
    private TacticalSkill CreateSkillInstance(TacticalSkillType skillType)
    {
        GameObject skillObject = new GameObject(skillType.ToString()); // 創建一個新的 GameObject
        TacticalSkill skillInstance = null;

        // 根據 skillType 來創建對應的 TacticalSkill 子類實例
        switch (skillType)
        {
            case TacticalSkillType.AttackBuff:
                skillInstance = skillObject.AddComponent<AttackBuff>(); // 使用 AddComponent 動態創建 SkillA 實例
                break;
            case TacticalSkillType.AttackSpeedBuff:
                skillInstance = skillObject.AddComponent<AttackSpeedBuff>(); // 使用 AddComponent 動態創建 SkillB 實例
                break;
            // 可以添加更多的技能類型
            default:
                Debug.LogError("未實現的技能類型");
                break;
        }

        return skillInstance;
    }

    // 當技能按鈕被點擊時呼叫的回調
    private void OnSkillClick(TacticalSkill skill)
    {
        if (skill != null)
        {
            // 使用具體的 TacticalSkill 實例來施放技能
            var units = UnitManager.Instance.GetPlayerUnits();
            foreach (var unit in units)
            {
                skill.Buff(unit);
            }
        }
    }
}