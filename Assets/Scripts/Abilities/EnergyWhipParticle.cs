using UnityEngine;
using System.Collections;

public class EnergyWhipParticle : MonoBehaviour {

	public void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "kirby") {
			other.SendMessage("TakeHit", this);
		}
	}

}
