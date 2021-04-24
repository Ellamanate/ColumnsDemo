using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Events;

public static class RecordsCollector
{
    private static Records records;

    static RecordsCollector() => records = Serialyzer.Load<Records>(typeof(Records).ToString());

    public static int CheckRecord(int score, string level)
    {
        if (records == null)
            records = new Records();

        GameResult gameResult = new GameResult(score, level, DateTime.Now);
        GameResult record = records.GetRecord(level);

        if (gameResult > record)
        {
            records.SetRecord(gameResult);
            Serialyzer.Save(records);

            return gameResult.Score;
        }
        else
        {
            return record.Score;
        }
    }

    public static GameResult GetRecord(string level)
    {
        if (records != null)
            return records.GetRecord(level);

        return default;
    }

    public static GameResult GetRecord()
    {
        if (records != null)
            return records.GetRecord();

        return default;
    }
}

[Serializable]
public class Records : IState
{
    public UnityEvent SerialyzingCallback => serialyzingCallback;

    [SerializeField] private List<GameResult> resultsList = new List<GameResult>();
    private Dictionary<string, GameResult> gameResults = new Dictionary<string, GameResult>();
    public UnityEvent serialyzingCallback = new UnityEvent();

    public GameResult GetRecord(string level)
    {
        if (gameResults.TryGetValue(level, out GameResult result))
            return result;

        return default;
    }

    public GameResult GetRecord() => gameResults.Values.OrderByDescending(item => item.Score).First();

    public void SetRecord(GameResult result)
    {
        if (gameResults.ContainsKey(result.Level))
            gameResults[result.Level] = result;
        else
            gameResults.Add(result.Level, result);
    }

    public void OnSerialyzing() => resultsList = gameResults.Values.ToList();

    public void OnDeserialyzing() => gameResults = resultsList.ToDictionary((x) => x.Level);
}

[Serializable]
public struct GameResult
{
    public int Score => score;
    public string Date => date;
    public string Level => level;

    [SerializeField] private int score;
    [SerializeField] private string date;
    [SerializeField] private string level;

    public GameResult(int score, string level, DateTime date)
    {
        this.score = score;
        this.level = level;
        this.date = date.ToString();
    }

    public static bool operator >(GameResult left, GameResult right) => left.score > right.score;

    public static bool operator <(GameResult left, GameResult right) => left.score < right.score;
}