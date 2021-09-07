using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBrain : MonoBehaviour
{
	public World World;
	// Start is called before the first frame update

	public float speed;
	public float rotatespeed;
	public float size = 1;

	public Vector3 Velocity;

	public float accel_factor = (float)(1000f / 1000f);
	public float brake_factor = 1.01f; // (float)(252.4 / EvoEngine.FPS);

	void Start()
	{
		World = GameObject.FindObjectOfType<World>();
		//gameObject.transform.Rotate(0, 0, 360*Random.value);
		speed = Random.value/10;
		rotatespeed = Random.Range(-2f,2f);
	}

	// Update is called once per frame
	void Update()
	{
		Velocity = Vector3.zero;

		GameObject food = GetClosestFood();

		if (food == null) return; // Do nothing if no food is found

		if (food.transform.position.x > gameObject.transform.position.x)
			AccelerateRight();
		else
			AccelerateLeft();

		if (food.transform.position.y > gameObject.transform.position.y)
			AccelerateDown();
		else
			AccelerateUp();


		gameObject.transform.position += (Velocity * Time.deltaTime);

		//float angle = 90*Mathf.Atan2(Velocity.y, Velocity.x);
		//gameObject.transform.rotation = Quaternion.Euler(0, 0, angle);
	}

	//Detect collisions between the GameObjects with Colliders attached
	void OnTriggerEnter2D(Collider2D col)
	{
		col.gameObject.transform.position = new Vector3(
				Random.Range(-5f, 5f),
				Random.Range(-5f, 5f),
		   -1f
	   );
	}


	public void AccelerateUp()
	{
		SetVelocity(new Vector3(Velocity.x / brake_factor, Velocity.y - accel_factor));
	}

	public void AccelerateDown()
	{
		SetVelocity(new Vector3(Velocity.x / brake_factor, Velocity.y + accel_factor));
	}

	public void AccelerateLeft()
	{
		SetVelocity(new Vector3(Velocity.x - accel_factor, Velocity.y / brake_factor));
	}

	public void AccelerateRight()
	{
		SetVelocity(new Vector3(Velocity.x + accel_factor, Velocity.y / brake_factor));
	}

	public virtual void SetVelocity(Vector3 velocity)
	{
		this.Velocity = velocity;
	}

	private GameObject GetClosestFood()
	{
		GameObject food = null;
		float dist = float.MaxValue;

		foreach (GameObject entity in World.apples)
		{

			float fooddist = Vector3.Distance(gameObject.transform.position, entity.transform.position);
			if (fooddist < dist) //Ensure we do not circle current food
			{
				food = (GameObject)entity;
				dist = fooddist;
			}
		}

		return food;
	}
}
