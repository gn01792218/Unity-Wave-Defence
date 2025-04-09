using UnityEngine;
public class TacticalSkillCostManager: MonoBehaviour
{
    public static float GetCostForTacticalSkillType(TacticalSkillType unitType)
    {
        return unitType switch
        {
            TacticalSkillType.AttackBuff => 2f,
            TacticalSkillType.AttackSpeedBuff => 2f,
            _ => 0f
        };
    }
}