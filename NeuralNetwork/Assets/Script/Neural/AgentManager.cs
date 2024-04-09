using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    [SerializeField] private int populationSize = 100;
    [SerializeField] private float trainingDuration = 30;
    [SerializeField] private AnimationCurve trainingDurationByFitness;
    [SerializeField] private Agent agentPrefab;
    [SerializeField] private Transform agentGroup;
    [SerializeField] private CameraManager _cameraManager;

    [Space] 
    [SerializeField] private float mutationRate = .2f;
    [SerializeField] private float mutationPower = .1f;

    [Header("Canvas")]
    [Space] [SerializeField]private int generationCount;
    [SerializeField] private TextMeshProUGUI generationCountTxt;
    
    [SerializeField] private float  timeElapsed  = 0;
    [SerializeField] private TextMeshProUGUI timeElapsedTxt;
    
    private List<Agent> agents = new List<Agent>();
    private Agent agent;
    
    public static AgentManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(Loop());
    }

    private IEnumerator Loop()
    {
        StartGeneration();
        ResetTimer();
        Focus();
        yield return new WaitForSeconds(trainingDuration);
        StartCoroutine(Loop());
    }

    private void StartGeneration()
    {
        AddOrRemoveAgent();
        agents = agents.OrderByDescending(a => a.fitness).ToList(); // ReOrder list by fitness value 
        //trainingDuration = trainingDurationByFitness.Evaluate(agents[0].fitness); // Timing managed by animation curve

        MutateAgent();
        ResetAgent();
        SetMaterials();
        RefreshGenerationCount();
    }

    private void AddOrRemoveAgent()
    {
        if (agents.Count != populationSize)
        {
            int dif = populationSize - agents.Count;
            if (dif > 0)
            {
                for (int i = 0; i < dif; i++)
                {
                    AddAgent();
                }
                
            }
            else
            {
                for (int i = 0; i < -dif; i++)
                {
                    RemoveAgent();
                }
            }
        }
    }
    
    private void AddAgent()
    {
        agent = Instantiate(agentPrefab, Vector3.zero, Quaternion.identity, agentGroup);
        agent.net = new NeuralNetwork(agentPrefab.net.layers); // Neural Structure 
        agents.Add(agent);
    }
    private void RemoveAgent()
    {
        Destroy(agents[^1].gameObject);
        agents.RemoveAt(agents.Count - 1);
    }

    private void MutateAgent()
    {
        for (int i = agents.Count / 2; i < agents.Count ; i++) // 50 last agents out of 100
        {
            agents[i].net.CopyNet(agents[i - agents.Count / 2].net);
            agents[i].net.Mutate(mutationRate, mutationPower);
            agents[i].SetMutatedMat();
        }
    }

    private void ResetAgent()
    {
        foreach (var agent in agents)
        {
            agent.ResetAgent();
        }
    }

    private void SetMaterials()
    {
        for (int i = 1; i < agents.Count / 2; i++)
        {
            agents[i].SetDefaultMat();
        }
        agents[0].SetFirstMat();
    }

    private void RefreshGenerationCount()
    {
        generationCount++;
        generationCountTxt.text = generationCount.ToString();
    }

    private void ResetTimer()
    {
        timeElapsed = Time.time;
    }

    private void Update()
    {
        timeElapsedTxt.text = (trainingDuration - (Time.time - timeElapsed)).ToString("f0");
    }

    private void Focus()
    {
        NeuralNetworkViewier.Instance.Refresh(agents[0]);
        _cameraManager.target = agents[0].transform;
    }

    public void Save()
    {
        List<NeuralNetwork> nets = new List<NeuralNetwork>();
        for (int i = 0; i < agents.Count; i++)
        {
            nets.Add(agents[i].net);
        }

        Data data = new Data()
        {
            nets = nets,
            generation = generationCount
        };
        
        DataManager.Instance.Save(data);
    }

    public void Load()
    {
        Data data = DataManager.Instance.Load();
        generationCount = data.generation;
        if (data != null)
        {
            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].net = data.nets[i];
            }
        }
        EndGeneration();
    }

    public void EndGeneration()
    {
        StopAllCoroutines();
        StartCoroutine(Loop());
    }
}
