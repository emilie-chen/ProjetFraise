using System.Runtime.CompilerServices;
using Unity.Mathematics;
using static Misc;
using UnityEngine;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;

[RequireComponent(typeof(GravityData))]
public sealed class PlayerController : MonoBehaviour
{
    private GravityData m_GravityData;

    private struct Facing
    {
        public float3 up;
        public float3 forward;
        public float3 right;
        public float3 down => -up;
        public float3 left => -right;
        public float3 back => -forward;
    }

    private void Start()
    {
        m_GravityData = GetComponent<GravityData>();
    }

    private Facing targetFacing
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var gravitation = m_GravityData.gravitationOnMe;
            var up = select(
                normalize(double3(transform.up)),
                normalize(-gravitation),
                all(isfinite(gravitation)) && !issmall(gravitation));
            var faceForward = double3(transform.forward);
            var forward = normalize(projectonplane(faceForward, up));
            var right = normalize(cross(up, forward));

            return new()
            {
                up = float3(up),
                forward = float3(forward),
                right = float3(right),
            };
        }
    }

    private void DrawFacing(in Facing facing)
    {
        var position = transform.position;
        Debug.DrawRay(position, facing.up * 5.0f, Color.green);
        Debug.DrawRay(position, facing.right * 5.0f, Color.red);
        Debug.DrawRay(position, facing.forward * 5.0f, Color.blue);
    }

    private void FixedUpdate()
    {
        var facing = targetFacing;
        DrawFacing(in facing);
        transform.up = facing.up;
    }
}
