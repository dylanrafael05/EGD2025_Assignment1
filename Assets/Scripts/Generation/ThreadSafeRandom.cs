using System;
using System.Threading;

public static class ThreadSafeRandom
{
    [ThreadStatic] private static bool randomInit;
    [ThreadStatic] private static Unity.Mathematics.Random random;
    private static int randomIndex;

    public static ref Unity.Mathematics.Random Get()
    {
        if (!randomInit)
        {
            randomInit = true;

            random = Unity.Mathematics.Random.CreateFromIndex(unchecked((uint)randomIndex));
            Interlocked.Increment(ref randomIndex);
        }

        return ref random;
    }
}
