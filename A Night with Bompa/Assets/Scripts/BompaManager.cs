using System.Collections;
using UnityEngine;

public class BompaManager : MonoBehaviour
{
    public GameObject bompaInCell;
    public GameObject hallBompa;

    public DoorController leftDoorScript;
    public DoorController rightDoorScript;

    public GameObject officeBompa;

    public GameObject[] roamingBompas;

    public GameObject currentBompa;

    private bool hasEscaped = false;

    public float moveInterval = 10f;
    public float moveChance = 0.5f;

    public GameObject leftHall;
    public GameObject rightHall;
    public GameObject leftDoor;
    public GameObject rightDoor;
    public GameObject cellHall;

    private bool isAtDoor = false;
    private bool readyToAttack = false;
    private bool isLeftDoor = false;

    public float minDoorWait = 10f;
    public float maxDoorWait = 40f;

    float cameraUpTimer = 0f;
    public float maxCameraUpTimeAtDoor = 15f;

    public AudioSource echoSource;
    public AudioSource cleanSource;

    public AudioClip[] doorArrivalSounds;
    public AudioClip[] doorLeaveSounds;
    public AudioClip jumpscareSound;

    private bool waitingForMidnightCall = false;

    private bool playerHasBlocked = false;
    private Coroutine doorRoutine;

    public void ForceToLeftDoor()
    {
        CancelInvoke(nameof(TryMove));

        SetActiveBompa(leftDoor);

        currentState = BompaState.LeftDoor;

        EnterDoorState(true);

    }

    public void ResetBompa()
    {
        hasEscaped = false;
        isAtDoor = false;
        readyToAttack = false;
        playerHasBlocked = false;
        waitingForMidnightCall = false;

        if (doorRoutine != null)
        {
            StopCoroutine(doorRoutine);
            doorRoutine = null;
        }

        if (currentBompa != null)
            currentBompa.SetActive(false);

        if (officeBompa != null)
            officeBompa.SetActive(false);

        foreach (GameObject b in roamingBompas)
            if (b != null) b.SetActive(false);

        if (leftHall != null) leftHall.SetActive(false);
        if (rightHall != null) rightHall.SetActive(false);
        if (cellHall != null) cellHall.SetActive(false);
        if (leftDoor != null) leftDoor.SetActive(false);
        if (rightDoor != null) rightDoor.SetActive(false);

        SetActiveBompa(bompaInCell);
        currentState = BompaState.Cell;

        CancelInvoke(nameof(TryMove));
        InvokeRepeating(nameof(TryMove), moveInterval, moveInterval);
    }

    public enum BompaState
    {
        Cell,
        CellHall,
        Roaming,
        LeftHall,
        RightHall,
        LeftDoor,
        RightDoor
    }

    private BompaState currentState;

    public void OnCameraUp()
    {
        if (!isAtDoor) return;

        DoorController door = isLeftDoor ? leftDoorScript : rightDoorScript;

        if (!door.isClosed)
        {
            readyToAttack = true;
        }
    }

    public void OnCameraDown()
    {
        if (!isAtDoor) return;

        DoorController door = isLeftDoor ? leftDoorScript : rightDoorScript;

        if (door.isClosed)
        {

            ReturnToHall();
            return;
        }

        if (readyToAttack)
        {

            TriggerAttack();
        }
    }

    void Start()
    {
        SetActiveBompa(bompaInCell);
        InvokeRepeating(nameof(TryMove), moveInterval, moveInterval);
    }

    void Update()
    {
        if (isAtDoor)
        {
            PlayerController player = FindObjectOfType<PlayerController>();

            if (player != null && player.IsViewingCameras())
            {
                cameraUpTimer += Time.deltaTime;

                if (cameraUpTimer >= maxCameraUpTimeAtDoor)
                {
                    readyToAttack = true;
                }
            }
            else
            {
                cameraUpTimer = 0f;
            }
        }
    }

    void EscapeCell()
    {
        hasEscaped = true;
        currentState = BompaState.CellHall;

        waitingForMidnightCall = true;

        CameraMonitor camMonitor = FindObjectOfType<CameraMonitor>();
        if (camMonitor != null)
            camMonitor.SetBompaEscaped(true);

        SetActiveBompa(cellHall);
    }

    void SetActiveBompa(GameObject newBompa)
    {
        if (currentBompa != null)
            currentBompa.SetActive(false);

        currentBompa = newBompa;
        currentBompa.SetActive(true);

        CameraMonitor camMonitor = FindObjectOfType<CameraMonitor>();
        if (camMonitor != null)
            StartCoroutine(camMonitor.CameraBlackout());
    }

    void TryMove()
    {
        GameManager gm = FindObjectOfType<GameManager>();

        if (waitingForMidnightCall && gm != null && !gm.midnightCallFinished)
        {
            return;
        }

        if (waitingForMidnightCall && gm != null && gm.midnightCallFinished)
        {
            waitingForMidnightCall = false;
        }

        if (!hasEscaped) return;

        if (isAtDoor) return;

        float roll = Random.value;
        if (roll > moveChance) return;

        DecideNextMove();
    }

    void DecideNextMove()
    {
        switch (currentState)
        {
            case BompaState.CellHall:
                MoveToRoaming();
                return;

            case BompaState.Roaming:
                MoveFromRoaming();
                break;

            case BompaState.LeftHall:
                SetActiveBompa(leftDoor.gameObject);
                currentState = BompaState.LeftDoor;
                EnterDoorState(true);
                break;

            case BompaState.RightHall:
                SetActiveBompa(rightDoor.gameObject);
                currentState = BompaState.RightDoor;
                EnterDoorState(false);
                break;

            case BompaState.LeftDoor:
            case BompaState.RightDoor:
                break;
        }
    }

    void MoveFromRoaming()
    {
        float decision = Random.value;

        if (decision < 0.6f)
        {
            int index = Random.Range(0, roamingBompas.Length);
            SetActiveBompa(roamingBompas[index]);
            currentState = BompaState.Roaming;
        }
        else if (decision < 0.80f)
        {
            SetActiveBompa(leftHall);
            currentState = BompaState.LeftHall;
        }
        else
        {
            SetActiveBompa(rightHall);
            currentState = BompaState.RightHall;
        }
    }

    void MoveToRoaming()
    {
        int index = Random.Range(0, roamingBompas.Length);
        SetActiveBompa(roamingBompas[index]);
        currentState = BompaState.Roaming;
    }

    void ReturnToHall()
    {
        isAtDoor = false;
        readyToAttack = false;

        SetActiveBompa(cellHall);
        currentState = BompaState.CellHall;

        PlayRandomSound(echoSource, doorLeaveSounds);

        CancelInvoke(nameof(TryMove));
        InvokeRepeating(nameof(TryMove), moveInterval, moveInterval);

        if (doorRoutine != null)
            StopCoroutine(doorRoutine);

    }

    public void TriggerAttack()
    {
        if (currentBompa != null)
            currentBompa.SetActive(false);

        if (officeBompa != null)
            officeBompa.SetActive(true);

        if (jumpscareSound != null)
            cleanSource.PlayOneShot(jumpscareSound);

        CancelInvoke(nameof(TryMove));

        StartCoroutine(HandleGameOver());
    }

    void EnterDoorState(bool left)
    {
        isAtDoor = true;
        readyToAttack = false;
        isLeftDoor = left;

        PlayRandomSound(echoSource, doorArrivalSounds);

        cameraUpTimer = 0f;

        CancelInvoke(nameof(TryMove));

        playerHasBlocked = false;

        if (doorRoutine != null)
            StopCoroutine(doorRoutine);

        doorRoutine = StartCoroutine(DoorRoutine());
    }

    void PlayRandomSound(AudioSource source, AudioClip[] clips)
    {
        if (clips.Length == 0 || source == null) return;

        int index = Random.Range(0, clips.Length);
        source.PlayOneShot(clips[index]);
    }

    IEnumerator HandleGameOver()
    {
        PlayerController player = FindObjectOfType<PlayerController>();

        if (player != null)
            player.DisablePlayer();

        yield return new WaitForSeconds(1f);

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
            gm.TriggerGameOver();
    }

    public void OnMidnight()
    {
        if (!hasEscaped)
        {
            EscapeCell();
        }
    }

    IEnumerator DoorRoutine()
    {
        float reactionTime = 15f;

        yield return new WaitForSeconds(reactionTime);

        if (!playerHasBlocked)
        {
            TriggerAttack();
            yield break;
        }

        float wait = Random.Range(minDoorWait, maxDoorWait);

        yield return new WaitForSeconds(wait);

        if (isAtDoor)
        {
            ReturnToHall();
        }
    }

    public void OnDoorClosed(DoorController door)
    {
        if (!isAtDoor) return;

        if ((isLeftDoor && door == leftDoorScript) ||
            (!isLeftDoor && door == rightDoorScript))
        {
            playerHasBlocked = true;
        }
    }
}