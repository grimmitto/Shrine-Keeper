using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShrineUI : MonoBehaviour
{
    public static ShrineUI Instance;

    [Header("UI References")]
    public Image reputationFill;
    public Image renownFill;
    public TMP_Text moneyText;

    private void Awake()
    {
        Instance = this;
    }

    // Called on game start or whenever values change
    public void UpdateUI(int reputation, int renown, int money)
    {
        // Normalize 0–100 → 0–1
        reputationFill.fillAmount = reputation / 100f;
        renownFill.fillAmount = renown / 100f;

        moneyText.text = money.ToString();
    }
}
