using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class BompaManager : MonoBehaviour
{
    [Header("All Bompa Positions")]
    public GameObject bompaInCell;
    public GameObject hallBompa;

    public GameObject[] roamingBompas; // kitchen, dining, etc

    public GameObject currentBompa;

    private bool hasEscaped = false;

    public float moveInterval = 10f;
    public float moveChance = 0.5f;

    public GameObject leftHall;
    public GameObject rightHall;
    public GameObject leftDoor;
    public GameObject rightDoor;
    public GameObject cellHall;

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

    void Start()
    {
        SetActiveBompa(bompaInCell);
        InvokeRepeating(nameof(TryMove), moveInterval, moveInterval);

        InvokeRepeating(nameof(TryMove), moveInterval, moveInterval);
    }

    void Update()
    {
        // TEMP TEST: press M to simulate midnight
        if (Input.GetKeyDown(KeyCode.M) && !hasEscaped)
        {
            EscapeCell();
        }
    }

    void EscapeCell()
    {
        hasEscaped = true;
        currentState = BompaState.Roaming;

        // Tell camera system Bompa escaped
        CameraMonitor camMonitor = FindObjectOfType<CameraMonitor>();
        if (camMonitor != null)
            camMonitor.SetBompaEscaped(true);

        SetActiveBompa(hallBompa);
    }

    void SetActiveBompa(GameObject newBompa)
    {
        if (currentBompa != null)
            currentBompa.SetActive(false);

        currentBompa = newBompa;
        currentBompa.SetActive(true);
    }

    void TryMove()
    {
        if (!hasEscaped) return;

        float roll = Random.value;
        if (roll > moveChance) return;

        DecideNextMove();
    }

    void DecideNextMove()
    {
        switch (currentState)
        {
            case BompaState.CellHall:
                MoveToRoaming(); // FORCE this
                break;

            case BompaState.Roaming:
                MoveFromRoaming();
                break;

            case BompaState.LeftHall:
                SetActiveBompa(leftDoor);
                currentState = BompaState.LeftDoor;
                break;

            case BompaState.RightHall:
                SetActiveBompa(rightDoor);
                currentState = BompaState.RightDoor;
                break;

            case BompaState.LeftDoor:
            case BompaState.RightDoor:
                // stays here until attack logic
                break;
        }
    }

    void MoveFromRoaming()
    {
        float decision = Random.value;

        // 50% stay roaming
        if (decision < 0.6f)
        {
            int index = Random.Range(0, roamingBompas.Length);
            SetActiveBompa(roamingBompas[index]);
            currentState = BompaState.Roaming;
        }
        // 20% go left hall
        else if (decision < 0.80f)
        {
            SetActiveBompa(leftHall);
            currentState = BompaState.LeftHall;
        }
        // 20% go right hall
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
}