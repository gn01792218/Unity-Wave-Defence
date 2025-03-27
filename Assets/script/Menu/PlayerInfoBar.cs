using UnityEngine;
using TMPro;  // 確保 TextMeshPro 可用

public class PlayerInfoBar : MonoBehaviour
{
    public TMP_Text moneyText;  // 金錢 UI 文字
    public TMP_Text tacticalPointsText; // 戰術點數 UI 文字

    private void Start()
    {
        if (PlayerInfo.Instance != null)
        {
            // 註冊事件監聽數值變更
            PlayerInfo.Instance.OnValueChanged += UpdateUI;
            UpdateUI(); // 先更新一次 UI
        }
    }

    private void UpdateUI()
    {
         if (PlayerInfo.Instance != null)
        {
            moneyText.text = $"$: <color=#FFA500>{PlayerInfo.Instance.Money}</color>";
            tacticalPointsText.text = $"point: <color=#FFFF00>{PlayerInfo.Instance.TacticalPoints}</color>";
        }
    }

    private void OnDestroy()
    {
        // 取消事件監聽，避免錯誤
        if (PlayerInfo.Instance != null)
        {
            PlayerInfo.Instance.OnValueChanged -= UpdateUI;
        }
    }
}

       