using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Figure
{
    public IReadOnlyCollection<Block> Blocks { get => blocks.AsReadOnly(); }

    private List<Block> blocks = new List<Block>();
    private SceneData sceneData;

    public Figure(BlockState[] blocksStates)
    {
        if (ToolBox.GetData(out sceneData))
            CreateBlocks(blocksStates);
    }

    public BlockState[] GetBlockStates()
    {
        BlockState[] states = new BlockState[blocks.Count];

        for (int i = 0; i < blocks.Count; i++)
            states[i] = BlockState.ToBlockState(blocks[i]);

        return states;
    }

    public Vector3 GetCenter()
    {
        Vector3 under = blocks.Last().transform.position;
        under.y -= 0.5f;
        under.y += blocks.Count() / 2f;

        return under;
    }

    public void Destroy()
    {
        foreach (Block block in blocks)
            GameObject.Destroy(block.gameObject);
    }

    private void CreateBlocks(BlockState[] blocksStates)
    {
        for (int i = 0; i < blocksStates.Length; i++)
            blocks.Add(Block.CreateBlock(
                sceneData.BlockPrefab, 
                blocksStates[i].X, 
                blocksStates[i].Y, 
                blocksStates[i].IsTarget,
                blocksStates[i].IsLock,
                blocksStates[i].BlockColor, 
                sceneData.BlockParent
            ));

        if (ToolBox.GetManagersInterface(out IGameField gameField))
            gameField.TryPlacingBlocks(blocks.ToArray());
    }

    public void Move(int x, int y)
    {
        if (ToolBox.GetManagersInterface(out IGameField gameField))
            gameField.TryMoveBlocks(blocks.ToArray(), x, y);
    }

    public void UpdateGameField()
    {
        if (ToolBox.GetManagersInterface(out IGameField gameField))
            gameField.UpdateGameField(blocks.ToArray());
    }

    public void Rotate()
    {
        if (ToolBox.GetManagersInterface(out IGameField gameField))
        {
            blocks.Sort((a, b) => a.Y > b.Y ? -1 : 1);
            gameField.TryMoveBlocks(blocks.Skip(blocks.Count - 1).ToArray(), 0, blocks.Count - 1);
            gameField.TryMoveBlocks(blocks.Take(blocks.Count - 1).ToArray(), 0, -1);
        }
    }
}