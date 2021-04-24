using UnityEngine;
using UnityEngine.UI;


public static class Extension
{
    public static Vector3 AddZ(this Vector2 vector, float z)
    {
        return new Vector3(vector.x, vector.y, z);
    }

    public static bool ToBool(this int value)
    {
        if (value > 0)
            return true;
        else
            return false;
    }

    public static int ToInt(this bool value)
    {
        if (value)
            return 1;
        else
            return 0;
    }

    public static Color ChangeAlpha(this Color color, float newAlpha)
    {
        return new Color(color.r, color.g, color.b, newAlpha);
    }
}