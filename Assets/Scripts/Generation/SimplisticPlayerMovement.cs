using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// A very basic camera movement script designed to facilitate basic debugging functionality
/// for the map generation system.
/// </summary>
public class SimplisticPlayerMovement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float speed;
    [SerializeField] private float shiftSpeed;
    [SerializeField] private float rotLerpFactor;

    private Quaternion desiredRotation;

    void Awake()
    {
        desiredRotation = transform.localRotation;
    }

    void Update()
    {
        // Snap to the ground on the ray from the camera to this instance //
        var loc = transform.position;
        loc.y = GenerationUtils.StandHeightAt(loc.asfloat3().xz);
        transform.position = loc;

        // Calculate what must happen this frame //
        var dir = float2.zero;
        var rot = 0f;

        if (Input.GetKey(KeyCode.W))
            dir += math.float2(0, 1);
        if (Input.GetKey(KeyCode.A))
            dir += math.float2(-1, 0);
        if (Input.GetKey(KeyCode.S))
            dir += math.float2(0, -1);
        if (Input.GetKey(KeyCode.D))
            dir += math.float2(1, 0);

        if (Input.GetKeyDown(KeyCode.Q))
            rot -= 90;
        if (Input.GetKeyDown(KeyCode.E))
            rot += 90;

        // Get the speed to move at //
        var cspeed = speed;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            cspeed = shiftSpeed;
        }

        // Perform the movements //
        desiredRotation = Quaternion.Euler(0, rot, 0) * desiredRotation;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, desiredRotation, rotLerpFactor * Time.deltaTime);

        transform.position += cspeed * Time.deltaTime * (transform.rotation * Vector3.Normalize(new(dir.x, 0, dir.y)));
    }
}
