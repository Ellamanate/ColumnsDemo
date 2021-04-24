using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class DataContainer : MonoBehaviour { }

public class SceneData : DataContainer
{
    public Transform Grid => grid;
    public Transform BlockParent => blockParent;
    public Transform BackGround => backGround;
    public Transform UpBorder => upBorder;
    public ButtonExtension LeftButton => leftButton;
    public ButtonExtension RightButton => rightButton;
    public ButtonExtension DownButton => downButton;
    public ButtonExtension ShakeButton => shakeButton;
    public RectTransform RestartButton => restartButton;
    public RectTransform NextFigure => nextFigure;
    public RectTransform Targets => targets;
    public Text PauseText => pauseText;
    public Text DescriptionText => descriptionText;
    public Text LevelDisplay => levelDisplay;
    public Text ScoreDisplay => scoreDisplay;
    public Animator CanvasAnimator => canvasAnimator;
    public Image BlockUI => settings.BlockUI;
    public GameObject CellPrefab => settings.CellPrefab;
    public Block BlockPrefab => settings.BlockPrefab;
    public Colors Colors => settings.Colors;
    public GameState GameState => settings.GameState;
    public Sprite PortraitSprite => settings.PortraitSprite;
    public string GodsName => settings.GodsName;
    public float StartTickDelay => settings.StartTickDelay;
    public float MinTickDelay => settings.MinTickDelay;
    public int Width => settings.Width;
    public int Height => settings.Height;
    public int MaxFigureHeight => settings.MaxFigureHeight;
    public int Level => settings.Level;
    public int ScoreToLevelUp => settings.ScoreToLevelUp;
    public float TickChangeWithLevel => settings.TickChangeWithLevel;
    public int TargetScore => settings.TargetScore;

    [SerializeField] private Transform blockParent;
    [SerializeField] private Transform grid;
    [SerializeField] private Transform backGround;
    [SerializeField] private Transform upBorder;
    [SerializeField] private ButtonExtension leftButton;
    [SerializeField] private ButtonExtension rightButton;
    [SerializeField] private ButtonExtension downButton;
    [SerializeField] private ButtonExtension shakeButton;
    [SerializeField] private RectTransform restartButton;
    [SerializeField] private RectTransform nextFigure;
    [SerializeField] private RectTransform targets;
    [SerializeField] private Image portrait;
    [SerializeField] private Text godsNameDisplay;
    [SerializeField] private Text pauseText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text levelDisplay;
    [SerializeField] private Text scoreDisplay;
    [SerializeField] private Animator canvasAnimator;
    [SerializeField] private UIScaler scaler;
    [SerializeField] private SceneSettings settings;
    private List<IManager> managersCopy = new List<IManager>();

    public void Save() => Serialyzer.Save();

    public void Restart()
    {
        Clear();

        PlayerPrefs.SetInt("CanContinue", false.ToInt());
        PlayerPrefs.Save();

        ToolBox.AddData(this);

        settings.Restart();

        Init();
    }

    private void Awake()
    {
        ToolBox.AddData(this);

        SceneSettings loadedSettings = (SceneSettings)Resources.Load("Settings/" + PlayerPrefs.GetString("LastLevel"), typeof(SceneSettings));

        if (loadedSettings == null)
            loadedSettings = (SceneSettings)Resources.Load("Settings/Endless", typeof(SceneSettings));

        settings = Instantiate(loadedSettings);
        settings.Init();

        if (portrait != null)
        {
            if (settings.PortraitSprite != null)
                portrait.sprite = settings.PortraitSprite;
            else
                portrait.gameObject.SetActive(false);
        }

        scaler.Scaling(this);
    }

    private void Start() => Init();

    private void ShowMessage()
    {
        string[] lines = settings.Description.Split('/');
        string message = string.Empty;

        foreach (string line in lines)
            message += line + '\n';

        descriptionText.text = message;
}

    private void Init()
    {
        foreach (ScriptableObject manager in settings.Managers)
        {
            if (manager != null && manager.GetType().GetInterface(typeof(IManager).Name) != null)
            {
                ScriptableObject copy = Instantiate(manager);
                ToolBox.AddManager(copy);
                managersCopy.Add(((IManager)copy));
                ((IManager)copy).BaseAwake();
            }
        }

        foreach (IManager manager in managersCopy)
            manager.BaseStart();

        godsNameDisplay.text = settings.GodsName;

        pauseText.text = "Pause";

        ShowMessage();

        PlayerPrefs.SetInt("CanContinue", true.ToInt());
        PlayerPrefs.Save();

        Events.OnInit.Invoke();
    }

    private void Update()
    {
        for (int i = 0; i < managersCopy.Count; i++)
            managersCopy[i].BaseUpdate();
    }

    private void OnEnable()
    {
        Events.OnScoreChanged.Subscribe(CalculateLevel);
        Events.TargetAction.Subscribe(settings.RegistrTargetAction);
    }

    private void OnDisable()
    {
        Events.OnScoreChanged.UnSubscribe(CalculateLevel);
        Events.TargetAction.UnSubscribe(settings.RegistrTargetAction);

        foreach (ScriptableObject manager in managersCopy)
            DestroyImmediate(manager, true);

        ToolBox.ClearScene();
    }

    private void OnApplicationPause() => Save();
    private void OnApplicationQuit() => Save();

    private void Clear()
    {
        foreach (ScriptableObject manager in managersCopy)
        {
            ((IManager)manager).ClearState();
            DestroyImmediate(manager, true);
        }

        managersCopy.Clear();

        ToolBox.ClearScene();
    }

    private void CalculateLevel(int score) => settings.CalculateLevel(score);
}