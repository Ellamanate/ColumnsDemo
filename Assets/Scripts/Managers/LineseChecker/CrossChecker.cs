using System.Linq;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "CrossChecker", menuName = "Managers/LinesCheckers/CrossChecker", order = 2)]
public class CrossChecker : LinesChecker
{
    [SerializeField] private int crossNumber = 3;
    [SerializeField] private TargetActionUI crossPrefab;
    private int crossCounter = 0;
    private bool complete = false;
    private TargetActionUI[] crosses;
    private TurnState turnState;

    public override Line[] GetLines(Block[,] gridArray)
    {
        lineDeleted = false;
        lines.Clear();

        CheckLines(gridArray);

        for (int i = 0; i < lines.Count; i++)
        {
            if (CrossIntersect(lines[i]))
                CrossConstructed();
        }

        if (!complete && crossCounter >= crossNumber)
        {
            complete = true;
            Events.TargetAction.Publish(sceneData.BlockPrefab.ClipDuration);
        }

        return lines.ToArray();
    }

    protected override void OnAwake()
    {
        if (turnState == null)
        {
            turnState = Serialyzer.RegisterState<TurnState>("TurnState");

            if (turnState.Counters == null || (turnState.Counters != null && turnState.Counters.Length != 1))
                turnState.SetCounters(1);
        }

        crossCounter = turnState.Counters[0];

        ShowCrosses();
    }

    protected override void Subscribes() 
    {
        base.Subscribes();

        Events.GameWon.AddListener(EndGame);
        turnState.SerialyzingCallback.AddListener(SerialyzeTurnState);
    }

    protected override void UnSubscribes()
    {
        base.UnSubscribes();

        Events.GameWon.RemoveListener(EndGame);

        if (turnState != null)
            turnState.SerialyzingCallback.RemoveListener(SerialyzeTurnState);
    }

    private void EndGame() => Events.GameLose.Invoke();

    protected virtual void SerialyzeTurnState()
    {
        if (turnState.Counters.Length >= 1)
            turnState.Counters[0] = crossCounter;
    }

    private void CrossConstructed()
    {
        crossCounter++;

        EnableCrosses();
    }

    private void EnableCrosses()
    {
        for (int i = 0; i < crossNumber && i < crossCounter; i++)
            crosses[i].Enable();
    }

    private bool CrossIntersect(Line line)
    {
        for (int i = 0; i < line.SubLines.Count; i++)
        {
            for (int j = 0; j < line.SubLines.Count; j++)
            {
                if (!line.SubLines[i].Equals(line.SubLines[j]) && line.SubLines[i].Orientation == line.SubLines[j].Orientation && line.SubLines[i].Orientation != LineOrientation.combined)
                {
                    if (line.SubLines[i].BlocksInLine.GetRange(1, line.SubLines[i].BlocksInLine.Count - 2)
                        .Intersect(line.SubLines[j].BlocksInLine.GetRange(1, line.SubLines[j].BlocksInLine.Count - 2))
                        .ToArray().Length > 0)
                        return true;
                }
            }
        }

        return false;
    }

    private void ShowCrosses()
    {
        foreach (Transform child in sceneData.Targets)
            Destroy(child.gameObject);

        crosses = new TargetActionUI[crossNumber];

        for (int i = 0; i < crossNumber; i++)
        {
            TargetActionUI cross = Instantiate(crossPrefab, sceneData.Targets);
            cross.Disable();

            crosses[i] = cross;
        }

        EnableCrosses();
    }
}