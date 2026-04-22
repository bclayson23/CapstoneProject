using UnityEngine;
using UnityEngine.UI;

public class PowerUI : MonoBehaviour
{
    public Text powerText;
    public PlayerController playerController;

    void Update()
    {
        if (playerController != null)
        {
            float powerPercent = playerController.GetPowerPercentage();

            if (powerText != null)
            {
                powerText.text = $"POWER: {Mathf.RoundToInt(powerPercent * 100)}%";
            }
        }
    }
}