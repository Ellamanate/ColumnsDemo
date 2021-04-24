[System.Serializable]
public struct BlockState
{
    public int X;
    public int Y;
    public BlockColor BlockColor;
    public bool IsTarget;
    public bool IsLock;

    public BlockState(int x, int y, BlockColor blockColor, bool isTarget, bool isLock)
    {
        X = x;
        Y = y;
        BlockColor = blockColor;
        IsTarget = isTarget;
        IsLock = isLock;
    }

    public static BlockState ToBlockState(Block block)
    {
        if (block != null) return new BlockState(block.X, block.Y, block.BlockColor, block.IsTarget, block.IsLock);
        else return default;
    }
}