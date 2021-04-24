using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "DefaultFigureSpawner", menuName = "Managers/FigureSpawners/Default", order = 1)]
public class FigureSpawner : ScriptableObject, IManager, IFigureSpawner
{
    protected SceneData sceneData;
    protected SpawnerState state;
    protected Figure currentFigure;
    protected AudioClip slideClip;
    protected float tickDelay;
    protected bool blockControl;
    protected bool acceleration;
    protected bool isLoad = true;
    protected bool endGame = false;

    public void BaseAwake()
    {
        if (ToolBox.GetData(out sceneData))
        {
            if (sceneData.GameState != null)
                state = Serialyzer.RegisterState("SpawnerState", Instantiate(sceneData.GameState).SpawnerState);
            else
                state = Serialyzer.RegisterState<SpawnerState>("SpawnerState");

            Events.NextFigureReady.Publish(state.NextBlockStates);

            slideClip = (AudioClip)Resources.Load("Sound/Slide/Slide", typeof(AudioClip));

            OnAwake();

            Subscribes();
        }
    }

    public void BaseStart()
    {
        CalculateTickDelay();

        ToolBox.EnableCoroutine(this, Tick());
    }

    public void BaseUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))
            OnLeftButtonDown();
        else if (Input.GetKeyDown(KeyCode.D))
            OnRightButtonDown();
        else if (Input.GetKeyDown(KeyCode.Space))
            OnShakeButtonDown();
        else if (Input.GetKeyDown(KeyCode.S))
            OnUnderButtonDown();
        else if (Input.GetKeyUp(KeyCode.S))
            OnUnderButtonUp();
    }

    public void ClearState()
    {
        if (currentFigure != null)
            currentFigure.Destroy();
    }

    public static BlockState[] CreateBlockStates()
    {
        if (ToolBox.GetData(out SceneData sceneData))
        {
            BlockColor[] possibleColors = sceneData.Colors.PossibleColors;

            int x = sceneData.Width / 2;

            return new BlockState[3] { new BlockState(x, sceneData.Height - 1, possibleColors[Random.Range(0, possibleColors.Length)], false, false),
                                       new BlockState(x, sceneData.Height - 2, possibleColors[Random.Range(0, possibleColors.Length)], false, false),
                                       new BlockState(x, sceneData.Height - 3, possibleColors[Random.Range(0, possibleColors.Length)], false, false) };
        }

        return null;
    }

    private void OnDisable()
    {
        OnManagerDisable();

        UnSubscribes();
    }

    protected virtual void OnAwake() { }

    protected virtual void Subscribes()
    {
        sceneData.LeftButton.OnDown.AddListener(OnLeftButtonDown);
        sceneData.RightButton.OnDown.AddListener(OnRightButtonDown);
        sceneData.DownButton.OnDown.AddListener(OnUnderButtonDown);
        sceneData.DownButton.OnUp.AddListener(OnUnderButtonUp);
        sceneData.ShakeButton.OnDown.AddListener(OnShakeButtonDown);

        state.SerialyzingCallback.AddListener(Serialyzing);

        Events.OnLevelChange.AddListener(CalculateTickDelay);
        Events.OnGrounded.AddListener(OnGrounded);
        Events.OnLockFigure.AddListener(OnLockFigure);
        Events.GameLose.AddListener(EndGame);
        Events.GamePause.Subscribe(Pause);
    }

    protected virtual void UnSubscribes()
    {
        if (sceneData != null)
        {
            sceneData.LeftButton.OnDown.RemoveListener(OnLeftButtonDown);
            sceneData.RightButton.OnDown.RemoveListener(OnRightButtonDown);
            sceneData.DownButton.OnDown.RemoveListener(OnUnderButtonDown);
            sceneData.DownButton.OnUp.RemoveListener(OnUnderButtonUp);
            sceneData.ShakeButton.OnDown.RemoveListener(OnShakeButtonDown);
        }

        if (state != null)
            state.SerialyzingCallback.RemoveListener(Serialyzing);

        Events.OnLevelChange.RemoveListener(CalculateTickDelay);
        Events.OnGrounded.RemoveListener(OnGrounded);
        Events.OnLockFigure.RemoveListener(OnLockFigure);
        Events.GameLose.RemoveListener(EndGame);
        Events.GamePause.UnSubscribe(Pause);
    }

    protected virtual void OnManagerDisable() { }

    protected void Serialyzing()
    {
        if (currentFigure != null)
            state.CurrentBlockStates = currentFigure.GetBlockStates();
        else
            state.CurrentBlockStates = null;
    }

    protected virtual void OnLeftButtonDown()
    {
        if (currentFigure != null && !endGame && !blockControl)
        {
            MoveFigure(-1, 0);
            Sound.PlayClip(slideClip);
        }
    }

    protected virtual void OnRightButtonDown()
    {
        if (currentFigure != null && !endGame && !blockControl)
        {
            MoveFigure(1, 0);
            Sound.PlayClip(slideClip);
        }
    }

    protected virtual void OnUnderButtonDown()
    {
        if (currentFigure != null && !endGame && !blockControl)
        {
            tickDelay = sceneData.MinTickDelay / 2;

            acceleration = true;
            blockControl = true;
        }
    }

    protected virtual void OnUnderButtonUp()
    {
        if (currentFigure != null && !endGame && (!blockControl || acceleration))
        {
            tickDelay = state.TickDelay;

            acceleration = false;
            blockControl = false;
        }
    }

    protected virtual void OnShakeButtonDown()
    {
        if (currentFigure != null && !endGame && !blockControl)
        {
            RotateFigure();
            Sound.PlayClip(slideClip);
        }
    }

    protected virtual void MoveFigure(int x, int y) => currentFigure.Move(x, y);

    protected virtual void RotateFigure() => currentFigure.Rotate();

    protected virtual void EndGame() => endGame = true;

    protected virtual void Pause(bool pause) => endGame = pause;

    protected virtual void OnGrounded()
    {
        if (acceleration)
            tickDelay = state.TickDelay;

        acceleration = false;
        blockControl = false;

        Spawn();
    }

    protected virtual void OnLockFigure() => currentFigure = null;

    protected virtual void Spawn()
    {
        if (isLoad) 
        {
            isLoad = false;

            if (state.CurrentBlockStates != null && state.CurrentBlockStates.Length != 0)
            {
                currentFigure = new Figure(state.CurrentBlockStates);
                Events.NextFigureReady.Publish(state.NextBlockStates);
            }
            else if (state.NextBlockStates != null && state.NextBlockStates.Length != 0)
            {
                currentFigure = new Figure(state.NextBlockStates);
                SetNextBlockStates();
            }
            else
            {
                SetNextBlockStates();
                CreateNewFigure();
            }

            return;
        }

        if (!endGame)
            CreateNewFigure();
    }

    protected virtual void CreateNewFigure(BlockState[] currentFigureStates)
    {
        currentFigure = new Figure(currentFigureStates);

        SetNextBlockStates();
    }

    protected virtual void CreateNewFigure()
    {
        currentFigure = new Figure(state.NextBlockStates);

        SetNextBlockStates();
    }

    protected virtual void SetNextBlockStates()
    {
        state.NextBlockStates = CreateBlockStates();

        Events.NextFigureReady.Publish(state.NextBlockStates);
    }

    protected virtual void CalculateTickDelay()
    {
        state.TickDelay = sceneData.StartTickDelay - sceneData.Level * sceneData.TickChangeWithLevel;
        tickDelay = state.TickDelay;
    }

    protected virtual IEnumerator Tick()
    {
        yield return new WaitForSeconds(tickDelay);

        while (true)
        {
            if (!endGame && currentFigure != null)
            {
                MoveFigure(0, -1);
                yield return new WaitForSeconds(tickDelay / 2);

                currentFigure.UpdateGameField();
            }

            yield return new WaitForSeconds(tickDelay / 2);
        }
    }
}