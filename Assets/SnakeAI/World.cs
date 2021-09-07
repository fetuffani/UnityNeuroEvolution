using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	public GameObject snake;
	public GameObject apple;
	public GameObject[] snakes;
	public GameObject[] apples;

	private int snakesnum = 10;
	private int applesnum = 10;


	void Start()
	{
		snakes = new GameObject[snakesnum];
		for (int i = 0; i < snakes.Length; i++)
		{
			snakes[i] = Instantiate(snake);
			snakes[i].name = $"snake_{i}";
			snakes[i].transform.position = new Vector3(
				Random.Range(-5f, 5f),
				Random.Range(-5f, 5f),
				-1f
			);
			snakes[i].transform.localScale = new Vector3(1f, 1f);
		}

		apples = new GameObject[applesnum];
		for (int i = 0; i < apples.Length; i++)
		{
			apples[i] = Instantiate(apple);
			apples[i].name = $"apple_{i}";
			apples[i].transform.position = new Vector3(
				Random.Range(-5f, 5f),
				Random.Range(-5f, 5f),
				-1f
			);
			apples[i].transform.localScale = new Vector3(0.5f, 0.5f);
		}
	}

	void Update()
	{
		GameObject sceneCamObj = GameObject.Find("SceneCamera");
		if (sceneCamObj != null)
		{
			// Should output the real dimensions of scene viewport
			//Debug.Log(sceneCamObj.GetComponent<Camera>().rect);
		}
	}
}
