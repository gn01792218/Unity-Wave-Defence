using UnityEngine;
public class UnitCostManager: MonoBehaviour
{
    public static float GetCostForUnitType(UnitType unitType)
    {
        return unitType switch
        {
            UnitType.Tank1 => 200f,
            UnitType.RocketCar => 450f,
            UnitType.Mech => 100f,
            _ => 0f
        };
    }
}