using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Time")]
    public TextMeshProUGUI timeText;
    public int currentHour = 11;
    public float hourLength = 60f;

    private float timer = 0f;
    private bool gameActive = false;

    [Header("Power")]
    public float maxPower = 100f;
    public float currentPower;
    public TextMeshProUGUI powerText;

    public float baseDrain = 0.2f;
    public float doorDrain = 0.5f;
    public float cameraDrain = 0.3f;
    public float lightDrain = 0.2f;

    public PlayerController player;
    public CameraMonitor cameraMonitor;
    public DoorController leftDoor;
    public DoorController rightDoor;

    private bool powerOut = false;

    [Header("Power Out")]
    public Light officeLight;

    void Start()
    {
        ResetGame();
    }

    void Update()
    {
        if (!gameActive) return;

        HandleTime();
        HandlePower();
    }

    public void StartGame()
    {
        gameActive = true;
    }

    public void ResetGame()
    {
        currentHour = 11;
        timer = 0f;
        currentPower = maxPower;
        powerOut = false;
        gameActive = false;

        UpdateTimeUI();
        UpdatePowerUI();
    }

    void HandleTime()
    {
        timer += Time.deltaTime;

        if (timer >= hourLength)
        {
            timer = 0f;

            currentHour++;

            // Wrap around after 12
            if (currentHour > 12)
                currentHour = 1;

            UpdateTimeUI();

            // Midnight trigger
            if (currentHour == 12)
            {
                BompaManager bompa = FindObjectOfType<BompaManager>();
                if (bompa != null)
                    bompa.OnMidnight();
            }

            // Win condition: 6 AM ONLY
            if (currentHour == 6)
            {
                WinGame();
            }
        }
    }

    void UpdateTimeUI()
    {
        string period = (currentHour >= 12 || currentHour < 6) ? "AM" : "PM";

        timeText.text = currentHour + " " + period;
    }

    void HandlePower()
    {
        float drain = baseDrain;

        // Doors
        if (leftDoor.isClosed) drain += doorDrain;
        if (rightDoor.isClosed) drain += doorDrain;

        // Cameras
        if (cameraMonitor != null && cameraMonitor.IsViewingCameras())
            drain += cameraDrain;

        // Lights (STATE BASED now)
        if (leftDoor.lightOn) drain += lightDrain;
        if (rightDoor.lightOn) drain += lightDrain;

        currentPower -= drain * Time.deltaTime;
        currentPower = Mathf.Clamp(currentPower, 0, maxPower);

        UpdatePowerUI();

        if (currentPower <= 0 && !powerOut)
        {
            StartCoroutine(PowerOutSequence());
        }
    }

    void UpdatePowerUI()
    {
        powerText.text = "Power: " + Mathf.RoundToInt(currentPower) + "%";
    }

    IEnumerator PowerOutSequence()
{
    powerOut = true;

    Debug.Log("POWER OUT");

    if (officeLight != null)
        officeLight.enabled = false;

    if (cameraMonitor != null)
        cameraMonitor.ToggleMonitor(false);

    if (leftDoor != null)
    {
        if (leftDoor.isClosed)
            leftDoor.ToggleDoor();

        leftDoor.enabled = false; // disables interaction
    }

    if (rightDoor != null)
    {
        if (rightDoor.isClosed)
            rightDoor.ToggleDoor();

        rightDoor.enabled = false;
    }

    leftDoor.ToggleLight(false);
    rightDoor.ToggleLight(false);

    if (player != null)
        player.enabled = false;

    BompaManager bompa = FindObjectOfType<BompaManager>();
    bompa.ForceToLeftDoor();

    float wait = Random.Range(5f, 15f);
    yield return new WaitForSeconds(wait);

    if (currentHour < 6)
    {
        bompa.TriggerAttack();
    }
}

    void WinGame()
    {
        gameActive = false;

        Debug.Log("6 AM — YOU WIN");

        MenuManager menu = FindObjectOfType<MenuManager>();
        if (menu != null)
        {
            menu.ShowWinScreen(); // we’ll build this next
        }
    }
}
