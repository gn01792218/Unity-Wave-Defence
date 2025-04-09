using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;       // 相機移動速度
    public float zoomSpeed = 5f;        // 相機縮放速度
    public float minZoom = 10f;         // 最小縮放距離
    public float maxZoom = 50f;         // 最大縮放距離
    public float edgeBuffer = 200f; // 邊界範圍（像素）

    private Camera mainCamera;          // 主相機

    void Start()
    {
        mainCamera = Camera.main;  // 確保使用主相機
    }

    void Update()
    {
        HandleMovement();
        HandleEdgeMovement();
        HandleZoom();
    }

    // 控制相機的移動
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");  // 水平方向的輸入（A/D 或 左/右鍵）
        float vertical = Input.GetAxis("Vertical");      // 垂直方向的輸入（W/S 或 上/下鍵）

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;

        // 根據移動方向更新相機的位置
        transform.Translate(moveDirection, Space.World);  // 使用世界座標移動，避免相機的旋轉影響移動方向
    }
    void HandleEdgeMovement()
    {
        // float horizontalMove = 0f;
        // float verticalMove = 0f;

        // // 檢查滑鼠位置，若超過邊界範圍則開始移動
        // if (Input.mousePosition.x <= edgeBuffer)
        // {
        //     horizontalMove = -1f; // 向左移動
        // }
        // else if (Input.mousePosition.x >= Screen.width - edgeBuffer)
        // {
        //     horizontalMove = 1f; // 向右移動
        // }

        // if (Input.mousePosition.y <= edgeBuffer)
        // {
        //     verticalMove = -1f; // 向下移動
        // }
        // else if (Input.mousePosition.y >= Screen.height - edgeBuffer)
        // {
        //     verticalMove = 1f; // 向上移動
        // }

        // // 根據滑鼠位置的偏移來移動相機
        // Vector3 moveDirection = new Vector3(horizontalMove, verticalMove, 0);
        // transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }
    // 控制相機的縮放
    void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");  // 滾輪的輸入

        if (scrollInput != 0)
        {
            // 根據滾輪輸入調整相機的縮放
            float newZoom = mainCamera.orthographicSize - scrollInput * zoomSpeed;  // 這裡使用正交相機來縮放

            // 限制相機的縮放範圍
            mainCamera.orthographicSize = Mathf.Clamp(newZoom, minZoom, maxZoom);
        }
    }
}