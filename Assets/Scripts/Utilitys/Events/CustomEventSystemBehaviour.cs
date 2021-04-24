using UnityEngine;


public class CustomEventSystemBehaviour : MonoBehaviour
{
    private void Awake()
    {
        CustomEventSystem.AddEvent("OnGameLose", new GameLoseHandler());
        CustomEventSystem.AddEvent("OnGameWon", new GameWonHandler());
        CustomEventSystem.AddEvent("OnInit", new InitHandler());
        CustomEventSystem.AddEvent("OnGameEnd", new GameEndHandler());
        CustomEventSystem.AddEvent("OnGamePause", new GamePauseHandler());
        CustomEventSystem.AddEvent("OnGameResume", new GameResumeHandler());
    }

    private void OnDestroy() => CustomEventSystem.Clear();
}

public enum CustomTriggerType
{
    OnGameLose,
    OnGameWon,
    OnInit,
    OnGameEnd,
    OnGamePause,
    OnGameResume,
}

public class BaseHandler
{
    public CustomTriggerType TriggerType;
}

public class GameLoseHandler : BaseHandler
{
    public GameLoseHandler()
    {
        TriggerType = CustomTriggerType.OnGameLose;

        Events.GameLoseAnimationEnd.AddListener(() => CustomEventSystem.TryInvoke("OnGameLose"));
    }
}

public class GameWonHandler : BaseHandler
{
    public GameWonHandler()
    {
        TriggerType = CustomTriggerType.OnGameWon;

        Events.GameWon.AddListener(() => CustomEventSystem.TryInvoke("OnGameWon"));
    }
}

public class InitHandler : BaseHandler
{
    public InitHandler()
    {
        TriggerType = CustomTriggerType.OnInit;

        Events.OnInit.AddListener(() => CustomEventSystem.TryInvoke("OnInit"));
    }
}

public class GameEndHandler : BaseHandler
{
    public GameEndHandler()
    {
        TriggerType = CustomTriggerType.OnGameEnd;

        Events.GameLoseAnimationEnd.AddListener(() => CustomEventSystem.TryInvoke("OnGameEnd"));
        Events.GameWon.AddListener(() => CustomEventSystem.TryInvoke("OnGameEnd"));
    }
}

public class GamePauseHandler : BaseHandler
{
    public GamePauseHandler()
    {
        TriggerType = CustomTriggerType.OnGamePause;

        Events.GamePause.Subscribe(OnPause);
    }

    private void OnPause(bool pause) { if (pause) CustomEventSystem.TryInvoke("OnGamePause"); }
}

public class GameResumeHandler : BaseHandler
{
    public GameResumeHandler()
    {
        TriggerType = CustomTriggerType.OnGameResume;

        Events.GamePause.Subscribe(OnResume);
    }

    private void OnResume(bool pause) { if (!pause) CustomEventSystem.TryInvoke("OnGameResume"); }
}

public interface IBaseCustomEventHandler { }

public interface IGameLoseHandler : IBaseCustomEventHandler
{
    void OnGameLose(BaseHandler handler);
}

public interface IGameWonHandler : IBaseCustomEventHandler
{
    void OnGameWon(BaseHandler handler);
}

public interface IInitHandler : IBaseCustomEventHandler
{
    void OnInit(BaseHandler handler);
}

public interface IGameEndHandler : IBaseCustomEventHandler
{
    void OnGameEnd(BaseHandler handler);
}

public interface IGamePauseHandler : IBaseCustomEventHandler
{
    void OnPause(BaseHandler handler);
}

public interface IGameResumeHandler : IBaseCustomEventHandler
{
    void OnResume(BaseHandler handler);
}