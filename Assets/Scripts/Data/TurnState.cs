using UnityEngine.Events;
using System.Collections.Generic;


public class TurnState : IState
{
    public UnityEvent SerialyzingCallback => serialyzingCallback;

    public int TurnCounter = 0;
    public int TargetTurns = 3;
    public bool[] States;
    public int[] Counters;

    private UnityEvent serialyzingCallback = new UnityEvent();

    public TurnState() { }

    public void SetStates(int size) => States = new bool[size];

    public void SetCounters(int size) => Counters = new int[size];

    public void OnSerialyzing() { }
    public void OnDeserialyzing() { }
}