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
            var up = normalize(-m_GravityData.gravitationOnMe);
            var faceForward = double3(transform.forward);
            var forward = normalize(projectOnPlane(faceForward, up));
            var right = normalize(cross(up, forward));

            return new()
            {
                up = float3(up),
                forward = float3(forward),
                right = float3(right)
            };
        }
    }

    private void FixedUpdate()
    {
        var facing = targetFacing;
        transform.up = facing.up;
    }
}
