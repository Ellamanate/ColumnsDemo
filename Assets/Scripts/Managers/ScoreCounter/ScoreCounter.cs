using UnityEngine;


[CreateAssetMenu(fileName = "ScoreCounter", menuName = "Managers/ScoreCounter", order = 5)]
public class ScoreCounter : ScriptableObject, IManager, IScoreCounter
{
    public int Score => state.Score;

    protected SceneData sceneData;
    protected ScoreState state;
    protected int targetScore;

    public void BaseAwake()
    {
        if (ToolBox.GetData(out sceneData))
        {
            state = Serialyzer.RegisterState<ScoreState>("ScoreState");

            targetScore = sceneData.TargetScore;

            OnAwake();

            Subscribes();
        }
    }

    public void BaseStart() => Events.OnScoreChanged.Publish(state.Score);

    public void BaseUpdate() { }

    public void ClearState() 
    { 
        state.Score = 0;
        state.Complete = false;
    }

    protected virtual void OnAwake() { }

    protected virtual void Subscribes()
    {
        Events.OnDeleteBlocks.Subscribe(ChangeScore);
    }

    protected virtual void UnSubscribes()
    {
        Events.OnDeleteBlocks.UnSubscribe(ChangeScore);
    }

    protected virtual void OnManagerDisable() { }

    private void OnDisable()
    {
        OnManagerDisable();

        UnSubscribes();
    }

    protected virtual void ChangeScore(Line[] lines, int combo)
    {
        foreach (Line line in lines)
            state.Score += line.BlocksInLine.Count * 10 * combo;

        Events.OnScoreChanged.Publish(state.Score);

        if (!state.Complete && targetScore != 0 && state.Score >= targetScore)
        {
            state.Complete = true;
            Events.TargetAction.Publish(sceneData.BlockPrefab.ClipDuration);
        }
    }
}