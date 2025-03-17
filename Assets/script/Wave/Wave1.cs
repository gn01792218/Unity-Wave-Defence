using UnityEngine;
public class Wave1 : BaseWave
{
    public GameObject normalBulletPrefab;
    public override void StartWave()
    {
        base.StartWave();
        Debug.Log("Level 1 has started");
    }
}