using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GridLayoutGroup recordsTable;
    [SerializeField] private SliderExtension musicVolume;
    [SerializeField] private SliderExtension soundVolume;
    [SerializeField] private SliderExtension opacity;
    [SerializeField] private Toggle mute;
    [SerializeField] private Text record;
    [SerializeField] private RectTransform[] levels;

    private void Awake()
    {
        Time.timeScale = 1;

        Sound.PlayMusic();

        musicVolume.onValueChanged.AddListener(OnChangeMusicVolume);
        musicVolume.onDragEnd.AddListener(SaveMusicVolume);

        soundVolume.onValueChanged.AddListener(OnChangeSoundVolume);
        soundVolume.onDragEnd.AddListener(SaveSoundVolume);

        opacity.onDragEnd.AddListener(SaveOpacity);

        mute.onValueChanged.AddListener(MuteAudio);

        soundVolume.value = PlayerPrefs.GetFloat("SoundVolume", 0.5f);
        musicVolume.value = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        opacity.value = PlayerPrefs.GetFloat("Opacity", 0.5f);
        mute.isOn = PlayerPrefs.GetInt("MuteAudio", 0).ToBool();

        SetRecord();

        continueButton.SetActive(PlayerPrefs.GetInt("CanContinue", 0).ToBool());
    }

    private void OnDisable()
    {
        musicVolume.onValueChanged.RemoveListener(OnChangeMusicVolume);
        musicVolume.onDragEnd.RemoveListener(SaveMusicVolume);

        soundVolume.onValueChanged.RemoveListener(OnChangeSoundVolume);
        soundVolume.onDragEnd.RemoveListener(SaveSoundVolume);

        opacity.onDragEnd.RemoveListener(SaveOpacity);

        mute.onValueChanged.RemoveListener(MuteAudio);
    }

    private void OnChangeMusicVolume(float value) => Sound.ChangeMusicVolume(value);

    private void OnChangeSoundVolume(float value) => Sound.ChangeSoundVolume(value);

    private void SaveSliderValue(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

    private void SaveMusicVolume(float value) => SaveSliderValue("MusicVolume", value);

    private void SaveSoundVolume(float value) => SaveSliderValue("SoundVolume", value);

    private void SaveOpacity(float value) => SaveSliderValue("Opacity", value);

    private void MuteAudio(bool mute) => Sound.Mute(mute);

    private void SetRecord() 
    {
        GameResult loadedRecord = RecordsCollector.GetRecord("Endless");

        record.text = loadedRecord.Score.ToString();
    }
}
