using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LevelLoader : MonoBehaviour
{
    [SerializeField] private GameObject levelPanel;
    [SerializeField] private Image portrait;
    [SerializeField] private Text godsName;
    [SerializeField] private Text description;
    [SerializeField] private RectTransform altDescription;
    [SerializeField] private Text altDescriptionText;
    [SerializeField] private RectTransform altDescriptionImages;
    [SerializeField] private Text challengeRecord;
    [SerializeField] private AspectRatioFitter ratioFilter;
    private SceneSettings loadSettings;
    private string sceneName;

    public void LoadScene(SceneSettings loadedSettings) => LoadSettings(loadedSettings);

    public void LoadScene() => LoadSettings(loadSettings);

    private void LoadSettings(SceneSettings loadSettings)
    {
        if (loadSettings != null)
        {
            PlayerPrefs.SetString("LastLevel", ((SceneSettings)Resources.Load("Settings/" + loadSettings.name, typeof(SceneSettings))).name);

            PlayerPrefs.SetInt("CanContinue", false.ToInt());
            PlayerPrefs.Save();

            sceneName = loadSettings.name;

            Serialyzer.DropState<TurnState>();

            if (!TryShowAds())
                LoadScene(sceneName);
        }
    }

    public void Continue()
    {
        sceneName = PlayerPrefs.GetString("LastLevel");

        if (!TryShowAds())
            LoadScene(sceneName);
    }

    public void OpenLevelPanel(SceneSettings loadSettings)
    {
        this.loadSettings = loadSettings;

        godsName.text = loadSettings.GodsName;
        portrait.sprite = loadSettings.PortraitSprite;
        ratioFilter.aspectRatio = loadSettings.PortraitSprite.rect.width / loadSettings.PortraitSprite.rect.height;
        challengeRecord.text = RecordsCollector.GetRecord(loadSettings.name).Score.ToString();

        foreach (Transform child in altDescriptionImages)
            Destroy(child.gameObject);

        if (loadSettings.DescriptionImages == null || loadSettings.DescriptionImages.Length == 0)
        {
            description.gameObject.SetActive(true);
            altDescription.gameObject.SetActive(false);
            description.text = loadSettings.Description;
        }
        else
        {
            description.gameObject.SetActive(false);
            altDescription.gameObject.SetActive(true);
            altDescriptionText.text = loadSettings.Description;

            foreach (Sprite sprite in loadSettings.DescriptionImages)
            {
                GameObject obj = new GameObject();
                obj.transform.SetParent(altDescriptionImages);

                Image image = obj.AddComponent<Image>();
                image.sprite = sprite;

                AspectRatioFitter filter = obj.AddComponent<AspectRatioFitter>();
                filter.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
                filter.aspectRatio = 1;

                obj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            }
        }

        levelPanel.SetActive(!levelPanel.activeSelf);
    }

    public void OpenLevel(int number)
    {
        PlayerPrefs.SetInt("OpenChallenge", number);
        PlayerPrefs.Save();
    }

    private void LoadScene(string name)
    {
        if (name == "Endless")
            SceneManager.LoadScene("Endless");
        else
            SceneManager.LoadScene("Challange");
    }

    private bool TryShowAds() => false;
}