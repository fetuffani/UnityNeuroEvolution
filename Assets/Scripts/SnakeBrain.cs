using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBrain : MonoBehaviour
{
	public World World;
	// Start is called before the first frame update

	public float speed;

	// Feeh - fiz esse campo so para vc ver, da para tornar editavel la no Unity mesmo sendo private
	[SerializeField]
	private float rotatespeed;

	public float size = 1;

	public Vector3 Velocity;

	public float accel_factor = (float)(0.0002f);
	public float brake_factor = 1.01f; // (float)(252.4 / EvoEngine.FPS);

	void Start()
	{
		World = GameObject.FindObjectOfType<World>();
		//gameObject.transform.Rotate(0, 0, 360*Random.value);
		speed = Random.value/10;
		rotatespeed = Random.Range(-2f,2f);
		meanarray = new float[meanarraylen];
	}

	int meanarraylen = 100;
	int meanarraypos = 0;
	float[] meanarray;

	// Update is called once per frame
	void Update()
	{
		meanarray[meanarraypos = ++meanarraypos % meanarraylen] = Time.deltaTime;
		accel_factor = (float)(0.05);
		brake_factor = (150.4f * Average(meanarray));
		//Velocity = Vector3.zero;

		float scale;
		if (Velocity.sqrMagnitude > 1)
			Velocity.Scale(new Vector3(scale = 1 / Velocity.sqrMagnitude,
				scale,
				scale)); ;

		GameObject food = GetClosestFood();

		if (food == null) return; // Do nothing if no food is found


		float difx = Mathf.Abs(food.transform.position.x - gameObject.transform.position.x);
		float dify = Mathf.Abs(food.transform.position.y - gameObject.transform.position.y);

		if (difx > 0.01)
		{
			if (food.transform.position.x > gameObject.transform.position.x)
				AccelerateRight();
			else
				AccelerateLeft();
		}

		if (dify > 0.01)
		{
			if (food.transform.position.y > gameObject.transform.position.y)
				AccelerateDown();
			else
				AccelerateUp();
		}

		gameObject.transform.position += (Velocity * Time.deltaTime);

		float angle = Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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
		SetVelocity(new Vector3(Velocity.x / brake_factor, Velocity.y - accel_factor, 0));		
	}

	public void AccelerateDown()
	{
		SetVelocity(new Vector3(Velocity.x / brake_factor, Velocity.y + accel_factor, 0));
	}

	public void AccelerateLeft()
	{
		SetVelocity(new Vector3(Velocity.x - accel_factor, Velocity.y / brake_factor, 0));
	}

	public void AccelerateRight()
	{
		SetVelocity(new Vector3(Velocity.x + accel_factor, Velocity.y / brake_factor, 0));
	}

	public virtual void SetVelocity(Vector3 velocity)
	{
		this.Velocity = velocity;		
	}	

	public virtual void SetRotate(GameObject target)
    {
		Vector3 dir = target.transform.position - this.transform.position;		
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
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

	public float Sum(params float[] customerssalary)
	{
		float result = 0;

		for (int i = 0; i < customerssalary.Length; i++)
		{
			result += customerssalary[i];
		}

		return result;
	}

	public float Average(params float[] customerssalary)
	{
		float sum = Sum(customerssalary);
		float result = (float)sum / customerssalary.Length;
		return result;
	}
}
