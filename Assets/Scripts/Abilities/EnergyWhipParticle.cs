using UnityEngine;
using System.Collections;

public class EnergyWhipParticle : Projectile {

	public void OnTriggerEnter2D(Collider2D other) {
		if ((friendly && other.gameObject.tag == "enemy") ||
		    (!friendly && other.gameObject.tag == "kirby") ||
		    other.gameObject.tag == "boss") {
			other.SendMessage("TakeHit", gameObject);
		}
	}

}
