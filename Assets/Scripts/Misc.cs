using static Unity.Mathematics.math;
using Unity.Mathematics;

public static class Misc
{
    public static float3 projectOnPlane(float3 vector, float3 planeNormal)
    {
        var num1 = dot(planeNormal, planeNormal);
        if (num1 < (double) EPSILON)
            return vector;
        var num2 = dot(vector, planeNormal);
        return float3(vector.x - planeNormal.x * num2 / num1,
            vector.y - planeNormal.y * num2 / num1,
            vector.z - planeNormal.z * num2 / num1);
    }

    public static double3 projectOnPlane(double3 vector, double3 planeNormal)
    {
        var num1 = dot(planeNormal, planeNormal);
        if (num1 < EPSILON_DBL)
            return vector;
        var num2 = dot(vector, planeNormal);
        return double3(vector.x - planeNormal.x * num2 / num1,
            vector.y - planeNormal.y * num2 / num1,
            vector.z - planeNormal.z * num2 / num1);
    }
}