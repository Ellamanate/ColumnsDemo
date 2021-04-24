using UnityEngine.Events;


[System.Serializable]
public class SpawnerState : IState
{
    public UnityEvent SerialyzingCallback => serialyzingCallback;
    public float TickDelay { get; set; }

    public BlockState[] CurrentBlockStates;
    public BlockState[] NextBlockStates;

    private UnityEvent serialyzingCallback = new UnityEvent();

    public SpawnerState() 
    {
        CurrentBlockStates = new BlockState[0];
        NextBlockStates = new BlockState[0];
    }

    public void OnSerialyzing() { }
    public void OnDeserialyzing() { }
}