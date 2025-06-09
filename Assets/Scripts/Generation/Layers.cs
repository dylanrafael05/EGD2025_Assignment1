using UnityEngine;

public static class Layers
{
    public static int InteractableMask { get; } = LayerMask.GetMask("Interactable");
    public static int GroundMask { get; } = LayerMask.GetMask("Ground");
    public static int IgnoreRaycastMask { get; } = LayerMask.GetMask("Ignore Raycast");
}
