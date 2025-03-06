using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> enemyUnits = new List<Unit>();

    void Update()
    {
        foreach (Unit unit in playerUnits)
        {
            if (unit != null)
            {
                Unit nearestEnemy = FindNearestEnemy(unit);
                if (nearestEnemy != null)
                {
                    unit.SetAttackTarget(nearestEnemy.transform.position);
                }
            }
        }
    }

    Unit FindNearestEnemy(Unit unit)
    {
        Unit nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (Unit enemy in enemyUnits)
        {
            float distance = Vector3.Distance(unit.transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }
        return nearest;
    }
}
