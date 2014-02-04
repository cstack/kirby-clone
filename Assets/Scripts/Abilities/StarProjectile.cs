using UnityEngine;
using System.Collections;

public class StarProjectile : MonoBehaviour {

	public float speed = 30f;
	public int direction = 1;

	void Start() {
		rigidbody2D.velocity = new Vector2(speed * direction, 0);
	}

	public void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "enemy" || other.tag == "ground") {
			Destroy(gameObject);
			if (other.tag == "enemy") {
				Destroy(other.gameObject);
			}
		}
	}
}
