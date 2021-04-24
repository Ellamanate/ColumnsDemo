using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;


[AddComponentMenu("CustomEventComponent")]
public class CustomEventComponent : MonoBehaviour, IGameLoseHandler, IGameWonHandler, IInitHandler, IGamePauseHandler, IGameResumeHandler
{
    [SerializeField] private Entry entry;

    public virtual void OnGameWon(BaseHandler handler) => TryInvoke(handler);

    public virtual void OnGameLose(BaseHandler handler) => TryInvoke(handler);

    public virtual void OnInit(BaseHandler handler) => TryInvoke(handler);

    public virtual void OnPause(BaseHandler handler) => TryInvoke(handler);

    public virtual void OnResume(BaseHandler handler) => TryInvoke(handler);

    private void TryInvoke(BaseHandler handler)
    {
        if (entry.TriggerType == handler.TriggerType)
            entry.Callback.Invoke();
    }

    [System.Serializable]
    public class Entry
    {
        public CustomTriggerType TriggerType;
        public UnityEvent Callback;

        public Entry() { }
    }
}

public static class CustomEventSystem
{
    private static Dictionary<string, BaseHandler> eventsList = new Dictionary<string, BaseHandler>();

    public static void AddEvent(string key, BaseHandler handler) => eventsList.Add(key, handler);

    public static void Clear() => eventsList.Clear();

    public static void TryInvoke(string key) 
    {
        if (eventsList.ContainsKey(key))
        {
            foreach (IBaseCustomEventHandler baseEvent in GetHandlers())
                Invoke(baseEvent, eventsList[key]);
        }
    }

    private static void Invoke(IBaseCustomEventHandler baseEvent, BaseHandler currentHandler)
    {
        foreach (BaseHandler handler in eventsList.Values)
        {
            if (handler.TriggerType == currentHandler.TriggerType)
                baseEvent.GetType().GetMethods()[0].Invoke(baseEvent, new object[] { currentHandler });
        }
    }

    private static IBaseCustomEventHandler[] GetHandlers() => Resources.FindObjectsOfTypeAll<MonoBehaviour>().OfType<IBaseCustomEventHandler>().ToArray();
}