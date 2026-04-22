using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject gameUI;
    public GameObject gameOverUI;

    public GameObject winUI;

    public PlayerController player;

    void Start()
    {
        Time.timeScale = 0f;

        mainMenuUI.SetActive(true);

        if (gameUI != null)
            gameUI.SetActive(false);

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        if (winUI != null)
            winUI.SetActive(false);
    }

    public void StartGame()
    {
        mainMenuUI.SetActive(false);

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        if (gameUI != null)
            gameUI.SetActive(true);

        Time.timeScale = 1f;

        if (player != null)
            player.enabled = true;

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
            gm.StartGame();

        BompaManager bompa = FindObjectOfType<BompaManager>();
        if (bompa != null)
            bompa.ResetBompa();
    }

    public void ShowGameOver()
    {
        Time.timeScale = 0f;

        if (gameUI != null)
            gameUI.SetActive(false);

        gameOverUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowWinScreen()
    {
        Time.timeScale = 0f;

        if (gameUI != null)
            gameUI.SetActive(false);

        winUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void BackToMenu()
    {
        mainMenuUI.SetActive(true);

        if (gameUI != null)
            gameUI.SetActive(false);

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        if (winUI != null)
            winUI.SetActive(false);

        Time.timeScale = 0f;

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
            gm.ResetGame();

        BompaManager bompa = FindObjectOfType<BompaManager>();
        if (bompa != null)
            bompa.ResetBompa();

        if (player != null)
            player.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}