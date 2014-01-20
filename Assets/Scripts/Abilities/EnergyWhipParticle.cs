using UnityEngine;
using System.Collections;

public class EnergyWhipParticle : MonoBehaviour {
	public float lifeSpan = 0.3f;

	public void Start() {
		StartCoroutine ("DieAfterTime");
	}

	public IEnumerator DieAfterTime() {
		yield return new WaitForSeconds (lifeSpan);
		Destroy (gameObject);
	}

	public void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "kirby") {
			other.SendMessage("TakeHit", this);
		}
	}

}
