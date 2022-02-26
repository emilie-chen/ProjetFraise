using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using static Misc;
using UnityEngine;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;

[RequireComponent(typeof(GravityData))]
public sealed class PlayerController : MonoBehaviour
{
    private GravityData m_GravityData;
    private InputActions inputActions;
    private Rigidbody rb ;
    private struct Facing
    {
        public float3 up;
        public float3 forward;
        public float3 right;
        public float3 down => -up;
        public float3 left => -right;
        public float3 back => -forward;
    }
    
    public float lookSensitivity = 50f;

    private float pitch = 0f;
    private float yaw = 0f;

    private float roll = 0f;
    public GameObject cameraSubObject;
    // movement code
    public float maximumPlayerMovementVelocity = 2f;


    private void Start()
    {
        m_GravityData = GetComponent<GravityData>();
        cameraSubObject = GameObject.Find("Main Camera");
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        inputActions ??= new InputActions();
        inputActions.Enable();
    }

    private Facing targetFacing
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var netForce = m_GravityData.relativisticNetForce;
            var up = select(
                normalize(double3(transform.up)),
                normalize(netForce),
                all(isfinite(netForce)) && !issmall(netForce));
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
        
        //Look code
        //Rotating camera with look axis
        var lookDelta = inputActions.Player.Look.ReadValue<Vector2>();
        //if there is any new look
        
        if (lookDelta.magnitude != 0)
        {
            lookDelta *= lookSensitivity * Time.deltaTime;

            pitch -= lookDelta.y;
            yaw += lookDelta.x;

            pitch = Mathf.Clamp(pitch, -89f, 89f);

            cameraSubObject.transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
        }
        
        var moveDelta = inputActions.Player.Move.ReadValue<Vector2>();
        
        float3 moveDelta3 = (moveDelta.x * facing.forward + moveDelta.y * facing.right);
        rb.AddForce(moveDelta3*maximumPlayerMovementVelocity);

    }
}
