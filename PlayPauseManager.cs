using UnityEngine;
using UnityEngine.UI;

public class PlayPauseManager : MonoBehaviour
{
    [Header("UI")]
    public Button button;
    public GameObject playIcon;   // pokazuje się gdy gra jest PAUSED
    public GameObject pauseIcon;  // pokazuje się gdy gra jest PLAYING

    private bool isPaused = false;

    private void Start()
    {
        // Gra startuje jako PLAY (Time.timeScale = 1)
        Time.timeScale = 1f;
        isPaused = false;

        UpdateIcons();

        button.onClick.AddListener(TogglePlayPause);
    }

    private void TogglePlayPause()
    {
        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0f : 1f;

        UpdateIcons();
    }

    private void UpdateIcons()
    {
        // PLAY ICON widoczny gdy gra jest spauzowana
        playIcon.SetActive(isPaused);

        // PAUSE ICON widoczny gdy gra działa
        pauseIcon.SetActive(!isPaused);
    }
}
