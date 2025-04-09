using UnityEngine;
public class AttackBuff : TacticalSkill
{
    //假如這個Buff要影響的是子彈的傷害的話
    //要設法改變該Unit的子彈傷害
    public override string skillDisplayName { get; set; } = "奮力攻擊";
    public override string skillEffectPropertiesText { get; set; } = "攻擊";
    public override string skillImagePath { get; set; } = "images/SB-attack";
    public override string skillDisableImagePath { get; set; } = "images/SB-attack2";
    public override float BuffAmount { get; set; } = 100f;
    public override float BuffDuration { get; set; } = 2f;
    public override float CoolDown { get; set; } = 3f;

    public override TacticalSkillType GetSkillType()
    {
        return TacticalSkillType.AttackBuff;
    }

    public override float OnBuff(Unit targetUnit)
    {
        var originData = targetUnit.Damage;
        Debug.Log($"單位初始攻擊力{originData}");
        targetUnit.Damage += BuffAmount; // 增加攻擊力 10
        Debug.Log("攻擊力增加到：" + targetUnit.Damage);
        return originData;
    }

    public override void ResetStatus(Unit targetUnit, float originData)
    {
       targetUnit.Damage = originData; 
       Debug.Log($"重置Damage回{originData},最終傷害為{targetUnit.Damage}");
    }
}