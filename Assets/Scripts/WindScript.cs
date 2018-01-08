using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class WindScript : MonoBehaviour {

	public Text speed;
	public float windSpeed;

	// Use this for initialization
	void Start () {
		speed.text = "";
		StartCoroutine ("WindHandler");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private IEnumerator WindHandler(){
		float delay;


		while(true){

			delay = Random.Range(1f,2f); //random time, from 1-2

			yield return new WaitForSeconds(delay); //wait that time

			windSpeed = (int)Random.Range (0f, 10f);
			speed.text = windSpeed + "";
		}
	}
}
