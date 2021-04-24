using System.Collections.Generic;

public interface IGameField
{
    bool TryPlacingBlocks(Block[] blocks);

    void UpdateGameField(Block[] blocks);

    void TryMoveBlocks(Block[] blocks, int directionX, int directionY);

    Block[,] GetGridClone();
}