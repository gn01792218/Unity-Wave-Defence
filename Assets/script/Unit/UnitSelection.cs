using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask unitLayer;
    public LayerMask groundLayer;
    public static UnitSelection Instance { get; private set; }  // 单例模式

    private List<Unit> selectedUnits = new List<Unit>();

    // 框選相關變數
    private Vector3 startMousePosition;
    private bool isDragging = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  // 确保只有一个 `UnitSelection` 实例
        }
    }

    void Start()
    {
        unitLayer = LayerMask.GetMask("Unit");
        groundLayer = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        HandleUnitSelection();
        HandleUnitMovement();
    }

    void HandleUnitSelection()
    {
        // 左鍵點擊開始
        if (Input.GetMouseButtonDown(0))
        {
            startMousePosition = Input.mousePosition;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 如果不按Shift鍵，清除先前選擇
            if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
            {
                ClearSelection();
            }

            // 點擊單一單位
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, unitLayer))
            {
                Unit clickedUnit = hit.collider.GetComponent<Unit>();
                if (clickedUnit != null)
                {
                    SelectUnit(clickedUnit);
                }
            }

            isDragging = true;
        }

        // 鬆開滑鼠按鍵
        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                CompleteSelection();
                isDragging = false;
            }
        }
    }

    void HandleUnitMovement()
    {
        // 右鍵移動
        if (Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                foreach (Unit unit in selectedUnits)
                {
                    unit.MoveToPlayerSpceficPosition(hit.point); //移動到玩家指定位置
                }
            }
        }
    }

    void CompleteSelection()
    {
        // 框選多個單位
        Rect selectionRect = GetScreenSelectionRect();

        foreach (Unit unit in FindObjectsByType<Unit>(FindObjectsSortMode.None))
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(unit.transform.position);

            if (selectionRect.Contains(screenPos))
            {
                SelectUnit(unit);
            }
        }
    }

    // 獲取螢幕座標系統中的框選區域（用於單位選擇）
    Rect GetScreenSelectionRect()
    {
        return CreateRect(startMousePosition, Input.mousePosition);
    }

    // 獲取GUI座標系統中的框選區域（用於OnGUI繪製）
    Rect GetGUISelectionRect()
    {
        // 創建GUI座標版本的滑鼠位置
        Vector3 guiStartPos = startMousePosition;
        Vector3 guiCurrentPos = Input.mousePosition;

        // 翻轉Y座標到GUI座標系統
        guiStartPos.y = Screen.height - guiStartPos.y;
        guiCurrentPos.y = Screen.height - guiCurrentPos.y;

        return CreateRect(guiStartPos, guiCurrentPos);
    }

    // 通用方法：從兩個點創建矩形
    // 這個方法處理任意兩點間的矩形創建，無論滑鼠拖曳方向為何
    // 使用 Min/Max 函數確保即使用戶從右下往左上拖曳時也能正確創建矩形
    // 在螢幕座標系統和GUI座標系統中都適用
    Rect CreateRect(Vector3 pointA, Vector3 pointB)
    {
        // 通過比較兩點的x和y座標取最小值，確保lowerLeft總是左下角點
        // 這樣無論滑鼠拖曳方向如何，都能正確計算矩形的左下角
        Vector3 lowerLeft = new Vector3(
            Mathf.Min(pointA.x, pointB.x),  // 取兩點x座標的最小值
            Mathf.Min(pointA.y, pointB.y),  // 取兩點y座標的最小值
            0
        );

        // 通過比較兩點的x和y座標取最大值，確保upperRight總是右上角點
        // 這樣無論滑鼠拖曳方向如何，都能正確計算矩形的右上角
        Vector3 upperRight = new Vector3(
            Mathf.Max(pointA.x, pointB.x),  // 取兩點x座標的最大值
            Mathf.Max(pointA.y, pointB.y),  // 取兩點y座標的最大值
            0
        );

        // 使用Rect.MinMaxRect創建矩形，參數順序為：左、下、右、上
        return Rect.MinMaxRect(lowerLeft.x, lowerLeft.y, upperRight.x, upperRight.y);
    }

    void SelectUnit(Unit unit)
    {
        if (unit.team.teamType != Team.TeamType.Player) return; //不能選擇玩家以外的單位
        if (!selectedUnits.Contains(unit))
        {
            selectedUnits.Add(unit);
            HighlightUnit(unit);
        }
    }

    void ClearSelection()
    {
        foreach (Unit unit in selectedUnits)
        {
            ResetUnitHighlight(unit);
        }
        selectedUnits.Clear();
    }

    void HighlightUnit(Unit unit)
    {
        // 可視化選中效果
        Renderer renderer = unit.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.yellow;
        }
    }

    void ResetUnitHighlight(Unit unit)
    {
        // 還原單位原本顏色
        Renderer renderer = unit.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.white;
        }
    }

    // 繪製框選區域
    void OnGUI()
    {
        if (!isDragging) return;
        Rect rect = GetGUISelectionRect();
        GUI.Box(rect, "");
    }

    public void RemoveUnitFromSelection(Unit unit)
    {
        if (selectedUnits.Contains(unit))
        {
            selectedUnits.Remove(unit);
            ResetUnitHighlight(unit);
        }
    }
}
