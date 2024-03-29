using UnityEngine;

public static class IntExtensions
{
    public static bool IsBossLevel(this int level)
    {
        return level % 10 == 0;
    }

    public static int CountMobsByLevel(this int level)
    {
        return level.IsBossLevel() ? 1 : 10;
    }
}