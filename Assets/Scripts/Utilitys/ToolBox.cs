using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;


public class ToolBox : Singleton<ToolBox>
{
    private Dictionary<Type, object> managersPool = new Dictionary<Type, object>();
    private Dictionary<Type, object> dataPool = new Dictionary<Type, object>();
    private Dictionary<Type, List<Coroutine>> coroutines = new Dictionary<Type, List<Coroutine>>();

    public static void AddManager(ScriptableObject manager)
    {
        Instance.managersPool.Add(manager.GetType(), manager);
    }

    public static bool GetManagersInterface<T>(out T intrface)
    {
        intrface = default;

        foreach (Type type in Instance.managersPool.Keys)
        {
            if (type.GetInterface(typeof(T).Name) != null)
            {
                if (Instance.managersPool.TryGetValue(type, out object resolve))
                {
                    intrface = (T)resolve;

                    return true;
                }
            }
        }

        return false;
    }

    public static void AddData(DataContainer dataContainer)
    {
        Instance.dataPool.Add(dataContainer.GetType(), dataContainer);
    }

    public static bool GetData<T>(out T data)
    {
        object resolve;
        Instance.dataPool.TryGetValue(typeof(T), out resolve);
        data = (T)resolve;

        return data != null;
    }

    public static void ClearScene()
    {
        Instance.Clear();
    }

    public static Coroutine EnableCoroutine(IEnumerator enumerator)
    {
        return Instance.StartCoroutine(enumerator);
    }

    public static Coroutine EnableCoroutine(IManager manager, IEnumerator enumerator)
    {
        if (enumerator != null && manager != null)
        {
            if (Instance.coroutines.ContainsKey(manager.GetType())) 
            {
                Coroutine newCoroutine = Instance.StartCoroutine(enumerator);
                Instance.coroutines[manager.GetType()].Add(newCoroutine);

                return newCoroutine;
            }
            else
            {
                Coroutine newCoroutine = Instance.StartCoroutine(enumerator);
                List<Coroutine> newList = new List<Coroutine>() { newCoroutine };
                Instance.coroutines.Add(manager.GetType(), newList);

                return newCoroutine;
            }
        }

        return null;
    }

    public static void DisableCoroutine(IManager manager, Coroutine stopedCoroutine)
    {
        if (stopedCoroutine != null && manager != null)
        {
            Instance.coroutines.TryGetValue(manager.GetType(), out List<Coroutine> coroutines);

            foreach (Coroutine coroutine in coroutines)
            {
                if (stopedCoroutine == coroutine)
                {
                    Instance.StopCoroutine(stopedCoroutine);
                    coroutines.Remove(stopedCoroutine);

                    return;
                }
            }
        }
    }

    public static void DisableAllCoroutines(IManager manager)
    {
        if (manager != null)
        {
            Instance.coroutines.TryGetValue(manager.GetType(), out List<Coroutine> coroutines);

            if (coroutines != null)
            {
                foreach (Coroutine coroutine in coroutines)
                    Instance.StopCoroutine(coroutine);

                coroutines.Clear();
            }
        }
    }

    private void DisableAllCoroutines()
    {
        foreach (List<Coroutine> managersCoroutines in coroutines.Values)
        {
            foreach (Coroutine coroutine in managersCoroutines) 
            {
                if (coroutine != null)
                    StopCoroutine(coroutine);
            }
        }

        coroutines.Clear();
    }

    private void Clear()
    {
        DisableAllCoroutines();

        managersPool.Clear();
        dataPool.Clear();
    }
}