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
		goToDefaultState();
		dir = Direction.Left;
	}

	private float DistanceFromScreen() {
		Vector3 leftEdge = Camera.main.WorldToScreenPoint(transform.position);
		Vector3 rightEdge = Camera.main.WorldToScreenPoint(transform.position + new Vector3(1f, 0,  0));
		if (rightEdge.x < 0) {
			return Mathf.Abs(rightEdge.x);
		}
		if (leftEdge.x > Camera.main.pixelWidth) {
			return leftEdge.x - Camera.main.pixelWidth;
		}
		return 0f;
	}

	new public void Update() {
		base.Update();
		if (DistanceFromScreen() >= 100f) {
			Destroy(gameObject);
		}
	}

	new void OnTriggerEnter2D(Collider2D other) {
		base.OnTriggerEnter2D(other);
		if (other.gameObject.tag == "inhale") {
			CurrentState = State.BeingInhaled;
		}
	}

	protected abstract void goToDefaultState();

	protected void BeingInhaledUpdate() {
		if (kirby.CurrentState.ToString() != Kirby.State.Inhaling.ToString()) {
			goToDefaultState();
			return;
		}
		Vector2 force = kirby.transform.position - transform.position;
		rigidbody2D.AddForce(force);
	}

	protected float distanceToKirby() {
		return Vector2.Distance (transform.position, kirby.transform.position);
	}

}
