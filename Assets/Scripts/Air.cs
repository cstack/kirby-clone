using UnityEngine;
using System.Collections;

public class Air : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "enemy") {
			Destroy(other.gameObject);
			Destroy(this.gameObject);
		}
	}
}
