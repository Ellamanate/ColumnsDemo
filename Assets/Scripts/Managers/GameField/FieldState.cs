using System.Collections.Generic;
using UnityEngine.Events;


[System.Serializable]
public class FieldState : IState
{
    public UnityEvent SerialyzingCallback => serialyzingCallback;
    public List<BlockState> GroundedBlocks = new List<BlockState>();

    private UnityEvent serialyzingCallback = new UnityEvent();

    public FieldState() { }

    public void OnSerialyzing() { }
    public void OnDeserialyzing() { }
}