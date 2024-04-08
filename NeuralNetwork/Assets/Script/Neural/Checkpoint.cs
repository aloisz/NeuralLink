using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform nextCheckpoint;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Agent>(out Agent agent)) return;
        if(agent.nextCheckpoint != transform) return;
        agent.CheckpointReached(nextCheckpoint);
    }
}
