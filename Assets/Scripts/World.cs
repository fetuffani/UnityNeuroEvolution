using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	public GameObject snake;
	public GameObject apple;
	public GameObject[] snakes;
	public GameObject[] apples;
	public GameObject Border;

	private int snakesnum = 10;
	private int applesnum = 10;


	public int[] NeuralNetworkShape = { 10, 4, 4 };
	public int NeuralNetworkWeightsCount;
	public GeneticAlgorithm<double> GenePool;
	public int AdditionalGenes = 0;


	void Start()
	{
		NeuralNetworkWeightsCount = Extensions.NeuralNetworkWeightCount(NeuralNetworkShape);

		GenePool = new GeneticAlgorithm<double>(
			snakesnum, //Population size
			NeuralNetworkWeightsCount + AdditionalGenes, //DNA size, calculated by the network weights plus the additional genes
			Extensions.Random,
			getRandomGene,
			5, //Elitism
			0.05
		);

		snakes = new GameObject[snakesnum];
		for (int i = 0; i < snakes.Length; i++)
		{
			snakes[i] = Instantiate(snake);
			snakes[i].name = $"snake_{i}";
			RandomLocation(snakes[i]);
			
		}

		apples = new GameObject[applesnum];
		for (int i = 0; i < apples.Length; i++)
		{
			apples[i] = Instantiate(apple);
			apples[i].name = $"apple_{i}";
			RandomLocation(apples[i]);

		}
	}

	private double getRandomGene()
	{
		return Extensions.GetRandom(-1, 1);
	}
	public void RandomLocation(GameObject obj)
	{
		var border = GetMaxBounds(Border);
		obj.transform.position = new Vector3(
				UnityEngine.Random.Range(border.min.x, border.max.x),
				UnityEngine.Random.Range(border.min.y, border.max.y),
		   -1f
	   );
	}

	Bounds GetMaxBounds(GameObject g)
	{
		var b = new Bounds(g.transform.position, Vector3.zero);
		foreach (Renderer r in g.GetComponentsInChildren<Renderer>())
		{
			b.Encapsulate(r.bounds);
		}
		return b;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			GenePool.NewGeneration(2);
			for (int i = 0; i < snakes.Length; i++)
			{
				snakes[i].GetComponent<SnakeBrain>().AssignDNA(i);
			}
		}
	}
}
