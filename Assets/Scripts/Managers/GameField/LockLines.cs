using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LockLines", menuName = "Managers/GameFields/LockLines", order = 3)]
public class LockLines : GameField
{
    protected TurnState turnState;
    protected AudioClip leavesClip;
    protected AudioClip hitClip;
    private int blockedIndex;

    protected override void OnAwake()
    {
        if (turnState == null) 
        {
            turnState = Serialyzer.RegisterState<TurnState>("TurnState");

            if (turnState.Counters == null || (turnState.Counters != null && turnState.Counters.Length != 1))
                turnState.SetCounters(1);
        }

        turnState.TargetTurns = 5;
        blockedIndex = turnState.Counters[0];

        leavesClip = (AudioClip)Resources.Load("Sound/Forest/Leaves", typeof(AudioClip));
        hitClip = (AudioClip)Resources.Load("Sound/Hits/Hit1", typeof(AudioClip));
    }

    protected override void OnStart()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < blockedIndex; y++)
            {
                if (TryGetBlock(x, y, out Block lockedblock) && lockedblock != null)
                    Lock(lockedblock);
            }

            for (int y = blockedIndex; y < height; y++)
            {
                if (TryGetBlock(x, y, out Block lockedblock) && lockedblock != null)
                    lockedblock.Unlock();
            }
        }
    }

    protected override void Subscribes()
    {
        base.Subscribes();

        turnState.SerialyzingCallback.AddListener(SerialyzeTurnState);
    }

    protected override void UnSubscribes()
    {
        base.UnSubscribes();

        if (turnState != null)
            turnState.SerialyzingCallback.RemoveListener(SerialyzeTurnState);
    }

    protected virtual void SerialyzeTurnState()
    {
        if (turnState.Counters.Length >= 1)
            turnState.Counters[0] = blockedIndex;
    }

    protected override IEnumerator UpdateColumns()
    {
        yield return Grounding(0.1f);

        if (ToolBox.GetManagersInterface(out ILinesChecker linesChecker))
            yield return DeleteColumns(linesChecker);

        CheckEndGame();

        TryLockLine();

        isLoaded = false;

        Events.OnGrounded.Invoke();
    }

    protected override IEnumerator DeleteColumns(ILinesChecker linesChecker)
    {
        Line[] lines = linesChecker.GetLines(gridArray);

        if (lines.Length > 0)
        {
            combo++;

            ToolBox.EnableCoroutine(TryUnlockLine(lines));

            Delete(lines);

            yield return new WaitForSeconds(sceneData.BlockPrefab.ClipDuration);

            yield return Grounding(0.1f);

            yield return DeleteColumns(linesChecker);
        }

        combo = 0;
    }

    private IEnumerator TryUnlockLine(Line[] lines)
    {
        foreach (Line line in lines)
        {
            for (int i = 0; i < line.BlocksInLine.Count; i++)
            {
                if (line.BlocksInLine[i].Y == blockedIndex && line.BlocksInLine[i].Y != 0)
                {
                    blockedIndex--;

                    turnState.TurnCounter = 0;

                    yield return Unlock();
                }
                else if (line.BlocksInLine[i].Y == 0)
                {
                    turnState.TurnCounter = 0;

                    yield return Unlock();
                }
            }
        }

        yield break;
    }

    private IEnumerator Unlock()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = blockedIndex; y < height; y++)
            {
                if (TryGetBlock(x, y, out Block block) && block != null && block.IsLock)
                {
                    block.UnlockAnimation("Lock");

                    Sound.PlayClip(hitClip);

                    yield return new WaitForSeconds(0.25f);

                    block.Unlock();
                }
            }
        }
    }

    private void TryLockLine()
    {
        if (LineIsFull(blockedIndex) && !isLoaded)
        {
            if (turnState.TurnCounter >= turnState.TargetTurns)
            {
                LockBlocks();

                blockedIndex++;

                turnState.TurnCounter = 0;
            }
            else if (!isLoaded)
            {
                turnState.TurnCounter++;
            }
        }
    }

    private bool LineIsFull(int line)
    {
        for (int x = 0; x < width; x++)
        {
            if (gridArray[x, line] == null)
                return false;
        }

        return true;
    }

    private void LockBlocks()
    {
        for (int x = 0; x < width; x++)
        {
            if (TryGetBlock(x, blockedIndex, out Block block) && block != null)
                Lock(block);
        }

        Sound.PlayClip(leavesClip);
    }

    private void Lock(Block block)
    {
        block.LockAnimation("Lock");
        block.Lock();
    }
}