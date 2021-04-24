using UnityEngine.Events;
using System.Collections.Generic;
using System;


public static class Events
{
    public static UnityEvent OnLockFigure = new UnityEvent();
    public static UnityEvent OnGrounded = new UnityEvent();
    public static UnityEvent GameLose = new UnityEvent();
    public static UnityEvent GameLoseAnimationEnd = new UnityEvent();
    public static UnityEvent GameWon = new UnityEvent();
    public static UnityEvent OnLevelChange = new UnityEvent();
    public static UnityEvent OnInit = new UnityEvent();
    public static EventAggregator<bool> GamePause = new EventAggregator<bool>();
    public static EventAggregator<float> TargetAction = new EventAggregator<float>();
    public static EventAggregator<int> OnScoreChanged = new EventAggregator<int>();
    public static EventAggregator<Line[], int> OnDeleteBlocks = new EventAggregator<Line[], int>();
    public static EventAggregator<BlockState[]> NextFigureReady = new EventAggregator<BlockState[]>();
}

public class EventAggregator<T>
{
    private readonly List<Action<T>> callbacks = new List<Action<T>>();

    public void Subscribe(Action<T> callback)
    {
        callbacks.Add(callback);
    }

    public void Publish(T unit)
    {
        foreach (Action<T> callback in callbacks)
            callback(unit);
    }

    public void UnSubscribe(Action<T> callback)
    {
        callbacks.Remove(callback);
    }
}

public class EventAggregator<T1, T2>
{
    private readonly List<Action<T1, T2>> callbacks = new List<Action<T1, T2>>();

    public void Subscribe(Action<T1, T2> callback)
    {
        callbacks.Add(callback);
    }

    public void Publish(T1 unit1, T2 unit2)
    {
        foreach (Action<T1, T2> callback in callbacks)
            callback(unit1, unit2);
    }

    public void UnSubscribe(Action<T1, T2> callback)
    {
        callbacks.Remove(callback);
    }
}