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

	#region implemented abstract members of EnemyBase

	protected override void goToDefaultState() {
		CurrentState = State.Walk;
	}

	#endregion

}
