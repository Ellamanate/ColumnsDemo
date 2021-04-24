using System.Collections.Generic;
using System.Linq;


public enum LineOrientation
{
    straight,
    sloping,
    combined
}

public struct Line
{
    public List<Block> BlocksInLine;
    public List<Line> SubLines;
    public LineOrientation Orientation;

    public Line(Block[] blocks, LineOrientation orientation)
    {
        Orientation = orientation;
        SubLines = new List<Line>();
        BlocksInLine = new List<Block>();
        BlocksInLine.AddRange(blocks);

        SubLines.Add(this);
    }

    public bool Intersect(Line newLine)
    {
        return BlocksInLine.Intersect(newLine.BlocksInLine).ToArray().Length > 0;
    }

    public static Line operator +(Line a, Line b)
    {
        LineOrientation newOrientation;
        List<Line> subLines = new List<Line>();

        subLines.AddRange(a.SubLines);
        subLines.AddRange(b.SubLines);

        newOrientation = a.Orientation == b.Orientation ? a.Orientation : LineOrientation.combined;

        Line newLine = new Line(a.BlocksInLine.Union(b.BlocksInLine).ToArray(), newOrientation);
        SetSublines(ref newLine, subLines);

        return newLine;
    }

    private static void SetSublines(ref Line line, List<Line> subLines)
    {
        line.SubLines = subLines;
    }
}