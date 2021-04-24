using UnityEngine;


[System.Serializable]
public enum ColorId
{
    red,
    green,
    blue,
    yellow,
    white,
}

public interface IBlockColor
{
    ColorId ColorId { get; }
    Color Color { get; }
}

[System.Serializable]
public struct BlockColor 
{
    public Color Color => color;
    public ColorId ColorId => colorId;

    [SerializeField] private Color color;
    [SerializeField] private ColorId colorId;

    public BlockColor(IBlockColor blockColor)
    {
        color = blockColor.Color;
        colorId = blockColor.ColorId;
    }

    public BlockColor(Color color, ColorId colorId)
    {
        this.color = color;
        this.colorId = colorId;
    }
}