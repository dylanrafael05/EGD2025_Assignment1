using System.Collections.Generic;
using Unity.Profiling;

public static class ProfilerUtil
{
    private static Dictionary<string, ProfilerMarker> markers = new();

    public static ProfilerMarker Marker(string name)
    {
        if (!markers.TryGetValue(name, out var marker))
        {
            marker = new(name);
            markers.Add(name, marker);
        }

        return marker;
    }

    public static ProfilerMarker.AutoScope Enter(string name)
    {
        return Marker(name).Auto();
    }
}
