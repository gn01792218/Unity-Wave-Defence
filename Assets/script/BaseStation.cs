using UnityEngine;

public class BaseStation : MonoBehaviour
{
    private void Awake()
    {
        // 確保有 Renderer 組件
        if (GetComponent<Renderer>() == null)
        {
            Debug.LogWarning($"Renderer is missing on {gameObject.name}. Adding a default MeshRenderer.");
            gameObject.AddComponent<MeshRenderer>(); // 添加一個默認的 MeshRenderer
        }

        // 自動對齊地板
        AlignToGround();
    }

    // This method returns the center position of the BaseStation object
    public Vector3 GetCenterPosition()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError($"Renderer is missing on {gameObject.name}. Please add a Renderer component.");
            return Vector3.zero; // Return a default value to avoid further errors
        }

        // 確保中心點貼在地板上
        Vector3 center = renderer.bounds.center;
        center.y = renderer.bounds.min.y; // 使用物件的最低點作為地板的 Y 軸
        return center;
    }

    // This method ensures the BaseStation is placed on the ground
    private void AlignToGround()
    {
        // 射線檢測地板
        if (Physics.Raycast(transform.position + Vector3.up * 10, Vector3.down, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Ground")) // 確保地板有 "Ground" 標籤
            {
                Vector3 position = transform.position;
                position.y = hit.point.y; // 將 Y 軸對齊到地板高度
                transform.position = position;
                Debug.Log($"{gameObject.name} 已對齊到地板高度: {hit.point.y}");
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} 未檢測到地板，請確認地板是否有正確的標籤。");
            }
        }
        else
        {
            Debug.LogError($"{gameObject.name} 無法檢測到地板，請確認地板是否存在並啟用了 Collider。");
        }
    }
}