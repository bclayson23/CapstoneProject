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
        // Show menu
        mainMenuUI.SetActive(true);

        // Hide other UI
        if (gameUI != null)
            gameUI.SetActive(false);

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        if (winUI != null)
            winUI.SetActive(false);

        // Pause game
        Time.timeScale = 0f;

        // Reset GameManager
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
            gm.ResetGame();

        // Reset Bompa
        BompaManager bompa = FindObjectOfType<BompaManager>();
        if (bompa != null)
            bompa.ResetBompa();

        // Disable player again (since menu is open)
        if (player != null)
            player.enabled = false;

        // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Returned to menu and reset game");
    }
}