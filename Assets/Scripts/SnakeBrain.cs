using Accord.Neuro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SnakeBrain : MonoBehaviour
{
	public World World;
	// Start is called before the first frame update

	// Feeh - fiz esse campo so para vc ver, da para tornar editavel la no Unity mesmo sendo private
	[SerializeField]
	private float rotatespeed;

	public float size = 1;

	public float sensorSensibility = 0.25f;

	public Vector3 Velocity;

	public float brake_factor = 1.01f; // (float)(252.4 / EvoEngine.FPS);

	int meanarraylen = 100;
	int meanarraypos = 0;
	float[] meanarray;




	ActivationNetwork Network;
	public int CurrentDNA;
	public int FoodSensorsCount;
	public static float FoodSensorsSpread = ((float)Math.PI / 180.0f) * 90.0f;
	public float VelocityRotation => (float)Math.Atan2(Velocity.y, Velocity.x);
	public float Energy = InitialEnergy;
	public const float InitialEnergy = 10f;


	void Start()
	{
		World = GameObject.FindObjectOfType<World>();
		//gameObject.transform.Rotate(0, 0, 360*Random.value);
		meanarray = new float[meanarraylen];

		FoodSensorsCount = World.NeuralNetworkShape[0];
	}


	// Update is called once per frame
	void Update()
	{
		meanarray[meanarraypos = ++meanarraypos % meanarraylen] = Time.deltaTime;
		//brake_factor = (150.4f * Average(meanarray)); /* talvez acabe usando pra balizar o filtro kalman */
		//Velocity = Vector3.zero;



		//Think();
		ThinkGenetic();
	}

	private void ThinkGenetic()
	{
		if (Energy <= 0)
		{
			var body = FindObjectOfType<Rigidbody2D>();
			body.simulated = false;
			return;
		}

		Energy -= Time.deltaTime;

		if (Network == null)
		{
			Network = new ActivationNetwork(new BipolarSigmoidFunction(), World.NeuralNetworkShape[0], World.NeuralNetworkShape.Skip(1).ToArray());
			// Nw = (I+1)*H1 +(H1+1)*H2 +(H2+1)*O
			// I = inputs
			// H1 = neurons in hidden layer 1
			// H2 = neurons in hidden layer 2
			// O = Number of outputs
			// Nw = (5+1)*0 + (0+1)*0 + (5+1)*4 = 24 // 5in 4out
			// Nw = (5+1)*4 + (4+1)*0 + (4+1)*4 = 44 // 5in 1hl4 4out
			// Nw = (11+1)*4 + (4+1)*0 + (4+1)*4 = 68 // 11in 1hl4 4out

			var dna = GetCurrentGene();
			SetNetworkWeights(dna);
			//float scale = (float)dna.Genes[scene.NeuralNetworkWeightsCount - 1 + 0];
			//if (scale < 0.4) scale = 0.4f;
			//if (scale > 0.6) scale = 0.6f;
			//Snake.SetScale(scale);

			//int div = scene.NeuralNetworkWeightsCount / 3;
			//Color c = new Color(
			//	(int)Math.Max((dna.Genes[div*0] * 255), 128),
			//	(int)Math.Max((dna.Genes[div*1] * 255), 128),
			//	(int)Math.Max((dna.Genes[div*2] * 255), 128)
			//	);
			//Snake.Color = c;
		}
		float[] sensors = GetFoodSensorsActivation();


		{
			//if (snake.Energy > 0)
			{
				float[] activation = sensors;

				for (int i = 0; i < FoodSensorsCount; i++)
				{
					float[] sensorrange = GetFoodSensorRange(i);
					float from = sensorrange[0] + VelocityRotation;
					float to = sensorrange[1] + VelocityRotation;

					float xs = transform.position.x + (0.7f * (float)Math.Cos(from));
					float ys = transform.position.y + (0.7f * (float)Math.Sin(from));
					float xe = transform.position.x + (0.7f * (float)Math.Cos(to));
					float ye = transform.position.y + (0.7f * (float)Math.Sin(to));

					Debug.DrawLine(
							new Vector3(xs, ys, transform.position.z),
							new Vector3(xe, ye, transform.position.z),
							Extensions.MixColor(Color.white,Color.red,activation[i]),
							Time.deltaTime,
							false);

				}
			}
		}

		Debug.ClearDeveloperConsole();
		int pos = 0;
		//ClearConsole();
		//Debug.Log($"{sensors[pos++]:0.00} : {sensors[pos++]:0.00} : {sensors[pos++]:0.00} : {sensors[pos++]:0.00} : {sensors[pos++]:0.00} : {sensors[pos++]:0.00} : {sensors[pos++]:0.00} : {sensors[pos++]:0.00} : {sensors[pos++]:0.00} : {sensors[pos++]:0.00}");


		double[] netout = Network.Compute(convertToDouble(sensors));

		//if (Math.Max(netout[0], netout[1]) > 0.5)
		if (netout[0] < netout[1])
			Accelerate();
		else
			Brake();

		//if (Math.Max(netout[2], netout[3]) > 0.5)
		if (netout[2] > netout[3])
			RotateLeft();
		else
			RotateRight();

		gameObject.transform.position += (Velocity * Time.deltaTime);

		float angle = Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	void OnGUI()
	{
		if (Energy > 0)
		{
			string text = $"{Energy.ToString()}";
			var position = Camera.main.WorldToScreenPoint(gameObject.transform.position);
			var textSize = GUI.skin.label.CalcSize(new GUIContent(text));
			GUI.Label(new Rect(position.x, Screen.height - position.y, textSize.x, textSize.y), text);
		}
	}


	IEnumerator ClearConsole()
	{
		// wait until console visible
		while (!Debug.developerConsoleVisible)
		{
			yield return null;
		}
		yield return null; // this is required to wait for an additional frame, without this clearing doesn't work (at least for me)
		Debug.ClearDeveloperConsole();
	}

	public void Accelerate()
	{
		if (Velocity.sqrMagnitude < 0.1)
			Velocity += new Vector3(0.1f, 0.1f, 0f);

		if (Velocity.sqrMagnitude < 2f)
			Velocity.Scale((Vector3.up + Vector3.right) * 1.1f);
	}

	public void Brake()
	{
		//Velocity.Scale(Vector3.one * 0.9f);
	}

	public void RotateLeft()
	{
		Velocity  = Quaternion.Euler(0, 0, 1f) * Velocity;
	}

	public void RotateRight()
	{
		Velocity = Quaternion.Euler(0, 0, -1f) * Velocity;
	}

	public float[] GetFoodSensorsActivation(GameObject[] apples = null)
	{
		float[] activation = new float[FoodSensorsCount];

		if (apples == null)
			apples = World.apples;


		foreach (GameObject food in apples)
		{
			for (int i = 0; i < FoodSensorsCount; i++)
			{
				float[] sensorrange = GetFoodSensorRange(i);
				float angletofood = (float)Math.Atan2(food.gameObject.transform.position.y - transform.position.y, food.transform.position.x - transform.position.x) - VelocityRotation;

				if (angletofood >= sensorrange[0] && angletofood <= sensorrange[1])
				{

					float distance = Vector3.Distance(transform.position, food.transform.position);
					float actv = (float)Math.Sqrt(Extensions.SigmoidFunctionDiffNormalized(distance * sensorSensibility));
					if (activation[i] < actv)
						activation[i] = actv;

				}
			}
		}

		return activation;

	}

	public float[] GetFoodSensorRange(int sensor)
	{
		if (sensor >= FoodSensorsCount)
			throw new Exception("Sensor not found");
		else if (sensor < 0)
			throw new Exception($"Sensor number must zero or less than {FoodSensorsCount}");
		else
		{
			float from = ((float)(sensor + 0) / (float)FoodSensorsCount) * FoodSensorsSpread - FoodSensorsSpread / 2.0f;
			float to = ((float)(sensor + 1) / (float)FoodSensorsCount) * FoodSensorsSpread - FoodSensorsSpread / 2.0f;

			return new float[] { from, to };
		}
	}

	private DNA<double> GetCurrentGene()
	{
		DNA<double> dna = World.GenePool.Population[CurrentDNA];

		return dna;
	}

	private void SetNetworkWeights(DNA<double> dna)
	{
		// Get only the network weights
		double[] genes = dna.Genes.Take(World.NeuralNetworkWeightsCount).ToArray();

		int index = 0;
		foreach (var layer in Network.Layers)
		{
			foreach (var neur in layer.Neurons)
			{
				for (int i = 0; i < neur.Weights.Length; i++)
				{
					try { 
						neur.Weights[i] = genes[index++]; 
					}
					catch
					{

					}
					
				}

				((ActivationNeuron)neur).Threshold = genes[index++];
			}
		}

		if (index != World.NeuralNetworkWeightsCount)
			throw new Exception("An error ocurred while retrieving the network weights!");
	}

	internal void AssignDNA(int i)
	{
		var body = FindObjectOfType<Rigidbody2D>();
		body.simulated = true;
		CurrentDNA = i;
		var dna = GetCurrentGene();
		SetNetworkWeights(dna);
		Velocity = Vector3.zero;
		Energy = InitialEnergy;
	}

	public double[] convertToDouble(float[] inputArray)
	{
		if (inputArray == null)
			return null;

		double[] output = new double[inputArray.Length];
		for (int i = 0; i < inputArray.Length; i++)
			output[i] = inputArray[i];

		return output;
	}

	private void Think()
	{
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
			Energy += 5f;
			World.GenePool.Population[CurrentDNA].Fitness++;
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
