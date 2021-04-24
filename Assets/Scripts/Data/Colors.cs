using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "Color", menuName = "Color", order = 1)]
public class Colors : ScriptableObject
{
    public BlockColor[] PossibleColors => possibleColors;

    [SerializeField] private BlockColor[] possibleColors;

    public Color GetColorById(ColorId id)
    {
        foreach (BlockColor blockColor in PossibleColors)
        {
            if (blockColor.ColorId == id)
                return blockColor.Color;
        }

        return default;
    }

    public BlockColor ToPossibleColor(BlockColor blockColor)
    {
        return new BlockColor(GetColorById(blockColor.ColorId), blockColor.ColorId);
    }
}