using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform target;
    [SerializeField] private Vector3 localPositionToMove = new Vector3(0,5,-15);
    [SerializeField] private Vector3 localPositionToLook = new Vector3(0, -1, 5);

    [SerializeField] private float movingSpeed = 0.02f;
    [SerializeField] private float rotationSpeed = 0.1f;

    private Vector3 wantedPos;
    private Quaternion wantedRot;

    private void FixedUpdate()
    {
        wantedPos = target.TransformPoint(localPositionToMove);
        wantedPos.y = target.position.y + localPositionToMove.y;
        transform.position = Vector3.Lerp(transform.position, wantedPos, movingSpeed);
        
        wantedRot = Quaternion.LookRotation(target.TransformPoint(localPositionToLook) - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, wantedRot, rotationSpeed);
    }
}
