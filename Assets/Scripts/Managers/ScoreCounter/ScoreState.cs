using UnityEngine.Events;
using UnityEngine;


[System.Serializable]
public class ScoreState : IState
{
    public int Score;
    public bool Complete;
    public UnityEvent SerialyzingCallback => serialyzingCallback;

    private UnityEvent serialyzingCallback = new UnityEvent();

    public ScoreState() => Score = 0;

    public void OnSerialyzing() { }
    public void OnDeserialyzing() { }
}