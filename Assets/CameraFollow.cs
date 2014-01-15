using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
	public Transform poi;
	public Vector3 offset = new Vector3(0,0,-10);
	public float u = 0.4f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 target = poi.position + offset;
		Vector3 currentPos = transform.position;
		transform.position = (1 - u) * currentPos + u * target;
	}
}
