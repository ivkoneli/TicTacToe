using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPopupController : MonoBehaviour
{
    [SerializeField] private GameObject settingsPopup;
    [SerializeField] private Toggle bgmToggle;
    [SerializeField] private Toggle sfxToggle;

    private void Awake()
    {
        bgmToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("BGM", 1) == 1);
        sfxToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt("SFX", 1) == 1);
        bgmToggle.onValueChanged.AddListener(v => AudioManager.Instance?.SetBGM(v));
        sfxToggle.onValueChanged.AddListener(v => AudioManager.Instance?.SetSFX(v));
        settingsPopup.SetActive(false);
    }

    public void OpenSettings()  => settingsPopup.SetActive(true);
    public void CloseSettings() => settingsPopup.SetActive(false);
}
