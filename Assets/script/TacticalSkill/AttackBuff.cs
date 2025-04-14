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
        // Debug.Log($"單位初始攻擊力{originData}");
        targetUnit.SetHealthTakenAmount(BuffAmount);
        Debug.Log($"子彈攻擊力增加：{BuffAmount}, 當前目標的子彈加成{targetUnit.GetHealthTakenAmount()}");
        return originData;
    }

    public override void ResetStatus(Unit targetUnit, float originData)
    {
        targetUnit.SetHealthTakenAmount(0);
           Debug.Log($"重置子彈加成回0,單位的子彈傷害加成為{targetUnit.GetHealthTakenAmount()}");
    }
}