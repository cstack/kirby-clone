using UnityEngine;
using System.Collections;

public abstract class EnemyBase : CharacterBase {
	protected Kirby kirby;

	private enum State {
		BeingInhaled
	}

	public void Start() {
		GameObject go = GameObject.Find ("Kirby");
		kirby = (Kirby) go.GetComponent(typeof(Kirby));
	}

	new void OnTriggerEnter2D(Collider2D other) {
		base.OnTriggerEnter2D(other);
		if (other.gameObject.tag == "inhale") {
			CurrentState = State.BeingInhaled;
		}
	}

	IEnumerator BeingInhaledEnterState() {
		yield return null;
	}

}
