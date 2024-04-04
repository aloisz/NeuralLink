using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

public class CustomCarController : MonoBehaviour
{
    [SerializeField] private AnimationCurve forceBySpeed;
    
    public float horizontalInput;
    public float verticalInput;
    public bool isDrifting;
    
    [SerializeField] private Transform centerOfMass;

    private float steering;
    [SerializeField] private float maxSteering = 45;
    [SerializeField] private float steeringLerp = 20;
    [SerializeField] private float driftingForce = 4;

    [SerializeField] private float torqueStrengh = 0.001f;
    [SerializeField] private float torqueStrenghDrifting = 0.001f;
    [SerializeField] private AnimationCurve antiTorque;
    [SerializeField] private AnimationCurve torqueByVelocity;
    [SerializeField] private AnimationCurve driftBySpeed;
    [SerializeField] private AnimationCurve suspensionCurve;

    // Suspension
    [SerializeField] private Transform[] suspensionPoints;
    [SerializeField] private float suspensionLenght = 1;
    [SerializeField] private Transform[] wheels;
    [SerializeField] private LayerMask suspensionMask;
    [SerializeField] private float wheelRadius;
    [SerializeField] private float suspensionStabilisator = 10;
    [SerializeField] private float suspensionForceApplied = 1;
        
    private Vector3 localvelocity;
    private Vector3 steeringDirection;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.localPosition;
    }
    
    void Update()
    {
        GetLocalVelocity();
        SetDirection();
        RotateVehicule();
        GoForward();
        CancelDrift();
        ApplySuspension();
        
        ShowDirection();
    }

    private void GetLocalVelocity()
    {
        localvelocity = transform.InverseTransformVector(rb.velocity);
    }
    
    private void SetDirection()
    {
        steering = math.lerp(steering, horizontalInput * maxSteering, steeringLerp * Time.deltaTime);
        steeringDirection = Quaternion.AngleAxis(steering, Vector3.up) * transform.forward;
    }
    
    private void RotateVehicule()
    {
        if(isDrifting)rb.AddTorque(0,steering * torqueStrenghDrifting * torqueByVelocity.Evaluate(localvelocity.z),0);
        else rb.AddTorque(0,steering * torqueStrengh * torqueByVelocity.Evaluate(localvelocity.z),0);
        rb.AddTorque(0,-rb.angularVelocity.y * antiTorque.Evaluate(math.abs(steering)),0);
    }
    
    private void GoForward()
    {
        if(!isGrounded)return;
        rb.AddForce(transform.forward * (verticalInput * forceBySpeed.Evaluate(localvelocity.z)));
    }

    private Vector3 antiDriftForce;
    private void CancelDrift()
    {
        if (isDrifting)
        {
            antiDriftForce = (Vector3.Dot(Vector3.Cross(transform.forward, Vector3.up), rb.velocity) *
                             (-Vector3.Cross(transform.forward, Vector3.up))) / driftingForce;
            rb.AddForce(antiDriftForce * driftBySpeed.Evaluate(localvelocity.z ));
        }
        else
        {
            antiDriftForce = Vector3.Dot(Vector3.Cross(transform.forward, Vector3.up), rb.velocity) *
                             (-Vector3.Cross(transform.forward, Vector3.up));
            rb.AddForce(antiDriftForce * driftBySpeed.Evaluate(localvelocity.z));
        }
    }

    private bool isGrounded;
    private void ApplySuspension()
    {
        isGrounded = false;

        for (int i = 0; i < suspensionPoints.Length; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(suspensionPoints[i].position, -transform.up, out hit, suspensionLenght, suspensionMask))
            {
                Debug.DrawRay(suspensionPoints[i].position, -transform.up * suspensionLenght, Color.red);
                rb.AddForceAtPosition(
                    Vector3.up * (suspensionForceApplied * suspensionCurve.Evaluate((suspensionLenght - hit.distance) / suspensionLenght)), 
                    suspensionPoints[i].position);
                    
                wheels[i].localPosition = suspensionPoints[i].localPosition - Vector3.up * (hit.distance - wheelRadius);
                isGrounded = true;
                
            }
        }

        if (isGrounded)
        {
            if (rb.velocity.y > 0)
            {
                rb.AddForce(-Vector3.up * rb.velocity.y / suspensionStabilisator);
            }
        }
    }

    private void ShowDirection()
    {
        Debug.DrawRay(transform.position, steeringDirection * 10, Color.red);
        Debug.DrawRay(transform.position, rb.velocity * 10, Color.blue);
        Debug.DrawRay(transform.position, antiDriftForce * driftBySpeed.Evaluate(localvelocity.z), Color.magenta);
    }
}
