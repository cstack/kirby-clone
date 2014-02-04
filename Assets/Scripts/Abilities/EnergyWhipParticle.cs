using UnityEngine;
using System.Collections;

public class EnergyWhipParticle : Projectile {
	public float lifeSpan = 0.3f;

	public void Start() {
		//StartCoroutine ("DieAfterTime");
	}

	public IEnumerator DieAfterTime() {
		yield return new WaitForSeconds (lifeSpan);
		Destroy (gameObject);
	}

	public void OnTriggerEnter2D(Collider2D other) {
		if ((friendly && other.gameObject.tag == "enemy") ||
		    (!friendly && other.gameObject.tag == "kirby")) {
			other.SendMessage("TakeHit", gameObject);
		}
	}

}
