using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public NeuralNetwork net;
    public float fitness;
    [Space]
    [SerializeField] private float rayRange = 5;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private CarController _carController;
    [SerializeField] private Rigidbody rb;
    
    private float distanceTraveled;
    [Space]
    [SerializeField] private float totalCheckpointDist;
    public Transform nextCheckpoint;
    [SerializeField] private float nextCheckpointDist;
    
    private float[] inputs;
    
    Vector3 setUpPos;
    Vector3 transformForward;
    Vector3 transformRight;

    [SerializeField]private MeshRenderer _meshRenderer;
    [Space]
    [SerializeField] private Material firstMat;
    [SerializeField] private Material defaulttMat;
    [SerializeField] private Material mutatedMat;
    private void Start()
    {
        //_meshRenderer = GetComponent<MeshRenderer>();
        
        setUpPos = Vector3.up * 0.02f;
        transformForward = transform.forward;
        transformRight = transform.right;
        
        IsTouched(false);
    }

    public void ResetAgent()
    {
        inputs = new float[net.layers[0]]; // Init input 
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        _carController.Reset();
        fitness = 0;
        totalCheckpointDist = 0;
        nextCheckpoint = CheckpointManager.Instance.firstCheckpoint;
        nextCheckpointDist = (nextCheckpoint.position - transform.position).magnitude;

        isGoingWrongWay = 0;
        isTouched = 0;
        checkPoints = 0;
        IsTouched(false);
        RemainingTime = baseTime;
    }

    private void FixedUpdate()
    {
        InputUpdate();
        OutputUpdate();
        FitnessUpdate();
    }

    private Vector3 pos;
    private void InputUpdate()
    {
        pos = transform.position;
        /*var setUpPos = Vector3.up * 0.02f;
        var transformForward = transform.forward;
        var transformRight = transform.right;*/
        
        // Front
        inputs[0] = RaySensor(pos + setUpPos, transform.forward, 4f);
        
        // Sides
        inputs[1] = RaySensor(pos + setUpPos, transform.right, 1.5f);
        inputs[2] = RaySensor(pos + setUpPos, -transform.right, 1.5f);
        
        // Diagonals
        inputs[3] = RaySensor(pos + setUpPos, transform.forward + transform.right, 2f);
        inputs[4] = RaySensor(pos + setUpPos, transform.forward + -transform.right, 2f);

        inputs[5] = 1;
    }

    private RaycastHit hit;
    private float RaySensor(Vector3 origin, Vector3 dir, float lenght)
    {
        if (Physics.Raycast(origin, dir, out hit, rayRange * lenght, layerMask))
        {
            float value = 1 - hit.distance / (rayRange * lenght);
            Debug.DrawRay(origin, dir * hit.distance, Color.Lerp(Color.red, Color.green, value));
            return value;
        }
        else
        {
            if (Physics.Raycast(origin, dir, out hit, rayRange * lenght, layerMask))
            {
                float value = 1 - hit.distance / (rayRange * lenght);
                Debug.DrawRay(origin, dir * hit.distance, Color.Lerp(Color.red, Color.red, value));
            }
            return 0;
        }
    }

    private void OutputUpdate()
    {
        net.FeedForward(inputs);
        _carController.horizontalInput = net.neurons[^1][0];
        _carController.verticalInput = net.neurons[^1][1];
    }

    [SerializeField] float isGoingWrongWay;
    private void FitnessUpdate()
    {
        distanceTraveled = totalCheckpointDist +
                           (nextCheckpointDist - (nextCheckpoint.position - transform.position).magnitude);

        isGoingWrongWay = nextCheckpointDist - (nextCheckpoint.position - transform.position).magnitude;
        //if (fitness < distanceTraveled) fitness = distanceTraveled;
        
        RemainingTime -= (Time.fixedDeltaTime % 60) * 10;
        
        fitness = isGoingWrongWay + isTouched + checkPoints + RemainingTime;
    }

    [SerializeField] float isTouched;
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.GetComponent<Collider>() != null)
        {
            IsTouched(true);
        }
    }
    
    private void OnCollisionExit(Collision other)
    {
        IsTouched(false);
    }

    private void IsTouched(bool touching)
    {
        if(touching) isTouched = -50;
        else isTouched = +15;
    }

    public void CheckpointReached(Transform nextCheckpoint)
    {
        totalCheckpointDist += nextCheckpointDist;
        this.nextCheckpoint = nextCheckpoint;
        nextCheckpointDist = (nextCheckpoint.position - transform.position).magnitude;
    }

    public void SetFirstMat()
    {
        _meshRenderer.material = firstMat;
    }
    
    public void SetDefaultMat()
    {
        _meshRenderer.material = defaulttMat;
    }
    public void SetMutatedMat()
    {
        _meshRenderer.material = mutatedMat;
    }

    [SerializeField] private float checkPoints;
    public float AddScoreByPassingCheckpoint(float points)
    {
        return this.checkPoints += points;
    }

    [SerializeField] private float baseTime = 5;
    [SerializeField] private float RemainingTime = 5;
    public void ResetTimer()
    {
        RemainingTime = baseTime;
    }
}
