using System.Collections;
using UnityEngine;

public abstract class TacticalSkill : Singleton<TacticalSkill>
{

    public abstract float BuffAmount { get; set; }
    public abstract float BuffDuration { get; set; }
    public abstract float CoolDown { get; set; }

    private bool isOnCooldown = false; // 是否處於冷卻狀態
    // 抽象方法，強制子類實現
    public abstract float OnBuff(Unit targetUnit); //子類去實作，看影響什麼屬性，最後返回該Unit的某個原始屬性數值供後續reset
    public abstract void ResetStatus(Unit targetUnit, float originData);  // 接收一個 Unit 類別的參數，對其產生影響
    public abstract TacticalSkillType GetSkillType();

    // 共用方法
    public void Buff(Unit targetUnit)
    {
        float skillCost = TacticalSkillCostManager.GetCostForTacticalSkillType(GetSkillType());
        // 檢查技能是否可用
        if (!CheckSkillAvailability(skillCost))
        {
            return; // 如果技能不可用，直接返回
        }
        PlayerInfo.Instance.AddTacticalPoints(-skillCost);
        isOnCooldown = true; // 設置冷卻狀態
        var originData = OnBuff(targetUnit);
        StartCoroutine(ResetAfterDelay(targetUnit, originData)); // buff持續完後，reset
        StartCoroutine(CooldownTimer()); // 啟動冷卻計時
    }
    private IEnumerator ResetAfterDelay(Unit targetUnit, float originData)
    {
        yield return new WaitForSeconds(BuffDuration); // 等待 Buff 持續時間
        ResetStatus(targetUnit, originData); // 重置狀態
    }
    private IEnumerator CooldownTimer()
    {
        yield return new WaitForSeconds(CoolDown);
        isOnCooldown = false; // 冷卻結束，允許再次使用
    }
    public bool CheckSkillAvailability(float skillCost)
    {
        // 1. 檢查玩家是否有足夠的戰術點數
        if (PlayerInfo.Instance.TacticalPoints < skillCost)
        {
            Debug.Log("戰術點數不足，無法施放技能！");
            return false;
        }

        // 2. 檢查技能是否處於冷卻狀態
        if (isOnCooldown)
        {
            Debug.Log("技能正在冷卻中，無法使用！");
            return false;
        }

        // 以上檢查都通過，技能可用
        return true;
    }
}