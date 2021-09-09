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
		//brake_factor = (150.4f * Average(meanarray)); /* talvez acabe usando pra balizar o filtro kalman */
		//Velocity = Vector3.zero;



		GameObject food = GetClosestFood();

		if (food == null) return; // Do nothing if no food is found


		float difx = Mathf.Abs(food.transform.position.x - gameObject.transform.position.x);
		float dify = Mathf.Abs(food.transform.position.y - gameObject.transform.position.y);

		Vector3 accel = Vector3.zero;


		accel.x = food.transform.position.x - gameObject.transform.position.x;
		accel.y = food.transform.position.y - gameObject.transform.position.y;

		
		System.Action<Vector3, float> LimitMagnitude = (Vector3 vector, float max) =>
		   {
			   if (vector.sqrMagnitude > 1)
			   {
				   float scale;
				   vector.Scale(new Vector3(scale = 1 / Velocity.sqrMagnitude,
				   scale,
				   scale));
			   }
		   };
		LimitMagnitude(accel, 0.1f);
		KalmanAcceleration(/*0.01f*/ Time.deltaTime, accel);
		LimitMagnitude(Velocity, 2f);

		//if (difx > 0.01)
		//{
		//	if (food.transform.position.x > gameObject.transform.position.x)
		//		AccelerateRight();
		//	else
		//		AccelerateLeft();
		//}

		//if (dify > 0.01)
		//{
		//	if (food.transform.position.y > gameObject.transform.position.y)
		//		AccelerateDown();
		//	else
		//		AccelerateUp();
		//}

		gameObject.transform.position += (Velocity * Time.deltaTime);

		float angle = Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	private void KalmanAcceleration(float v, Vector3 accel)
	{
		Velocity.x = Velocity.x * (1f - v) + accel.x * v;
		Velocity.y = Velocity.y * (1f - v) + accel.y * v;
	}

	//Detect collisions between the GameObjects with Colliders attached
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.CompareTag("apple"))
		{
			World.RandomLocation(col.gameObject);
		}

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
