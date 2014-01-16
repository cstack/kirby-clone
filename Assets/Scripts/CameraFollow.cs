using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	public Transform poi;
	public float u = 0.4f;
	public float distanceFromEdge = 6.6f;
	public float levelWidth = 63.7f;
	public float y = 27.6f;
	public float z = -9f;

	// Use this for initialization
	void Start () {
		poi = GameObject.Find ("Kirby").transform;
	}
	
	// Update is called once per frame
	void Update () {
		float target = poi.position.x;

		// Don't go past edge of level
		if (target < distanceFromEdge) {
			target = distanceFromEdge;
		} if (target > (levelWidth - distanceFromEdge)) {
			target = (levelWidth - distanceFromEdge);
		}

		float currentX = transform.position.x;
		float newX = (1 - u) * currentX + u * target;
		transform.position = new Vector3 (newX, y, z);
	}
}
