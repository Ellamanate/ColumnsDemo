using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


public class Sound : Singleton<Sound>
{
    private AudioSource music;
    private AudioSource soundEffects;
    private List<AudioClip> mainMusic = new List<AudioClip>();
    [SerializeField] private float musicVolume;
    [SerializeField] private float soundVolume;
    [SerializeField] private bool isMute;
    private int musicCounter = 0;
    private int musicClips;

    private void Awake()
    {
        GameObject musicObject = new GameObject();
        musicObject.transform.SetParent(transform);
        music = musicObject.AddComponent<AudioSource>();

        GameObject soundObject = new GameObject();
        soundObject.transform.SetParent(transform);
        soundEffects = soundObject.AddComponent<AudioSource>();

        musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        soundVolume = PlayerPrefs.GetFloat("SoundVolume");
        isMute = PlayerPrefs.GetInt("MuteAudio").ToBool();

        music.volume = isMute ? 0 : musicVolume;
        soundEffects.volume = isMute ? 0 : soundVolume;

        foreach (AudioClip clip in Resources.LoadAll("Sound/Music", typeof(AudioClip)))
            mainMusic.Add(clip);

        musicClips = mainMusic.Count;

        PlayLoop();
    }

    public static void ChangeMusicVolume(float value) => Instance.ChangeVolume(Instance.music, value);

    public static void ChangeSoundVolume(float value) => Instance.ChangeVolume(Instance.soundEffects, value);

    public static void PlayMusic() => Instance.PlayMusic(Instance.music, Instance.mainMusic[0]);

    public static void PauseMusic() => Instance.PauseMusic(Instance.music);

    public static void Mute() => Instance.MuteAudio();

    public static void Mute(bool mute) => Instance.MuteAudio(mute);

    public static void PlayClip(AudioClip clip) => Instance.PlayClip(Instance.soundEffects, clip);

    private void PlayLoop()
    {
        if (musicCounter >= musicClips)
            musicCounter = 0;

        music.clip = mainMusic[musicCounter];
        music.Play();

        UnityEvent callback = new UnityEvent();
        callback.AddListener(PlayLoop);

        StartCoroutine(Timer(music.clip.length, 2.5f, callback));

        musicCounter++;
    }

    private IEnumerator Timer(float targetTime, float delay, UnityEvent callback)
    {
        float timer = 0;

        while (timer <= targetTime + delay)
        {
            timer += Time.unscaledDeltaTime;

            yield return new WaitForEndOfFrame();
        }

        callback.Invoke();
    }

    private void ChangeVolume(AudioSource audioSource, float value)
    {
        if (!isMute)
            audioSource.volume = musicVolume = value;
    }

    private void PlayMusic(AudioSource audioSource, AudioClip clip)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    private void PauseMusic(AudioSource audioSource)
    {
        if (audioSource.isPlaying)
            audioSource.Pause();
    }

    private void PlayClip(AudioSource audioSource, AudioClip clip) => audioSource.PlayOneShot(clip);

    private void MuteAudio()
    {
        isMute = !isMute;
        PlayerPrefs.SetInt("MuteAudio", isMute.ToInt());
        music.volume = isMute ? 0 : musicVolume;
        soundEffects.volume = isMute ? 0 : soundVolume;
    }

    private void MuteAudio(bool mute)
    {
        isMute = mute;
        PlayerPrefs.SetInt("MuteAudio", isMute.ToInt());
        music.volume = isMute ? 0 : musicVolume;
        soundEffects.volume = isMute ? 0 : soundVolume;
    }
}