using UnityEngine;

public static class UnityExtensions
{
    public static void DestroyChildrens(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}