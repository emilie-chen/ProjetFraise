using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Timeline.Actions;
using static Unity.Mathematics.math;
using UnityEngine;
using UnityEngine.Jobs;
using float3 = Unity.Mathematics.float3;

public sealed class GravityData : MonoBehaviour
{
    public IEnumerable<GameObject> allOtherGravityBodies => GameObject
        .FindGameObjectsWithTag("Gravity")
        .AsEnumerable()
        .Where(go => go != gameObject);

    public const double G = 6.67408e-2;
    public static double UniversalGravitation(double m1, double m2, double r) => G * m1 * m2 / pow(r, 2.0);

    public static double3 UniversalGravitationBetween(double4 me, double4 other) =>
        UniversalGravitation(me.w, other.w, distance(me.xyz, other.xyz)) * normalize(other.xyz - me.xyz);

    public double3 UniversalGravitationWith(GameObject other) => UniversalGravitationBetween(
        double4(double3(transform.position), GetComponent<Rigidbody>().mass),
        double4(double3(other.transform.position), other.GetComponent<Rigidbody>().mass));

    public double3 gravitationOnMe =>
        allOtherGravityBodies.Aggregate(double3(0.0f), (acc, go) => acc + UniversalGravitationWith(go));

    private void FixedUpdate()
    {
        var rb = GetComponent<Rigidbody>();
        rb.AddForce(float3(gravitationOnMe));
    }
}