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

	private int snakesnum = 3;
	private int applesnum = 3;


	void Start()
	{
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

	public void RandomLocation(GameObject obj)
	{
		var border = GetMaxBounds(Border);
		obj.transform.position = new Vector3(
				Random.Range(border.min.x, border.max.x),
				Random.Range(border.min.y, border.max.y),
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
		GameObject sceneCamObj = GameObject.Find("SceneCamera");
		if (sceneCamObj != null)
		{
			// Should output the real dimensions of scene viewport
			//Debug.Log(sceneCamObj.GetComponent<Camera>().rect);
		}
	}
}
