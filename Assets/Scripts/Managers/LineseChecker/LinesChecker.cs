using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "LinesChecker", menuName = "Managers/LinesCheckers/LinesChecker", order = 1)]
public class LinesChecker : ScriptableObject, IManager, ILinesChecker
{
    protected SceneData sceneData;
    protected List<Line> lines = new List<Line>();
    protected bool lineDeleted;

    public void BaseAwake() 
    {
        ToolBox.GetData(out sceneData);

        OnAwake();

        Subscribes();
    }

    public void BaseStart() { }

    public void BaseUpdate() { }

    public void ClearState() { }

    public virtual Line[] GetLines(Block[,] gridArray)
    {
        lineDeleted = false;
        lines.Clear();

        CheckLines(gridArray);

        return lines.ToArray();
    }

    protected virtual void OnAwake() { }

    protected virtual void Subscribes() { }

    protected virtual void UnSubscribes() { }

    protected virtual void OnManagerDisable() { }

    protected virtual void CheckLines(Block[,] gridArray)
    {
        for (int y = 0; y < gridArray.GetLength(1) - 1; y++)
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                if (gridArray[x, y] != null && !gridArray[x, y].IsLock)
                    CheckNeighbors(x, y, gridArray);
            }
        }
    }

    protected virtual void CheckNeighbors(int x, int y, Block[,] gridArray)
    {
        List<Block> validBlocks = new List<Block>();
        ColorId myColor = gridArray[x, y].BlockColor.ColorId;

        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].BlocksInLine.Contains(gridArray[x, y]))
                return;
        }

        CheckWay(x, y, 0, 1, myColor, gridArray, ref validBlocks, LineOrientation.straight);
        CheckWay(x, y, 1, 0, myColor, gridArray, ref validBlocks, LineOrientation.straight);
        CheckWay(x, y, 1, 1, myColor, gridArray, ref validBlocks, LineOrientation.sloping);
        CheckWay(x, y, -1, 1, myColor, gridArray, ref validBlocks, LineOrientation.sloping);
    }

    protected virtual void CheckWay(int x, int y, int directionX, int directionY, ColorId myColor, Block[,] gridArray, ref List<Block> validBlocks, LineOrientation orientation)
    {
        validBlocks.Clear();
        validBlocks.Add(gridArray[x, y]);

        Check(x, y, directionX, directionY, myColor, gridArray, ref validBlocks);
        Check(x, y, -directionX, -directionY, myColor, gridArray, ref validBlocks);

        CheckValid(validBlocks, orientation);
    }

    protected virtual void CheckValid(List<Block> validBlocks, LineOrientation orientation)
    {
        if (validBlocks.Count >= 3)
        {
            Line newLine = new Line(validBlocks.ToArray(), orientation);

            if (!TryConcatLines(newLine))
                lines.Add(newLine);
        }
    }

    protected virtual bool TryConcatLines(Line newLine)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Intersect(newLine))
            {
                lines.Add(newLine + lines[i]);
                lines.Remove(lines[i]);

                return true;
            }
        }

        return false;
    }

    protected virtual void Check(int x, int y, int directionX, int directionY, ColorId myColor, Block[,] gridArray, ref List<Block> validBlocks)
    {
        int newX = x + directionX;
        int newY = y + directionY;

        if (newX >= 0 && newX < sceneData.Width && newY >= 0 && newY < sceneData.Height && gridArray[newX, newY] != null && !gridArray[newX, newY].IsLock)
        {
            if (gridArray[newX, newY].BlockColor.ColorId == myColor)
            {
                validBlocks.Add(gridArray[newX, newY]);
                Check(newX, newY, directionX, directionY, myColor, gridArray, ref validBlocks);
            }
        }
    }

    private void OnDisable() 
    {
        OnManagerDisable();

        UnSubscribes(); 
    }
}
