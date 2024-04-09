using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public struct NeuronDisplay
{
    public GameObject go;
    public RectTransform rectTransform;
    private Image image;
    private TextMeshProUGUI text;

    public void Init(float xPos, float yPos)
    {
        rectTransform = go.GetComponent<RectTransform>();
        image = go.GetComponent<Image>();
        text = go.GetComponentInChildren<TextMeshProUGUI>();
        rectTransform.anchoredPosition = new Vector2(xPos, yPos);
    }

    public void Refresh(float value, Color color)
    {
        text.text = value.ToString("F2");
        image.color = color;
    }
}

public struct AxonDisplay
{
    public GameObject go;
    private RectTransform rectTransform;
    public Image image;

    public void Init(RectTransform start, RectTransform end, float thickness, float neuronDiameter)
    {
        rectTransform = go.GetComponent<RectTransform>();
        image = go.GetComponent<Image>();
        rectTransform.anchoredPosition = start.anchoredPosition + (end.anchoredPosition - start.anchoredPosition) * .5f;
        rectTransform.sizeDelta =
            new Vector2((end.anchoredPosition - start.anchoredPosition).magnitude - neuronDiameter, thickness);
        rectTransform.rotation = Quaternion.FromToRotation(rectTransform.right, (end.anchoredPosition - start.anchoredPosition).normalized);
        rectTransform.SetAsFirstSibling();
    }
    public void Refresh(float value, Color color)
    {
        image.color = color;
    }
}

public class NeuralNetworkViewier : MonoBehaviour
{
    [SerializeField] private float layersSpacing = 100;
    [SerializeField] private float neuronVerticalSpacing = 32;
    [SerializeField] private float neuronDiameter = 32;
    [SerializeField] private float axonThickness = 2;
    [SerializeField] private Gradient colorGradient;

    [Space]
    [SerializeField] private GameObject neuronePrefab;
    [SerializeField] private GameObject axonPrefab;
    [SerializeField] private GameObject fitnessPrefab;
    [SerializeField] private RectTransform viewGroup;

    [Space]
    public Agent agent;
    private NeuralNetwork net;

    private NeuronDisplay[][] neurons;
    private AxonDisplay[][][] axons;
    private TextMeshProUGUI fitnessDisplay;

    private bool initialised;
    private int maxNeurons;
    private float padding;

    private int x;
    private int y;
    private int z;
    
    public static NeuralNetworkViewier Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void Refresh(Agent agentToLook)
    {
        agent = agentToLook;
        net = agent.net;
        if (initialised == false)
        {
            initialised = true;
            Init();
        }

        RefreshAxons();
    }

    private void Init()
    {
        InitMaxNeuron();
        InitNeurons();
        InitAxon();
        InitFitness();
    }

    private void InitMaxNeuron()
    {
        for (x = 0; x < net.layers.Length; x++)
        {
            if (net.layers[x] > maxNeurons)
            {
                maxNeurons = net.layers[x];
            }
        }   
    }

    private void InitNeurons()
    {
        neurons = new NeuronDisplay[net.layers.Length][];
        for (x = 0; x < net.layers.Length; x++)
        {
            if (net.layers[x] < maxNeurons)
            {
                padding = (maxNeurons - net.layers[x]) * .5f * neuronVerticalSpacing;
                
                if (net.layers[x] % 2 != maxNeurons % 2)
                {
                    padding += neuronVerticalSpacing * .5f;
                }
            }
            else padding = 0;

            neurons[x] = new NeuronDisplay[net.layers[x]];
            for (y = 0; y < net.layers[x]; y++)
            {
                neurons[x][y] = new NeuronDisplay();
                neurons[x][y].go = Instantiate(neuronePrefab, viewGroup);
                neurons[x][y].Init(x * layersSpacing, -padding - neuronVerticalSpacing * y);
            }
        }
    }

    private void InitAxon()
    {
        axons = new AxonDisplay[net.layers.Length - 1][][];
        for (x = 0; x < net.layers.Length - 1; x++)
        {
            axons[x] = new AxonDisplay[net.layers[x]][];
            for (y = 0; y < net.layers[x]; y++)
            {
                axons[x][y] = new AxonDisplay[net.layers[x+1]];
                for (z = 0; z < net.layers[x+1]; z++)
                {
                    axons[x][y][z] = new AxonDisplay();
                    axons[x][y][z].go = Instantiate(axonPrefab, viewGroup);
                    
                    axons[x][y][z].Init(neurons[x][y].rectTransform, neurons[x+1][z].rectTransform,
                        axonThickness, neuronDiameter);
                }
            }
        }
    }

    private void InitFitness()
    {
        GameObject fitness = Instantiate(fitnessPrefab, viewGroup);
        fitness.GetComponent<RectTransform>().anchoredPosition = new Vector2(net.layers.Length * layersSpacing,
            maxNeurons * .5f * neuronVerticalSpacing);
        fitnessDisplay = fitness.GetComponent<TextMeshProUGUI>();
    }

    private void RefreshAxons()
    {
        for (x = 0; x < axons.Length; x++)
        {
            for (y = 0; y < axons[x].Length; y++)
            {
                for (z = 0; z < axons[x][y].Length; z++)
                {
                    axons[x][y][z].image.color = colorGradient.Evaluate((net.axons[x][y][z] + 1) * .5f);
                }
            }
        }
    }

    private void Update()
    {
        for (x = 0; x < neurons.Length; x++)
        {
            for (y = 0; y < neurons[x].Length; y++)
            {
                neurons[x][y].Refresh(net.neurons[x][y], colorGradient.Evaluate((net.neurons[x][y] + 1) * .5f));
            }
        }

        fitnessDisplay.text = agent.fitness.ToString("F1");
    }
}
