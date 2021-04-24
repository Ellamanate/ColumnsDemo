using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


[CreateAssetMenu(fileName = "DefaultGameField", menuName = "Managers/GameFields/Default", order = 1)]
public class GameField : ScriptableObject, IManager, IGameField
{
    protected SceneData sceneData;
    protected FieldState state;
    protected Block[,] gridArray;
    protected List<Block> allBlocks = new List<Block>();
    protected List<AudioClip> audioClips = new List<AudioClip>();
    protected int width;
    protected int height;
    protected int combo = 0;
    protected bool isLoaded = true;

    public void BaseAwake()
    {
        if (ToolBox.GetData(out sceneData))
        {
            if (sceneData.GameState != null)
                state = Serialyzer.RegisterState("FieldState", Instantiate(sceneData.GameState).FieldState);
            else
                state = Serialyzer.RegisterState<FieldState>("FieldState");

            width = sceneData.Width;
            height = sceneData.Height;

            gridArray = new Block[width, height];

            foreach (AudioClip clip in Resources.LoadAll("Sound/Combo", typeof(AudioClip)))
                audioClips.Add(clip);

            OnAwake();

            Subscribes();
        }
    }

    public void BaseStart() 
    {
        Block[] createdBlocks = new Block[state.GroundedBlocks.Count];

        for (int i = 0; i < state.GroundedBlocks.Count; i++)
            createdBlocks[i] = Block.CreateBlock(
                sceneData.BlockPrefab, 
                state.GroundedBlocks[i].X, 
                state.GroundedBlocks[i].Y, 
                state.GroundedBlocks[i].IsTarget,
                state.GroundedBlocks[i].IsLock,
                state.GroundedBlocks[i].BlockColor, 
                sceneData.BlockParent
            );

        if (state.GroundedBlocks.Count != 0 && TryPlacingBlocks(createdBlocks))
            UpdateGameField(createdBlocks);
        else
            Events.OnGrounded.Invoke();

        OnStart();
    }

    public void BaseUpdate() { }

    public void ClearState()
    {
        foreach (Block block in allBlocks)
            GameObject.Destroy(block.gameObject);
    }

    public virtual Block[,] GetGridClone() => (Block[,])gridArray.Clone();

    public virtual bool TryPlacingBlocks(Block[] blocks)
    {
        if (IsCellsValid(blocks, 0, 0))
        {
            for (int i = 0; i < blocks.Length; i++)
                allBlocks.Add(blocks[i]);

            return true;
        }

        return false;
    }

    public virtual void UpdateGameField(Block[] blocks)
    {
        if (CheckGrounded(blocks))
        {
            for (int i = 0; i < blocks.Length; i++)
                gridArray[blocks[i].X, blocks[i].Y] = blocks[i];

            Events.OnLockFigure.Invoke();
            ToolBox.EnableCoroutine(this, UpdateColumns());
        }
    }

    public virtual void TryMoveBlocks(Block[] blocks, int directionX, int directionY)
    {
        if (IsBlocksValid(blocks))
        {
            if (IsCellsValid(blocks, directionX, directionY))
                MoveBlocks(blocks, directionX, directionY);
        }
    }

    private void OnDisable()
    {
        OnManagerDisable();

        UnSubscribes();
    }

    protected virtual void OnAwake() { }

    protected virtual void OnStart() { }

    protected virtual void Subscribes() 
    {
        state.SerialyzingCallback.AddListener(Serialyzing);
        Events.GameLose.AddListener(StartLoseAnimation);
    }

    protected virtual void UnSubscribes() 
    {
        if (state != null)
            state.SerialyzingCallback.RemoveListener(Serialyzing);

        Events.GameLose.RemoveListener(StartLoseAnimation);
    }

    protected virtual void OnManagerDisable() { }

    protected void Serialyzing()
    {
        state.GroundedBlocks.Clear();

        foreach (Block block in gridArray)
        {
            if (block != null)
                state.GroundedBlocks.Add(BlockState.ToBlockState(block));
        }
    }

    protected virtual void MoveBlocks(Block[] blocks, int directionX, int directionY)
    {
        for (int i = 0; i < blocks.Length; i++)
            blocks[i].Move(blocks[i].X + directionX, blocks[i].Y + directionY);
    }

    protected virtual bool IsBlocksValid(Block[] blocks)
    {
        if (blocks != null && blocks.Length != 0)
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                if (!allBlocks.Contains(blocks[i]))
                    throw new Exception("Received blocks does not exist in this game field");

                if (blocks[i] == null)
                    throw new Exception("Some of received blocks is null");
            }

            return true;
        }

        return false;
    }

    protected virtual bool IsCellsValid(Block[] blocks, int directionX, int directionY)
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            if (!IsCellValid(blocks[i].X + directionX, blocks[i].Y + directionY))
                return false;
        }

        return true;
    }

    protected virtual bool IsCellValid(int x, int y)
    {
        if (TryGetBlock(x, y, out Block block))
        {
            if (block != null)
                return false;
        }
        else return false;

        return true;
    }

    protected virtual bool CheckGrounded(Block[] blocks)
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i].Y == 0)
                return true;

            if (TryGetBlock(blocks[i].X, blocks[i].Y - 1, out Block block))
            {
                if (block != null)
                    return true;
            }
        }

        return false;
    }

    protected virtual void CheckEndGame()
    {
        int y = sceneData.Height - 1 - sceneData.MaxFigureHeight;

        for (int x = 0; x < sceneData.Width; x++)
        {
            if (gridArray[x, y] != null)
            {
                Events.GameLose.Invoke();

                return;
            }
        }
    }

    protected virtual IEnumerator UpdateColumns()
    {
        yield return Grounding(0.1f);

        if (ToolBox.GetManagersInterface(out ILinesChecker linesChecker))
            yield return DeleteColumns(linesChecker);

        CheckEndGame();

        isLoaded = false;

        Events.OnGrounded.Invoke();
    }

    protected virtual IEnumerator DeleteColumns(ILinesChecker linesChecker)
    {
        Line[] lines = linesChecker.GetLines(gridArray);

        if (lines.Length > 0)
        {
            combo++;

            Delete(lines);

            yield return new WaitForSeconds(sceneData.BlockPrefab.ClipDuration);

            yield return Grounding(0.1f);

            yield return DeleteColumns(linesChecker);
        }

        combo = 0;
    }

    protected virtual void Delete(Line[] lines)
    {
        Events.OnDeleteBlocks.Publish(lines, combo);

        Sound.PlayClip(audioClips[UnityEngine.Random.Range(0, audioClips.Count)]);

        foreach (Line line in lines)
        {
            for (int i = 0; i < line.BlocksInLine.Count; i++)
            {
                gridArray[line.BlocksInLine[i].X, line.BlocksInLine[i].Y] = null;
                allBlocks.Remove(line.BlocksInLine[i]);
                Block.RemoveBlock(line.BlocksInLine[i]);
            }
        }
    }

    protected virtual IEnumerator Grounding(float delay)
    {
        bool isGrounded = false;

        for (int x = 0; x < width; x++)
        {
            for (int y = 1; y < height - sceneData.MaxFigureHeight; y++) 
            { 
                if (gridArray[x, y] != null)
                {
                    if (gridArray[x, y - 1] == null)
                    {
                        gridArray[x, y - 1] = gridArray[x, y];
                        gridArray[x, y].Move(x, y - 1);
                        gridArray[x, y] = null;

                        isGrounded = true;
                    }
                }
            }
        }

        yield return new WaitForSeconds(delay);

        if (isGrounded)
            yield return Grounding(delay);
    }

    protected virtual bool TryGetBlock(int x, int y, out Block block)
    {
        if (x < width && x >= 0 && y >= 0 && y < height)
        {
            block = gridArray[x, y];
            return true;
        }
        else
        {
            block = null;
            return false;
        }
    }

    private void StartLoseAnimation() => ToolBox.EnableCoroutine(this, LoseAnimation());

    protected virtual IEnumerator LoseAnimation()
    {
        yield return DeleteLines();

        Events.GameLoseAnimationEnd.Invoke();
    }

    protected virtual IEnumerator DeleteLines()
    {
        for (int x = 0; x < sceneData.Width; x++)
        {
            if (TryGetBlock(x, 0, out Block block) && block != null)
            {
                allBlocks.Remove(block);
                Block.RemoveBlock(block, 5f);
                gridArray[x, 0] = null;
            }
        }

        yield return new WaitForSeconds(sceneData.BlockPrefab.ClipDuration / 5f);

        yield return Grounding(0);

        if (!LineIsEmpty(0))
            yield return DeleteLines();
    }

    protected bool LineIsEmpty(int line)
    {
        for (int x = 0; x < width; x++)
        {
            if (gridArray[x, line] != null)
                return false;
        }

        return true;
    }
}