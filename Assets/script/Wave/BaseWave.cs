using UnityEngine;
public abstract class BaseWave : MonoBehaviour
{
    public virtual void StartWave()
    {
        Debug.Log("Level has started");
    }
}