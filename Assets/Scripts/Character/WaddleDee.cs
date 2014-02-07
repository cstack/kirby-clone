using UnityEngine;
using System.Collections;

public class WaddleDee : EnemyBase {
	public float speed = 2f;

	private enum State {
		Walk
	}

	new void Start () {
		base.Start();
		dir = Direction.Left;
	}

	protected override void setPoints() {
		points = 200;
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

	#region implemented abstract members of EnemyBase

	protected override void goToDefaultState() {
		CurrentState = State.Walk;
	}

	#endregion

}
