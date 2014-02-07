using UnityEngine;
using System.Collections;

public class DoorEntrance : MonoBehaviour {
	private Kirby kirby;
	public string destination;

	// Use this for initialization
	public void Start() {
		GameObject go = GameObject.Find ("Kirby");
		kirby = (Kirby) go.GetComponent(typeof(Kirby));
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance(kirby.transform.position, transform.position) < 0.5f &&
		                     Input.GetKeyDown(KeyCode.UpArrow)) {
			Application.LoadLevel(destination);
		} else {
			kirby.onDoor = false;
		}
	}
}
