using System.Runtime.CompilerServices;
using static Unity.Mathematics.math;
using static GravityData;
using Unity.Mathematics;
using UnityEngine;

public static class Misc
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float3 projectonplane(float3 vector, float3 planeNormal)
    {
        var num1 = dot(planeNormal, planeNormal);
        if (num1 < (double) EPSILON)
            return vector;
        var num2 = dot(vector, planeNormal);
        return float3(vector.x - planeNormal.x * num2 / num1,
            vector.y - planeNormal.y * num2 / num1,
            vector.z - planeNormal.z * num2 / num1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double3 projectonplane(double3 vector, double3 planeNormal)
    {
        var num1 = dot(planeNormal, planeNormal);
        if (num1 < EPSILON_DBL)
            return vector;
        var num2 = dot(vector, planeNormal);
        return double3(vector.x - planeNormal.x * num2 / num1,
            vector.y - planeNormal.y * num2 / num1,
            vector.z - planeNormal.z * num2 / num1);
    }

    public const float SMALL_THRESHOLD = EPSILON * 100.0f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool issmall(float3 value) => length(value) < SMALL_THRESHOLD;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool issmall(double3 value) => length(value) < SMALL_THRESHOLD;

    public static void PutIntoOrbit(GameObject center, GameObject satellite, float r)
    {
        var centerRb = center.GetComponent<Rigidbody>();
        var satRb = satellite.GetComponent<Rigidbody>();
        var M = centerRb.mass;
        var v = (float) sqrt(G * M / r);
        float3 centerPos = center.transform.position;
        var centerToSat = normalize(float3(satellite.transform.position) - centerPos);
        var newSatPos = centerPos + centerToSat * r;
        satellite.transform.position = newSatPos;
        satRb.velocity = cross(up(), centerToSat) * v;
    }
}