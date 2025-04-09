using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class TacticalSkill : MonoBehaviour
{
    //改進 : 不需要Disable的圖片，直接使用圖片灰階效果就好了

    public abstract string skillDisplayName { get; set; }
    public abstract string skillEffectPropertiesText { get; set; }
    public abstract string skillImagePath { get; set; }
    public abstract string skillDisableImagePath { get; set; }
    public abstract float BuffAmount { get; set; }
    public abstract float BuffDuration { get; set; }
    public abstract float CoolDown { get; set; }

    private bool isOnCooldown = false; // 是否處於冷卻狀態
    public Button skillButtonPrefb; //按鈕
    private void Awake()
    {
        skillButtonPrefb = gameObject.GetComponent<Button>();
    }
    public void InitSkillButton()
    {
        Debug.Log("初始化按鈕");

        if (skillButtonPrefb == null)
        {
            Debug.LogError("Skill Button Prefab is not assigned!");
            return;
        }
        // 1. 設定按鈕圖片
        Image buttonImage = skillButtonPrefb.GetComponent<Image>();
        if (buttonImage != null)
        {
            Sprite skillSprite = Resources.Load<Sprite>(skillImagePath);
            if (skillSprite != null)
            {
                buttonImage.sprite = skillSprite;
            }
            else
            {
                Debug.LogWarning("Skill image not found at path: " + skillImagePath);
            }
        }

        // 2. 設定 skillTitle 的文字（TextMeshPro）
        TextMeshProUGUI titleText = skillButtonPrefb.GetComponentInChildren<TextMeshProUGUI>(true);
        if (titleText != null)
        {
            titleText.text = skillDisplayName;
        }
        else
        {
            Debug.LogWarning("找不到 TextMeshProUGUI 來顯示 skill title！");
        }

        // 🔘 加入按鈕點擊監聽
        skillButtonPrefb.onClick.RemoveAllListeners(); // 避免重複註冊
        skillButtonPrefb.onClick.AddListener(OnSkillButtonClicked);
        Debug.Log("初始化按鈕完畢");
    }
    // 抽象方法，強制子類實現
    public string GetSkillInfoDesplay()
    {
        return $"{skillDisplayName} : \n 持續時間: {BuffDuration} \n 冷卻時間:{BuffDuration} \n 技能說明:使我方全體Unit的{skillEffectPropertiesText} + {BuffAmount}";
    }
    public abstract float OnBuff(Unit targetUnit); //子類去實作，看影響什麼屬性，最後返回該Unit的某個原始屬性數值供後續reset
    public abstract void ResetStatus(Unit targetUnit, float originData);  // 接收一個 Unit 類別的參數，對其產生影響
    public abstract TacticalSkillType GetSkillType();

    // 共用方法
    public void Buff(Unit targetUnit)
    {
        float skillCost = TacticalSkillCostManager.GetCostForTacticalSkillType(GetSkillType());
        // 檢查技能是否可用
        if (!CheckSkillAvailability(skillCost))
        {
            return; // 如果技能不可用，直接返回
        }
        PlayerInfo.Instance.AddTacticalPoints(-skillCost);
        var originData = OnBuff(targetUnit);
        StartCoroutine(ResetAfterDelay(targetUnit, originData)); // buff持續完後，reset
        StartCoroutine(CooldownTimer()); // 啟動冷卻計時
    }
    public bool CheckSkillAvailability(float skillCost)
    {
        // 1. 檢查玩家是否有足夠的戰術點數
        if (PlayerInfo.Instance.TacticalPoints < skillCost)
        {
            Debug.Log("戰術點數不足，無法施放技能！");
            return false;
        }

        // 2. 檢查技能是否處於冷卻狀態
        if (isOnCooldown)
        {
            Debug.Log("技能正在冷卻中，無法使用！");
            return false;
        }

        // 以上檢查都通過，技能可用
        return true;
    }
    //內部方法
    // 內部觸發器 - 可包含冷卻時間檢查等
    private void OnSkillButtonClicked()
    {
        if (isOnCooldown)
        {
            Debug.Log($"{skillDisplayName} is on cooldown.");
            return;
        }
        foreach (var unit in UnitManager.Instance.GetPlayerUnits())
        {
            Buff(unit);
        }
    }
    private IEnumerator ResetAfterDelay(Unit targetUnit, float originData)
    {
        yield return new WaitForSeconds(BuffDuration); // 等待 Buff 持續時間
        ResetStatus(targetUnit, originData); // 重置狀態
    }
    private IEnumerator CooldownTimer()
    {
        isOnCooldown = true; // 設置冷卻狀態
        SetSkillInteractable(false);
        yield return new WaitForSeconds(CoolDown);
        isOnCooldown = false; // 冷卻結束，允許再次使用
        SetSkillInteractable(true);
    }
    private void SetSkillInteractable(bool interactable)
    {
        if (interactable) SetSkillButtonImage(skillImagePath); 
        else SetSkillButtonImage(skillDisableImagePath); // 切換圖片為灰階 / 禁用版本
        skillButtonPrefb.interactable = interactable;
        Debug.Log($"設置按鈕互動{interactable},圖片更換為{skillButtonPrefb.GetComponent<Image>().sprite.name}");
    }

    private void SetSkillButtonImage(string path)
    {
        Debug.Log($"更換的圖片路徑為:{path}");
        if (string.IsNullOrEmpty(path)) return;

        Sprite sprite = Resources.Load<Sprite>(path);
        if (sprite == null)
        {
            Debug.LogWarning($"Skill button image not found at path: {path}");
            return;
        }

        Image buttonImage = skillButtonPrefb.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.sprite = sprite;
        }
    }
}