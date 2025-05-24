using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// A helper class providing extension methods for conversion between 
/// the types defined by <see cref="Unity.Mathematics"/> and the standard
/// vector types defined by <see cref="UnityEngine"/> 
/// </summary>
public static class MathConvert
{
    /// <summary>
    /// Convert this vector2 to a float2
    /// </summary>
    public static float2 tofloat2(this Vector2 self)
    {
        return self;
    }

    /// <summary>
    /// Convert this vector2 reference to a reference to a float2
    /// </summary>
    public static unsafe ref float2 asfloat2(this ref Vector2 self)
    {
        //! Safety !//
        // Vector2 and float2 have the same in-memory layout.
        fixed (Vector2* selfPtr = &self)
        {
            return ref *(float2*)selfPtr;
        }
    }

    /// <summary>
    /// Convert this vector3 to a float3
    /// </summary>
    public static float3 tofloat3(this Vector3 self)
    {
        return self;
    }

    /// <summary>
    /// Convert this vector3 reference to a reference to a float3
    /// </summary>
    public static unsafe ref float3 asfloat3(this ref Vector3 self)
    {
        //! Safety !//
        // Vector3 and float3 have the same in-memory layout.
        fixed (Vector3* selfPtr = &self)
        {
            return ref *(float3*)selfPtr;
        }
    }

    /// <summary>
    /// Convert this vector4 to a float4
    /// </summary>
    public static float4 tofloat4(this Vector4 self)
    {
        return self;
    }

    /// <summary>
    /// Convert this vector4 reference to a reference to a float4
    /// </summary>
    public static unsafe ref float4 asfloat4(this ref Vector4 self)
    {
        //! Safety !//
        // Vector3 and float3 have the same in-memory layout.
        fixed (Vector4* selfPtr = &self)
        {
            return ref *(float4*)selfPtr;
        }
    }
}
