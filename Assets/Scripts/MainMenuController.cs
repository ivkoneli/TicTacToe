using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Main Panel")]
    [SerializeField] private GameObject mainPanel;

    [Header("Popups")]
    [SerializeField] private GameObject themePopup;
    [SerializeField] private GameObject statsPopup;
    [SerializeField] private GameObject settingsPopup;
    [SerializeField] private GameObject exitPopup;

    [Header("Theme")]
    [SerializeField] private Button[] themeButtons; // 0=Red 1=Green 2=Yellow

    [Header("Stats")]
    [SerializeField] private TMP_Text totalGamesText;
    [SerializeField] private TMP_Text p1WinsText;
    [SerializeField] private TMP_Text p2WinsText;
    [SerializeField] private TMP_Text drawsText;
    [SerializeField] private TMP_Text avgDurationText;

    [Header("Settings")]
    [SerializeField] private Toggle bgmToggle;
    [SerializeField] private Toggle sfxToggle;


    private int _selectedTheme;
    private CanvasGroup _mainPanelGroup;

    private void Awake()
    {
        _mainPanelGroup = mainPanel.GetComponent<CanvasGroup>();
        if (_mainPanelGroup == null) _mainPanelGroup = mainPanel.AddComponent<CanvasGroup>();
        _selectedTheme = PlayerPrefs.GetInt("SelectedTheme", 0);
        CloseAllPopups();
        RefreshThemeHighlight();

        bgmToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("BGM", 1) == 1);
        sfxToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("SFX", 1) == 1);
        bgmToggle.onValueChanged.AddListener(v => AudioManager.Instance?.SetBGM(v));
        sfxToggle.onValueChanged.AddListener(v => AudioManager.Instance?.SetSFX(v));
    }

    // ─── Main menu buttons ───────────────────────────────────────────────────
    public void OnPlayClicked()     { OpenPopup(themePopup); }
    public void OnStatsClicked()    { RefreshStats(); OpenPopup(statsPopup); }
    public void OnSettingsClicked() { OpenPopup(settingsPopup); }
    public void OnExitClicked()     { OpenPopup(exitPopup); }

    private void OpenPopup(GameObject popup)
    {
        CloseAllPopups();
        popup.SetActive(true);
        SetMainPanelInteractable(false);
    }

    public void CloseAllPopups()
    {
        themePopup.SetActive(false);
        statsPopup.SetActive(false);
        settingsPopup.SetActive(false);
        exitPopup.SetActive(false);
        SetMainPanelInteractable(true);
    }

    private void SetMainPanelInteractable(bool state)
    {
        _mainPanelGroup.interactable = state;
        _mainPanelGroup.blocksRaycasts = state;
    }

    // ─── Theme popup ────────────────────────────────────────────────────────
    public void SelectTheme(int index)
    {
        _selectedTheme = index;
        PlayerPrefs.SetInt("SelectedTheme", index);
        PlayerPrefs.Save();
        RefreshThemeHighlight();
    }

    public void StartGame() => SceneManager.LoadScene("GameScene");

    private void RefreshThemeHighlight()
    {
        for (int i = 0; i < themeButtons.Length; i++)
        {
            var border = themeButtons[i].transform.Find("SelectionBorder");
            if (border != null)
                border.gameObject.SetActive(i == _selectedTheme);
        }
    }

    // ─── Stats popup ────────────────────────────────────────────────────────
    private void RefreshStats()
    {
        totalGamesText.text  = GameStats.TotalGames.ToString();
        p1WinsText.text      = GameStats.Player1Wins.ToString();
        p2WinsText.text      = GameStats.Player2Wins.ToString();
        drawsText.text       = GameStats.Draws.ToString();
        float avg            = GameStats.AverageDuration;
        avgDurationText.text = string.Format("{0:00}:{1:00.0}", Mathf.FloorToInt(avg / 60f), avg % 60f);
    }

    // ─── Exit popup ─────────────────────────────────────────────────────────
    public void ConfirmExit() => Application.Quit();
    public void CancelExit()  => exitPopup.SetActive(false);
}
