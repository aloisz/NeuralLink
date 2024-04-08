using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CheckpointManager : MonoBehaviour
{
    public Transform firstCheckpoint;
    public static CheckpointManager Instance;
    private Checkpoint[] _checkpoints;

    private void Awake()
    {
        Instance = this;
        Init();
    }

    private void Init()
    {
        firstCheckpoint = transform.GetChild(0);
        for (int i = 0; i < transform.childCount -1; i++)
        {
            transform.GetChild(i).GetComponent<Checkpoint>().nextCheckpoint = transform.GetChild(i + 1);
        }
        
        transform.GetChild(transform.childCount - 1).GetComponent<Checkpoint>().nextCheckpoint = firstCheckpoint;
    }
}
