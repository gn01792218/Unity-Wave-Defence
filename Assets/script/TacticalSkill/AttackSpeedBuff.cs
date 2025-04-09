using UnityEngine;
public class AttackSpeedBuff : TacticalSkill
{
    public override string skillDisplayName { get; set; } = "疾風攻擊";
    public override string skillEffectPropertiesText { get; set; } = "攻速";
    public override string skillImagePath { get; set; } = "images/SB-atkSpeedUp";
    public override string skillDisableImagePath { get; set; } = "images/SB-atkSpeedUp2";
    public override float BuffAmount { get; set; } = 10f;
    public override float BuffDuration { get; set; } = 5f;
    public override float CoolDown { get; set; } = 6f;

    public override TacticalSkillType GetSkillType()
    {
        return TacticalSkillType.AttackSpeedBuff;
    }
    public override float OnBuff(Unit targetUnit)
    {
        var originData = targetUnit.AttackCooldown;
        Debug.Log($"單位初始攻擊冷卻{originData}");
        if(originData - BuffAmount < 0) targetUnit.AttackCooldown = 1;
        else targetUnit.AttackCooldown -= BuffAmount;
        Debug.Log("攻擊冷卻增加到：" + targetUnit.AttackCooldown);
        return originData;
    }
    public override void ResetStatus(Unit targetUnit, float originData)
    {
       targetUnit.AttackCooldown = originData; 
       Debug.Log($"重置攻擊冷卻時間回{originData},最終傷害為{targetUnit.AttackCooldown}");
    }
}