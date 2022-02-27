using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using static Unity.Mathematics.math;
using static Misc;
using UnityEngine;
using double3 = Unity.Mathematics.double3;

public sealed class GravityData : MonoBehaviour
{
    private Rigidbody m_RigidBody;
    private float3 m_PreviousVelocity;

    public IEnumerable<GameObject> allOtherGravityBodies => GameObject
        .FindGameObjectsWithTag("Gravity")
        .AsEnumerable()
        .Where(go => go != gameObject);

    public const double G = 1.0f;
    public static double UniversalGravitation(double m1, double m2, double r) => G * m1 * m2 / pow(r, 2.0);

    public static double3 UniversalGravitationBetween(double4 me, double4 other) =>
        UniversalGravitation(me.w, other.w, distance(me.xyz, other.xyz)) * normalize(other.xyz - me.xyz);

    public double3 UniversalGravitationWith(GameObject other) => UniversalGravitationBetween(
        double4(double3(transform.position), m_RigidBody.mass),
        double4(double3(other.transform.position), other.GetComponent<Rigidbody>().mass));

    public double3 gravitationOnMe { get; private set; }

    public double3 calculatedGravitationOnMe =>
        allOtherGravityBodies.Aggregate(double3(0.0f), (acc, go) => acc + UniversalGravitationWith(go));

    public double3 acceleration
    {
        get
        {
            float3 currentVel = m_RigidBody.velocity;
            var acc = (currentVel - m_PreviousVelocity) / Time.fixedDeltaTime;
            m_PreviousVelocity = currentVel;
            return acc;
        }
    }

    public double3 relativisticNetForce => acceleration * m_RigidBody.mass - gravitationOnMe;

    private void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_PreviousVelocity = m_RigidBody.velocity;
        gravitationOnMe = double3.zero;
    }

    private void FixedUpdate()
    {
        gravitationOnMe = calculatedGravitationOnMe;
        m_RigidBody.AddForce(float3(gravitationOnMe));
        Debug.DrawRay(transform.position, float3(normalize(relativisticNetForce)) * 10.0f, Color.yellow);
    }

    [CustomEditor(typeof(GravityData))]
    private class EditorScript : Editor
    {
        private GameObject center, satellite;
        private float r;

        private void Awake()
        {
            center = GameObject.Find("Sun");
            satellite = GameObject.Find("Earth");
            r = 5000;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            center = (GameObject) EditorGUILayout.ObjectField("Center", center, typeof(GameObject), true);
            satellite = (GameObject) EditorGUILayout.ObjectField("Satellite", satellite, typeof(GameObject), true);
            r = EditorGUILayout.FloatField("Orbit height", r);
            if (GUILayout.Button("Orbit"))
            {
                PutIntoOrbit(center, satellite, r);
            }
        }
    }
}