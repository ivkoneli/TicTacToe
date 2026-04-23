using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip whooshClip;
    [SerializeField] private AudioClip popClip;
    [SerializeField] private AudioClip lineDrawnClip;
    [SerializeField] private AudioClip winClip;

    private AudioSource _bgmSource;
    private AudioSource _sfxSource;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.clip = musicClip;
        _bgmSource.loop = true;
        _bgmSource.playOnAwake = false;
        _bgmSource.volume = 0.5f;

        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.loop = false;
        _sfxSource.playOnAwake = false;

        ApplySettings();
    }

    public void ApplySettings()
    {
        bool bgm = PlayerPrefs.GetInt("BGM", 1) == 1;
        bool sfx = PlayerPrefs.GetInt("SFX", 1) == 1;
        _bgmSource.mute = !bgm;
        _sfxSource.mute = !sfx;
        if (!_bgmSource.isPlaying) _bgmSource.Play();
    }

    public void SetBGM(bool on)
    {
        PlayerPrefs.SetInt("BGM", on ? 1 : 0);
        PlayerPrefs.Save();
        _bgmSource.mute = !on;
    }

    public void SetSFX(bool on)
    {
        PlayerPrefs.SetInt("SFX", on ? 1 : 0);
        PlayerPrefs.Save();
        _sfxSource.mute = !on;
    }

    public void PlayClick()  { if (clickClip)  _sfxSource.PlayOneShot(clickClip); }
    public void PlayHover()  { if (hoverClip)  _sfxSource.PlayOneShot(hoverClip); }
    public void PlayWhoosh() { if (whooshClip) _sfxSource.PlayOneShot(whooshClip); }
    public void PlayPop()       { if (popClip)       _sfxSource.PlayOneShot(popClip, 0.5f); }
    public void PlayLineDrawn() { if (lineDrawnClip) _sfxSource.PlayOneShot(lineDrawnClip); }
    public void PlayWin()       { if (winClip)       _sfxSource.PlayOneShot(winClip); }
}
