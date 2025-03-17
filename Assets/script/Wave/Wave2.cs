using UnityEngine;
public class Wave2 : BaseWave
{
    public GameObject normalBulletPrefab;

    public override void StartWave()
    {
        base.StartWave();
        Debug.Log("Level 2 has started");
    }
}