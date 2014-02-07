using UnityEngine;
using System.Collections;

public abstract class EnemyBase : CharacterBase {
	private float inhaleStrength = 20;
	public float distanceFromScreen = 0f;

	protected Kirby kirby;

	public int points;

	private enum State {
		BeingInhaled
	}

	new public void Start() {
		base.Start();
		setPoints();
		GameObject go = GameObject.Find ("Kirby");
		kirby = (Kirby) go.GetComponent(typeof(Kirby));
		goToDefaultState();
		dir = Direction.Left;
	}

	protected abstract void setPoints();

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
		if (DistanceFromScreen() > distanceFromScreen) {
			Destroy(gameObject);
		} else if (transform.position.x <= 0) {
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
	
	protected IEnumerator BeingInhaledEnterState() {
		rigidbody2D.velocity = Vector3.zero;
		yield break;
	}

	protected void BeingInhaledUpdate() {
		if (kirby.CurrentState.ToString() != Kirby.State.Inhaling.ToString()) {
			goToDefaultState();
		} else {
			int forceDir = kirby.transform.position.x > transform.position.x ? 1 : -1;
			rigidbody2D.AddForce(Vector2.right * forceDir * inhaleStrength);
		}
	}

	protected float distanceToKirby() {
		return Vector2.Distance (transform.position, kirby.transform.position);
	}

	public void TakeHit(GameObject particle) {
		Destroy(gameObject);
	}

}
