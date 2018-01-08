using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	private float translation = 6.0f;

	private float rotation = 60.0f;

	public float MinClamp = 30;
	public float MaxClamp = 150;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update() {
		//restrict side to side movement
		transform.position = new Vector3(Mathf.Clamp(transform.position.x, -4.84F, 4.64F), transform.position.y, transform.position.z);

		var x = Input.GetAxis("Horizontal") * Time.deltaTime * translation;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * rotation;

		transform.Translate(x, 0, 0, Space.World);
		transform.Rotate(0, 0, z);

		//limit rotation of the fish to +- 60 degrees


		if (transform.eulerAngles.z < MinClamp || transform.eulerAngles.z > MaxClamp) {
			transform.Rotate (0, 0, z * -1f);
		}


	}
}