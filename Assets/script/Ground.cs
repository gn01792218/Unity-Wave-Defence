using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField] 
    private string layerName = "Ground";
    void Awake()
    {
        // 自動將Ground物件設置到Ground圖層
        gameObject.layer = LayerMask.NameToLayer(layerName);
    }

    // 如果需要，可以添加地板相關的其他功能
    // 例如地形類型、地形屬性等
    public enum GroundType
    {
        Grass,
        Sand,
        Rock,
        Water
    }

    public GroundType groundType = GroundType.Grass;

    // 可以添加其他地板相關的方法
    public bool IsTraversable()
    {
        // 根據地形類型判斷是否可通行
        return groundType != GroundType.Water;
    }
}