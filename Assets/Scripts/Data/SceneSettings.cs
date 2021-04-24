using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SceneSettings", menuName = "Data/SceneSettings", order = 0)]
public class SceneSettings : ScriptableObject
{
    public IReadOnlyCollection<ScriptableObject> Managers => managers.AsReadOnly();
    public Image BlockUI => blockUI;
    public GameObject CellPrefab => cellPrefab;
    public Block BlockPrefab => blockPrefab;
    public Colors Colors => colors;
    public GameState GameState => gameState;
    public Sprite PortraitSprite => portraitSprite;
    public string GodsName => godsName;
    public float StartTickDelay => startTickDelay;
    public float MinTickDelay => minTickDelay;
    public int Width => width;
    public int Height => height;
    public int MaxFigureHeight => maxFigureHeight;
    public int Level => level;
    public int ScoreToLevelUp => scoreToLevelUp;
    public float TickChangeWithLevel => tickChangeWithLevel;
    public int TargetScore => targetScore;
    public string Description => description;
    public Sprite[] DescriptionImages => descriptionImages;

    [SerializeField] private List<ScriptableObject> managers = new List<ScriptableObject>();
    [SerializeField] private Image blockUI;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Block blockPrefab;
    [SerializeField] private Colors colors;
    [SerializeField] private GameState gameState;
    [SerializeField] private Sprite portraitSprite;
    [SerializeField] private string godsName;
    [SerializeField] private float startTickDelay;
    [SerializeField] private float minTickDelay;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int maxFigureHeight;
    [SerializeField] private int startLevel = 1;
    [SerializeField] private int scoreToLevelUp = 40;
    [SerializeField] private float tickChangeWithLevel = 0.01f;
    [SerializeField] private int targetScore = 0;
    [SerializeField] private int actionsToWin = 1;
    [SerializeField] private string description;
    [SerializeField] private Sprite[] descriptionImages;
    private ActionsState actionsState;
    private int level;

    public void Init()
    {
        actionsState = Serialyzer.RegisterState<ActionsState>("ActionsState");
    }

    public void Restart()
    {
        level = startLevel;
        actionsState.TargetedActions = 0;
    }

    public void CalculateLevel(int score)
    {
        int newLevel = startLevel + Mathf.FloorToInt(score / scoreToLevelUp);

        if (newLevel > level) 
        {
            level = newLevel;
            Events.OnLevelChange.Invoke();
        }
    }

    public void RegistrTargetAction(float delay) => ToolBox.EnableCoroutine(RegistrDelay(delay));

    private IEnumerator RegistrDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        actionsState.TargetedActions++;

        if (actionsState.TargetedActions >= actionsToWin)
            Events.GameWon.Invoke();
    }
}

public class ActionsState : IState
{
    public int TargetedActions;
    public UnityEvent SerialyzingCallback => serialyzingCallback;

    private UnityEvent serialyzingCallback = new UnityEvent();

    public ActionsState()
    {
        TargetedActions = 0;
    }

    public void OnSerialyzing() { }

    public void OnDeserialyzing() { }
}