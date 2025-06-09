using UnityEngine;

/// <summary>
/// A helper class which provides simple extension methods for testing the
/// relationship of two transforms in the heirarchu.
/// </summary>
public static class HeirarchyUtils
{
    public static bool IsChildOf(this Transform self, Transform parent)
    {
        return self.parent == parent
            || (self.parent != null && self.parent.IsChildOf(parent));
    }

    public static bool IsChildOf(this Component self, Transform parent)
        => self.transform.IsChildOf(parent);
    public static bool IsChildOf(this GameObject self, Transform parent)
        => self.transform.IsChildOf(parent);

    public static bool IsParentOf(this Transform self, Transform child)
        => child.IsChildOf(self.transform);
    public static bool IsParentOf(this Component self, Transform child)
        => child.IsChildOf(self.transform);
    public static bool IsParentOf(this GameObject self, Transform child)
        => child.IsChildOf(self.transform);
}
