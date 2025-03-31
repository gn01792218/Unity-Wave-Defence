using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class UIHelper
{
    public static GameObject CreateCanvas(string name, Transform parent, Vector3 localPosition)
    {
        // 创建一个空的物体专门处理Canvas的旋转
        GameObject canvasRotationHelper = new GameObject("CanvasRotationHelper");
        canvasRotationHelper.transform.SetParent(parent);  // 设置为当前单位的子物体
        canvasRotationHelper.transform.localPosition = Vector3.zero; // 位置保持为零

        //創建canvas本身
        GameObject canvasGO = new GameObject(name);
        canvasGO.transform.SetParent(parent);
        canvasGO.transform.localPosition = localPosition;  // 设置 Canvas 的位置

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;  // 使用 WorldSpace 模式
        canvas.worldCamera = Camera.main;  // 让 Canvas 使用主摄像机
        canvas.sortingOrder = 1;  // 调整渲染顺序，避免被其他物件遮挡

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 1;  // 确保 UI 的比例适合画面大小

        canvasGO.AddComponent<GraphicRaycaster>();  // 添加 GraphicRaycaster 组件

        return canvasGO;  // 返回创建的 Canvas GameObject
    }
    public static Slider CreateSlider(string name, Vector3 position, Transform parent, Color fillColor, float width = 1)
    {
        GameObject sliderGO = new GameObject(name);
        sliderGO.transform.SetParent(parent);
        sliderGO.transform.localPosition = position;

        // 添加 RectTransform，并设置宽度为传入的 width 参数
        RectTransform rectTransform = sliderGO.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(width, 10); // 设置宽度为传入的 width，高度保持为 10

        // 设置 UI 不受父物体缩放影响
        rectTransform.localScale = Vector3.one; // 确保UI元素的缩放是正常的

        Slider slider = sliderGO.AddComponent<Slider>();

        // 添加背景
        GameObject backgroundGO = new GameObject("Background");
        backgroundGO.transform.SetParent(sliderGO.transform, false);
        Image bgImage = backgroundGO.AddComponent<Image>();
        bgImage.color = Color.red;  // 设置背景颜色为红色

        RectTransform bgRect = backgroundGO.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;

        // 添加填充区
        GameObject fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(sliderGO.transform, false);
        Image fillImage = fillGO.AddComponent<Image>();
        fillImage.color = fillColor;  // 填充颜色由传入的参数决定

        RectTransform fillRect = fillGO.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        slider.fillRect = fillRect;
        slider.targetGraphic = fillImage;

        return slider;
    }

    // 创建血量文本
    public static TMP_Text CreateText(Transform parent, string name, Vector3 localPosition)
    {
        // 创建一个新的文本对象
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent);
        textObject.transform.localPosition = localPosition;

        // 添加 TextMeshPro 组件
        TMP_Text text = textObject.AddComponent<TextMeshPro>();
        text.fontSize = 8; // 设置字体大小
        text.color = Color.white; // 设置字体颜色为白色
        text.alignment = TextAlignmentOptions.Center; // 设置文本居中
        text.text = "100/100"; // 默认文本

        // 设置文本的最大宽度，避免随着父物体的变化而拉伸
        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100, rectTransform.sizeDelta.y); // 设置固定宽度，保持高度不变
        rectTransform.rotation = Quaternion.Euler(90, rectTransform.rotation.eulerAngles.y, 270);  // 修复翻转

        return text;
    }
    // 让物体始终朝向摄像机，但不受父物体旋转的影响
    public static void CanvasLookAtCamera(Transform objTransform)
    {
        if (objTransform != null && Camera.main != null)
        {
            // 只让Canvas的父物体旋转，不影响其他UI元素
            Vector3 targetLocalPosition = objTransform.localPosition;

            // 让物体朝向摄像机（仅旋转父物体）
            objTransform.LookAt(Camera.main.transform);

            // 保持Canvas父物体的旋转不变，只在Y轴旋转
            objTransform.rotation = Quaternion.Euler(0, objTransform.rotation.eulerAngles.y, 0);  // 修复翻转

            // 恢复物体的局部位置
            objTransform.localPosition = targetLocalPosition;

            // 保证 Canvas 下的 UI 元素不受旋转影响
            foreach (Transform child in objTransform)
            {
                RectTransform rectTransform = child.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.rotation = Quaternion.identity;  // 重置旋转
                }
            }
        }
    }
}