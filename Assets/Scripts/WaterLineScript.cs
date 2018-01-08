using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLineScript : MonoBehaviour {

	public Transform lowest;
	public Transform lineStart, lineEnd;
	private int NMAX = 1000;
	public GameObject line;
	public Vector3[] result;

	// Use this for initialization
	void Start () {
		result = new Vector3[NMAX];
		DrawWaterLine ();
	}

	void DrawWaterLine() {
		
		midpointBisection (lineStart.position, lineEnd.position, 0, NMAX - 1, result);

		result [0] = lineStart.position;
		result [NMAX - 1] = lineEnd.position;
		GameObject waterLine = Instantiate(line, this.gameObject.GetComponent<Transform>());
		waterLine.GetComponent<LineRenderer> ().positionCount = NMAX;
		waterLine.GetComponent<LineRenderer>().SetPositions(result);

	}

	void midpointBisection(Vector3 p1, Vector3 p2, int counter1, int counter2, Vector3[] result){

		float variance, x, y, z;
		x = (p1.x + p2.x) / 2;
		variance = Mathf.Abs (p2.x - p1.x) * 0.2f; 
		y = (p1.y + p2.y) / 2 + Random.value * variance - (variance / 2);
		z = p1.z;

		if (y < lowest.position.y) {
			y = lowest.position.y;
		}
		Vector3 p = new Vector3 (x, y, z);
		int counter = (counter1 + counter2) / 2;
		result [counter] = p;
		if (counter1 + 1 < counter) {
			midpointBisection (p1, p, counter1, counter, result);
		}
		if (counter + 1 < counter2) {
			midpointBisection (p, p2, counter, counter2, result);
		}
	}
		
	// Update is called once per frame
	void Update () {
		
	}
}
