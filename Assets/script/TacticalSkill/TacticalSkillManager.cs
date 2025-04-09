using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TacticalSkillManager : Singleton<TacticalSkillManager>
{
    public int skillSlot = 2;

    // 存放玩家已選擇的技能列表
    private HashSet<TacticalSkillType> playerTacticalSkillSet = new HashSet<TacticalSkillType>();

    // skillBar
    public Canvas skillBarCanvas;
    public Transform skillButtonContainer;  // SkillBar 上的容器，用來擺放技能按鈕

    // 獲取玩家技能列表
    public List<TacticalSkillType> GetPlayerTacticalSkills()
    {
        return playerTacticalSkillSet.ToList(); // ✅ 避免直接暴露 HashSet;
    }
    public bool IsReachSkillSlotLimit()
    {
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
    public void GenerateSkillButtons()
    {
        foreach (Transform child in skillButtonContainer)
        {
            Destroy(child.gameObject); // 清空現有技能按鈕
        }

        foreach (TacticalSkillType skillType in playerTacticalSkillSet)
        {
            // 載入對應技能預製體
            GameObject prefab = Resources.Load<GameObject>($"prefabs/Skill/{skillType}");
            if (prefab == null)
            {
                Debug.LogWarning($"找不到技能預製體: prefabs/Skill/SkillButton");
                continue;
            }

            // 產生技能按鈕
            GameObject skillGO = Instantiate(prefab, skillButtonContainer);

            // 根據技能類型實例化具體的技能腳本
            TacticalSkill skill = skillType switch
            {
                TacticalSkillType.AttackBuff => skillGO.AddComponent<AttackBuff>(),  // 假設 HealSkill 是 TacticalSkill 的具體實現
                TacticalSkillType.AttackSpeedBuff => skillGO.AddComponent<AttackSpeedBuff>(),  // 假設 ShieldSkill 是 TacticalSkill 的具體實現
                _ => null  // 預設處理，防止錯誤
            };

            if (skill == null)
            {
                Debug.LogWarning($"技能類型 {skillType} 未實作對應的子類！");
                continue;
            }
            else{
                skill.InitSkillButton();
            }
        }
    }
}