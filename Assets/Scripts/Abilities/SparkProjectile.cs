using UnityEngine;
using System.Collections;

public class SparkProjectile : MonoBehaviour {
	public float lifeSpan = 0.2f;

	public void Start() {
		StartCoroutine ("DieAfterTime");
	}

	public IEnumerator DieAfterTime() {
		yield return new WaitForSeconds (lifeSpan);
		Destroy (gameObject);
	}

	public void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "kirby") {
			other.SendMessage("TakeHit", gameObject);
		}
	}

}
