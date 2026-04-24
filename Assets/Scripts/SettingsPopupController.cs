using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPopupController : MonoBehaviour
{
    [SerializeField] private GameObject settingsPopup;
    [SerializeField] private GameObject portraitSettingsPopup;
    [SerializeField] private Toggle bgmToggle;
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private Toggle portraitBgmToggle;
    [SerializeField] private Toggle portraitSfxToggle;

    private void Awake()
    {
        bool bgm = PlayerPrefs.GetInt("BGM", 1) == 1;
        bool sfx = PlayerPrefs.GetInt("SFX", 1) == 1;

        bgmToggle.SetIsOnWithoutNotify(bgm);
        sfxToggle.SetIsOnWithoutNotify(sfx);
        bgmToggle.onValueChanged.AddListener(v => { SyncBgm(v); AudioManager.Instance?.SetBGM(v); });
        sfxToggle.onValueChanged.AddListener(v => { SyncSfx(v); AudioManager.Instance?.SetSFX(v); });

        if (portraitBgmToggle != null)
        {
            portraitBgmToggle.SetIsOnWithoutNotify(bgm);
            portraitBgmToggle.onValueChanged.AddListener(v => { SyncBgm(v); AudioManager.Instance?.SetBGM(v); });
        }
        if (portraitSfxToggle != null)
        {
            portraitSfxToggle.SetIsOnWithoutNotify(sfx);
            portraitSfxToggle.onValueChanged.AddListener(v => { SyncSfx(v); AudioManager.Instance?.SetSFX(v); });
        }

        SetActive(false);
    }

    private void SyncBgm(bool v) { bgmToggle.SetIsOnWithoutNotify(v); if (portraitBgmToggle != null) portraitBgmToggle.SetIsOnWithoutNotify(v); }
    private void SyncSfx(bool v) { sfxToggle.SetIsOnWithoutNotify(v); if (portraitSfxToggle != null) portraitSfxToggle.SetIsOnWithoutNotify(v); }

    public void OpenSettings()  => SetActive(true);
    public void CloseSettings() => SetActive(false);

    private void SetActive(bool v)
    {
        settingsPopup.SetActive(v);
        if (portraitSettingsPopup != null) portraitSettingsPopup.SetActive(v);
    }
}
