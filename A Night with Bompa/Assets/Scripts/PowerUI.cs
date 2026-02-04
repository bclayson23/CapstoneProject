using UnityEngine;
using UnityEngine.UI;

public class PowerUI : MonoBehaviour
{
    public Image powerBar;
    public Text powerText;
    public PlayerController playerController;

    void Update()
    {
        if (playerController != null)
        {
            float powerPercent = playerController.GetPowerPercentage();

            if (powerBar != null)
            {
                powerBar.fillAmount = powerPercent;
                powerBar.color = Color.Lerp(Color.red, Color.green, powerPercent);
            }

            if (powerText != null)
            {
                powerText.text = $"POWER: {Mathf.RoundToInt(powerPercent * 100)}%";
            }
        }
    }
}