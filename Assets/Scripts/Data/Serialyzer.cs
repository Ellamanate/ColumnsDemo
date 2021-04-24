using UnityEngine;
using UnityEngine.Events;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public interface IState 
{
    UnityEvent SerialyzingCallback { get; }
    void OnSerialyzing();
    void OnDeserialyzing();
}

public static class Serialyzer
{
    private static List<(string, IState)> states = new List<(string, IState)>();

    public static T RegisterState<T>(string path) where T : class, IState
    {
        if (PlayerPrefs.GetInt("CanContinue").ToBool())
        {
            T load = Load<T>(path);

            if (load != null)
            {
                states.Add((path, load));

                return load;
            }
        }

        T newState = (T)Activator.CreateInstance(typeof(T));

        states.Add((path, newState));

        return newState;
    }

    public static T RegisterState<T>(string path, T state) where T : class, IState
    {
        if (PlayerPrefs.GetInt("CanContinue").ToBool())
        {
            T load = Load<T>(path);

            if (load != null)
            {
                states.Add((path, load));

                return load;
            }
        }

        states.Add((path, state));

        return state;
    }

    public static void DropState<T>() where T : IState
    {
        T state = (T)Activator.CreateInstance(typeof(T));

        SaveLoad.Save(state, state.GetType().ToString());
    }

    public static void Save()
    {
        foreach ((string path, IState state) in states)
        {
            state.SerialyzingCallback.Invoke();
            state.OnSerialyzing();

            MethodInfo castMethod = typeof(Serialyzer).GetMethod("Cast").MakeGenericMethod(state.GetType());
            object castedObject = castMethod.Invoke(null, new object[] { state });

            SaveLoad.Save(castedObject, path);
        }
    }

    public static void Save<T>(T obj) where T : IState
    {
        obj.SerialyzingCallback.Invoke();
        obj.OnSerialyzing();

        SaveLoad.Save(obj, obj.GetType().ToString());
    }

    public static T Load<T>(string type) where T : class, IState
    {
        T loads = (T)SaveLoad.Load<T>(type);

        if (loads != null)
            loads.OnDeserialyzing();

        return loads;
    }

    public static T Cast<T>(object obj)
    {
        return (T)obj;
    }
}

public static class SaveLoad
{
    static public void Save(object data, string path)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + path + ".gd");
        bf.Serialize(file, JsonUtility.ToJson(data));
        file.Close();
    }

    static public object Load<T>(string path)
    {
        if (File.Exists(Application.persistentDataPath + "/" + path + ".gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + path + ".gd", FileMode.Open);
            string json = (string)bf.Deserialize(file);
            file.Close();
            var saves = JsonUtility.FromJson(json, typeof(T));

            return saves;
        }

        return default;
    }
}