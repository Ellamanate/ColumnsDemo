using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


[RequireComponent(typeof(SceneData))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private Text gameResult;
    [SerializeField] private Text endScore;
    [SerializeField] private Image leftButton;
    [SerializeField] private Image rightButton;
    [SerializeField] private Image downButton;
    [SerializeField] private Image shakeButton;
    private bool pause = false;
    private bool isGameEnd = false;
    private bool isWin = false;
    private bool isWinAnimation = false;

    public void OnPauseClick() => Pause();
    public void OnResumeClick() => Resume();

    public void Restart()
    {
        if (ToolBox.GetData(out SceneData sceneData))
        {
            sceneData.Restart();

            Resume();
        }
    }

    public void BackToMenu()
    {
        if (ToolBox.GetData(out SceneData sceneData))
            sceneData.Save();

        SceneManager.LoadScene("Menu");
    }

    public void MuteAudio() => Sound.Mute();

    private void Awake()
    {
        Sound.PlayMusic();

        float newAlpha = PlayerPrefs.GetFloat("Opacity");
        leftButton.color = leftButton.color.ChangeAlpha(newAlpha);
        rightButton.color = rightButton.color.ChangeAlpha(newAlpha);
        downButton.color = downButton.color.ChangeAlpha(newAlpha);
        shakeButton.color = shakeButton.color.ChangeAlpha(newAlpha);

        OnInit();
    }

    private void OnEnable()
    {
        Events.GameLose.AddListener(OnGameLose);
        Events.GameLoseAnimationEnd.AddListener(Pause);
        Events.GameWon.AddListener(OnGameWon);
        Events.OnInit.AddListener(OnInit);
    }

    private void OnDisable()
    {
        Events.GameLose.RemoveListener(OnGameLose);
        Events.GameLoseAnimationEnd.RemoveListener(Pause);
        Events.GameWon.RemoveListener(OnGameWon);
        Events.OnInit.RemoveListener(OnInit);

        SetEndGame();
    }

    private void OnApplicationPause() 
    {
        Pause();

        SetEndGame(); 
    }

    private void OnApplicationQuit() => SetEndGame();

    private void SetEndGame()
    {
        if (isGameEnd)
        {
            PlayerPrefs.SetString("LastLevel", string.Empty);
            PlayerPrefs.SetInt("CanContinue", false.ToInt());
            PlayerPrefs.Save();
        }
    }

    private void Pause()
    {
        pause = true;
        Time.timeScale = 0;

        if (isWin && ToolBox.GetManagersInterface(out IScoreCounter scoreCounter) && endScore != null)
            endScore.text = "Score: " + scoreCounter.Score + "\nBest : " + RecordsCollector.CheckRecord(scoreCounter.Score, PlayerPrefs.GetString("LastLevel"));

        Events.GamePause.Publish(pause);
    }

    private void Resume()
    {
        pause = false;
        Time.timeScale = 1;
        Events.GamePause.Publish(pause);
    }

    private void OnInit()
    {
        isGameEnd = false;
        isWin = false;

        Resume();
    }

    private void OnGameLose() => StartCoroutine(WaitWinAnimationEnd());

    private void GameLose()
    {
        if (ToolBox.GetData(out SceneData sceneData))
        {
            if (!isWin)
            {
                sceneData.PauseText.text = "Lose...";

                if (SceneManager.GetActiveScene().name != "Endless" && gameResult != null)
                    gameResult.text = sceneData.GodsName + " turned out to be stronger";
            }

            if (ToolBox.GetManagersInterface(out IScoreCounter scoreCounter) && endScore != null)
                endScore.text = "Score: " + scoreCounter.Score + "\nBest : " + RecordsCollector.CheckRecord(scoreCounter.Score, PlayerPrefs.GetString("LastLevel"));
        }

        Sound.PlayClip((AudioClip)Resources.Load("Sound/Lose", typeof(AudioClip)));

        isGameEnd = true;
    }

    private void OnGameWon()
    {
        if (ToolBox.GetData(out SceneData sceneData))
        {
            if (sceneData.CanvasAnimator != null)
            {
                sceneData.CanvasAnimator.SetTrigger("Win");
                isWinAnimation = true;
            }

            sceneData.PauseText.text = "Win!";

            if (gameResult != null)
                gameResult.text = "You beat " + sceneData.GodsName;

            if (ToolBox.GetManagersInterface(out IScoreCounter scoreCounter) && endScore != null)
                endScore.text = "Score: " + scoreCounter.Score + "\nBest : " + RecordsCollector.CheckRecord(scoreCounter.Score, PlayerPrefs.GetString("LastLevel"));
        }

        Sound.PlayClip((AudioClip)Resources.Load("Sound/Win", typeof(AudioClip)));

        int lastLevel = PlayerPrefs.GetInt("LastChallenge");
        int openLevel = PlayerPrefs.GetInt("OpenChallenge");

        if (openLevel > lastLevel)
        {
            PlayerPrefs.SetInt("LastChallenge", openLevel);
            PlayerPrefs.Save();
        }

        isGameEnd = true;
        isWin = true;
    }

    private IEnumerator WaitWinAnimationEnd()
    {
        yield return new WaitWhile(() => isWinAnimation);

        GameLose();
    }
}