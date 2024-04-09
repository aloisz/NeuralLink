using System;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class NeuralNetwork
{
    public int[] layers = new []{6,6,6,2};
    public float[][] neurons;
    public float[][][] axons;

    // avoid memory allocation
    private int x;
    private int y;
    private int z;
    
    public NeuralNetwork(){}

    public NeuralNetwork(int[] layersModel)
    {
        // Create a new copy 
        layers = new int[layersModel.Length];
        for (x = 0; x < layersModel.Length; x++)
        {
            layers[x] = layersModel[x];
        }
        
        InitNeurons();
        InitAxons();
    }

    private void InitNeurons()
    {
        neurons = new float[layers.Length][]; // number of collun
        for (x = 0; x < layers.Length; x++)
        {
            neurons[x] = new float[layers[x]]; // depth of the collun
        }
    }

    private void InitAxons()
    {
        axons = new float[layers.Length-1][][];
        for (x = 0; x < layers.Length-1; x++)
        {
            axons[x] = new float[layers[x]][];
            for (y = 0; y < layers[x]; y++)
            {
                axons[x][y] = new float [layers[x+1]];
                for (z = 0; z < layers[x+1]; z++)
                {
                    axons[x][y][z] = Random.Range(-1f,1f);
                }
            }
        }
    }
    
    
    private int yPreviousLayer;
    public void FeedForward(float[] inputs)
    {
        neurons[0] = inputs;

        for (x = 1; x < layers.Length; x++)
        {
            for (y = 0; y < layers[x]; y++)
            {
                neurons[x][y] = 0;
                for (yPreviousLayer = 0; yPreviousLayer < layers[x-1]; yPreviousLayer++)
                {
                    neurons[x][y] += neurons[x - 1][yPreviousLayer] * axons[x - 1][yPreviousLayer][y];
                }
                
                neurons[x][y] = (float)Math.Tanh(neurons[x][y]);
            }
        }
    }

    public void CopyNet(NeuralNetwork netCopy)
    {
        for (x = 0; x < netCopy.axons.Length; x++)
        {
            for (y = 0; y < netCopy.axons[x].Length; y++)
            {
                for (z = 0; z < netCopy.axons[x][y].Length; z++)
                {
                    axons[x][y][z] = netCopy.axons[x][y][z];
                }
            }
        }
    }

    public void Mutate(float propability, float power)
    {
        for (x = 0; x < axons.Length; x++)
        {
            for (y = 0; y < axons[x].Length; y++)
            {
                for (z = 0; z < axons[x][y].Length; z++)
                {
                    if (Random.value < propability)
                    {
                        axons[x][y][z] += Random.Range(-power, power);
                    }
                }
            }
        }
    }
}
