using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateFlies : MonoBehaviour {

	public GameObject enemy;
	public Transform spawn;

	// put the points from unity interface
	public Transform targetWayPoint;

	public float speed = 1f;

	public GameObject cachedEnemy;
	public bool activeEnemy;

	private void Start(){
		StartCoroutine("SpawnHandler");
		activeEnemy = false;
	}

	private IEnumerator SpawnHandler(){
		float spawnDelay;


		while(true){
			
			spawnDelay = Random.Range(5f,10f); //random time, from 5-10

			yield return new WaitForSeconds(spawnDelay); //wait that time

			if (!activeEnemy) {
				cachedEnemy = (GameObject)Instantiate(enemy, spawn.position, spawn.rotation);//spawn enemy, cache him
				activeEnemy = true;
			}

		}
	}

	private IEnumerator Kill(GameObject enemy){
		yield return null;
		Destroy(enemy);
		activeEnemy = false;
	}

	// Update is called once per frame
	void Update () {
		if (activeEnemy) {
			Walk ();
		}
	}

	private void Walk(){

		WindScript windScript = GetComponent<WindScript> ();

		float windPush = windScript.windSpeed/5 * Time.deltaTime;

		// move towards the target
		cachedEnemy.transform.position = Vector3.MoveTowards(cachedEnemy.transform.position, targetWayPoint.position, Time.deltaTime/1.5f + windPush);

		if(cachedEnemy.transform.position == targetWayPoint.position)
		{
			StartCoroutine(Kill(cachedEnemy));
		}
	} 

}
