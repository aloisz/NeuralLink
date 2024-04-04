using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private float maxSteerAngle = 42;
    [SerializeField] private float motorForce = 800;
    
    
    [SerializeField] private WheelCollider wheelFrontLeftCollider,
        wheelFrontRightCollider,
        wheelBackLeftCollider,
        wheelBackRightCollider;
    
    [SerializeField] private Transform wheelFrontLeftTransform,
        wheelFrontRightTransform,
        wheelBackLeftTransform,
        wheelBackRightTransform;

    private Rigidbody rb;
    [SerializeField] private Transform centerOfMass;
    
    public float horizontalInput;
    public float verticalInput;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.localPosition;
    }
    
    private void Update()
    {
        UpdateWheelsModel();
    }

    private void UpdateWheelsModel()
    {
        UpdateWheelModel(wheelFrontLeftCollider, wheelFrontLeftTransform);
        UpdateWheelModel(wheelFrontRightCollider, wheelFrontRightTransform);
        
        UpdateWheelModel(wheelBackLeftCollider, wheelBackLeftTransform);
        UpdateWheelModel(wheelBackRightCollider, wheelBackRightTransform);
    }

    private Vector3 pos;
    private Quaternion rot;
    private void UpdateWheelModel(WheelCollider col, Transform tr)
    {
        col.GetWorldPose(out pos, out rot);

        tr.position = pos;
        tr.rotation = rot;
    }

    public void Reset()
    {
        horizontalInput = 0;
        verticalInput = 0;
    }


    private void FixedUpdate()
    {
        Steering();
        Accelerate();
    }

    private void Steering()
    {
        wheelFrontLeftCollider.steerAngle = horizontalInput * maxSteerAngle;
        wheelFrontRightCollider.steerAngle = horizontalInput * maxSteerAngle;
    }

    private void Accelerate()
    {
        //Front wheel
        /*wheelFrontLeftCollider.motorTorque = verticalInput * motorForce;
        wheelFrontRightCollider.motorTorque = verticalInput * motorForce;*/
        
        //Back wheel
        wheelBackLeftCollider.motorTorque = verticalInput * motorForce;
        wheelBackRightCollider.motorTorque = verticalInput * motorForce;
    }
}
