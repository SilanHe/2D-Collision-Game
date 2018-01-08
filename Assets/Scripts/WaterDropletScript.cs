using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDropletScript : MonoBehaviour {

	public GameObject largeDroplet;
	public GameObject mediumDroplet;
	public GameObject smallDroplet;

	public Transform largeSpawn;
	public Transform mediumSpawn;
	public Transform smallSpawn;

	public GameObject player;
	public Transform bowlTop; // where wind affect the water

	public Transform bowl;

	public Transform edgeLeft;
	public Transform edgeBot;
	public Transform edgeRight;

	private List<GameObject> interactiveObjects;
	private List<GameObject> bowlComponents;
	private List<GameObject> removeGameObject;
	private List<Vector2> vectors;
	private WindScript windScript;
	private WaterLineScript waterLineScript;

	private Vector3[] waterLine;

	private float verticalAngle;

	private float gravity;

	private float speed;

	public GameObject enemy;
	public Transform spawn;

	// put the points from unity interface
	public Transform targetWayPoint;

	public float flySpeed = 1f;

	private List<GameObject> cachedEnemies;

	// Use this for initialization
	IEnumerator Start () {
		StartCoroutine("SpawnHandler");

		cachedEnemies = new List<GameObject> ();
		interactiveObjects = new List<GameObject> ();
		removeGameObject = new List<GameObject> ();
		vectors = new List<Vector2> ();

		windScript = GetComponent<WindScript> ();
		waterLineScript = GetComponent<WaterLineScript> ();
		verticalAngle = 90;

		gravity = 0.01f;
		speed = 8f * Time.deltaTime;

		//ghetto way of getting the all the bowl elements that will collide
		bowlComponents = new List<GameObject> ();
		foreach (Transform child in bowl) {
			bowlComponents.Add (child.gameObject);
		}

		yield return new WaitForSeconds(2f);

		waterLine = waterLineScript.result;
	}
	
	// Update is called once per frame
	void Update () {

		int i = 0;
		
		if (Input.GetKeyDown ("space")) {
			ShootDroplets ();
		}

		//manage movement and collisions

		//fly movement

		Walk ();
		

		if (interactiveObjects.Count > 0){

			float horizontal, vertical;

			//droplet parabola movement
			foreach (GameObject go in interactiveObjects) {
					
				//upgrade the movement of the water droplet/ fly
				go.transform.Translate (vectors [i] * speed, Space.World); 

				//apply gravity so that it changes the vector for the next iteration

				horizontal = vectors [i].x;
				vertical = vectors [i].y - gravity;

				if (go.transform.position.y > bowlTop.transform.position.y) {
					horizontal = vectors [i].x + windScript.windSpeed / 1500f;
				}

				vectors [i] = new Vector2 (horizontal, vertical);

				if (go.CompareTag ("Water Droplet")) {
					CollisionWithFly (vectors [i], go);
					vectors [i] = CollisionWithBowl (vectors [i], go);
					if (vectors [i].y < 0) {
						CollisionWithWaterLine (vectors [i], go);
					}
				} else if (go.CompareTag ("Fly")) {
					vectors [i] = CollisionWithBowl (vectors [i], go);
					CollisionWithWaterLine (vectors [i], go);
				}

				OutOfView (vectors [i], go);

				i++;
			}
		}

	}

	void ShootDroplets() {

		float horizontal, vertical;
		float angle = player.transform.eulerAngles.z;

		if (angle > verticalAngle) {
			//shooting towards the left

			horizontal = Mathf.Cos (Mathf.Deg2Rad * angle);
			vertical = Mathf.Sin (Mathf.Deg2Rad * angle);

		} else if (angle < verticalAngle) {
			//shooting towards the right

			horizontal = Mathf.Cos (Mathf.Deg2Rad * angle);
			vertical = Mathf.Sin (Mathf.Deg2Rad * angle);

		} else {
			// angle =  90 in the current scheme
			horizontal = 0;
			vertical = 1;
		}
			
		interactiveObjects.Add((GameObject)Instantiate(largeDroplet, largeSpawn.position, largeSpawn.rotation));
		interactiveObjects.Add((GameObject)Instantiate(mediumDroplet, mediumSpawn.position, mediumSpawn.rotation));
		interactiveObjects.Add((GameObject)Instantiate(smallDroplet, smallSpawn.position, smallSpawn.rotation));

		for (int i = 0; i < 3; i++) {
			vectors.Add (new Vector2(horizontal,vertical));
		}
	}

	Vector2 CollisionWithBowl(Vector2 v, GameObject o) {

		/* Due to the way that sometimes the bugs would get stuck, i decided to up the x component of the force
		whenever a collision would occur to a fly this while reducing the y component to satisfy the requirement
		losing energy */

		Bounds bowl;
		float xExtent, yExtent;
		float circleDistanceX, circleDistanceY, cornerDistance_sq, circleRadius;

		foreach (GameObject go in bowlComponents) {
			bowl = go.GetComponent<MeshRenderer>().bounds;

			xExtent = bowl.extents.x;
			yExtent = bowl.extents.y;

			//Collision detection using AABB and circle
			if (o.CompareTag ("Fly")) {
				circleRadius = 0.31f;
			} else {
				circleRadius = o.transform.lossyScale.x;
			}

			circleDistanceX = Mathf.Abs(o.transform.position.x - go.transform.position.x);
			circleDistanceY = Mathf.Abs(o.transform.position.y - go.transform.position.y);
			cornerDistance_sq = Mathf.Pow(circleDistanceX - xExtent,2) + Mathf.Pow(circleDistanceY - yExtent,2);

			if (circleDistanceX > (xExtent + circleRadius)) {
				//false

			} else if (circleDistanceY > (yExtent + circleRadius)) { 
				//false 

			} else if (Mathf.Abs(v.x) > Mathf.Abs(v.y)) {
				if (circleDistanceX <= xExtent) { 
					//return true;
					if (o.CompareTag ("Water Droplet")) {
						return new Vector2 (v.x * -0.99f, v.y);
					} else if (o.CompareTag ("Fly")) {
						return new Vector2 (v.x * -0.7f, v.y);
					}
				} else if (circleDistanceY <= yExtent) { 
					//return true;
					if (o.CompareTag ("Water Droplet")) {
						return new Vector2 (v.x, v.y * -0.99f);
					} else if (o.CompareTag ("Fly")) {
						return new Vector2 (v.x, v.y * -0.7f);
					}
				} else if (cornerDistance_sq <= Mathf.Pow (circleRadius, 2)) {
					//true
					if (o.CompareTag ("Water Droplet")) {
						return new Vector2 (-0.99f * v.x, -0.99f * v.y);
					} else if (o.CompareTag ("Fly")) {
						return new Vector2 (v.x, -0.7f * v.y);
					}
				}
			} else if (Mathf.Abs(v.y) > Mathf.Abs(v.x)) {
				if (circleDistanceY <= yExtent) { 
					//return true;
					if (o.CompareTag ("Water Droplet")) {
						return new Vector2 (v.x, v.y * -0.99f);
					} else if (o.CompareTag ("Fly")) {
						return new Vector2 (-1.5f * v.x, v.y * -0.4f);
					}
				} else if (circleDistanceX <= xExtent) { 
					//return true;
					if (o.CompareTag ("Water Droplet")) {
						return new Vector2 (v.x * -0.99f, v.y);
					} else if (o.CompareTag ("Fly")) {
						return new Vector2 (-1.5f * v.x * -0.4f, v.y);
					}
				} else if (cornerDistance_sq <= Mathf.Pow (circleRadius, 2)) {
					//true
					if (o.CompareTag ("Water Droplet")) {
						return new Vector2 (-0.99f * v.x, -0.99f * v.y);
					} else if (o.CompareTag ("Fly")) {
						return new Vector2 (1.5f * v.x, -0.4f * v.y);
					}
				}
			} 
				
		}
		return v;
			
	}

	private void CollisionWithFly(Vector2 v, GameObject o) {

		// fly has a radius of 0.31

		foreach (GameObject cachedEnemy in cachedEnemies) {

			//check for collision
			if (Mathf.Pow (o.transform.position.x - cachedEnemy.transform.position.x, 2) + Mathf.Pow (o.transform.position.y - cachedEnemy.transform.position.y, 2) <= Mathf.Pow (o.transform.lossyScale.x + 0.31f, 2)) {

				vectors.Remove (v);
				interactiveObjects.Remove (o);
				StartCoroutine (Kill (o));

				vectors.Add (new Vector2(0f,0f));
				interactiveObjects.Add(cachedEnemy);

				removeGameObject.Add (cachedEnemy);
			}
		}

		//remove all collided cached ennemy after iterating through them
		foreach( GameObject toRemove in removeGameObject) {
			cachedEnemies.Remove (toRemove);
		}

		removeGameObject.Clear ();
	}

	private void CollisionWithWaterLine(Vector2 v, GameObject o) {

		foreach (Vector3 vector in waterLine) {
			
			if (Mathf.Pow (o.transform.position.x - vector.x, 2) + Mathf.Pow (o.transform.position.y - vector.y, 2) <= Mathf.Pow (o.transform.lossyScale.x, 2)) {

				vectors.Remove (v);
				interactiveObjects.Remove (o);
				StartCoroutine (Kill (o));
			}
		}
	}

	private void OutOfView(Vector2 v, GameObject o) {

		if (o.transform.position.x < edgeLeft.position.x || o.transform.position.x > edgeRight.position.x || o.transform.position.y < edgeBot.position.y) {

			vectors.Remove (v);
			interactiveObjects.Remove (o);
			StartCoroutine (Kill (o));
		}

	}
		
	private IEnumerator Kill(float wait,GameObject o) {
		yield return wait;
		Destroy(o);
	}

	private IEnumerator Kill(GameObject o) {
		yield return 0.1f;
		Destroy(o);
	}

	//spawn flies
	private IEnumerator SpawnHandler() {
		float spawnDelay;


		while(true){

			spawnDelay = Random.Range(5f,10f); //random time, from 5-10

			yield return new WaitForSeconds(spawnDelay); //wait that time


			cachedEnemies.Add ((GameObject)Instantiate(enemy, spawn.position, spawn.rotation));//spawn enemy, cache him

		}
	}

	//flies path
	private void Walk() {

		WindScript windScript = GetComponent<WindScript> ();

		float windPush = windScript.windSpeed/5 * Time.deltaTime;

		// move towards the target

		foreach (GameObject cachedEnemy in cachedEnemies) {
			if (windPush > 0) {
				cachedEnemy.transform.position = Vector3.MoveTowards (cachedEnemy.transform.position, targetWayPoint.position, Time.deltaTime / 1.5f + windPush);
			}

			if (cachedEnemy.transform.position == targetWayPoint.position) {
				cachedEnemies.Remove (cachedEnemy);
				StartCoroutine (Kill (cachedEnemy));
			}
		}
	} 


}
