using UnityEngine;
using System.Collections;

public class WaddleDee : CharacterBase {
	public float speed = 2f;

	private enum State {
		Walk
	}

	void Start () {
		CurrentState = State.Walk;
		dir = Direction.Left;
	}
	
	void WalkUpdate () {
		float vel = speed * (dir == Direction.Left ? -1 : 1);
		updateXVelocity (vel);

		BounceOffWalls ();
	}

	void BounceOffWalls() {
		float delta = 0.1f;
		Vector3 origin = transform.position + new Vector3 (0, 0.5f, 0);
		if (dir == Direction.Right) {
			origin += new Vector3(1, 0, 0);
		}
		origin += delta * Vector3.right * (dir == Direction.Right ? 1 : -1);
		RaycastHit2D[] hits = new RaycastHit2D[1];
		int numHits = Physics2D.RaycastNonAlloc (origin, rigidbody2D.velocity, hits, delta);
		if (numHits > 0) {
			RaycastHit2D hit = hits[0];
			if (hit.collider.gameObject.tag == "ground") {
				Flip();
			}
		}
	}

	void WalkOnCollisionEnter2D(Collision2D collision) {
		/*Debug.Log ("OnCollisionEnter");
		if (collision.gameObject.tag == "ground") {
			Debug.Log("with ground");
			if (collision.contacts.Length > 0) {
				Vector2 point = collision.contacts[0].point;
				Vector3 direction = transform.position. + new Vector3(0.5f, 0.5f, 0f) - Vector3 point;
				Debug.Log("Direction " + direction);
				if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
					Flip();
				}
			}

		}*/
	}
}
