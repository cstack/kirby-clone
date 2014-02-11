using UnityEngine;
using System.Collections;

public class HotHead : EnemyBase {
	public float speed = 3f;
	public float range = 3f;
	public float timeBetweenAttacks = 2.5f;
	
	private enum State {
		Walk, Prepare, Flamethrower, Fireball
	}
	
	protected override void setPoints() {
		points = 300;
	}

	#region Walk

	public IEnumerator WalkEnterState() {
		yield return new WaitForSeconds(timeBetweenAttacks);
		if (CurrentState.ToString() == State.Walk.ToString()) {
			CurrentState = State.Prepare;
		}
	}

	public void WalkUpdate() {
		updateXVelocity(speed * (dir == Direction.Right ? 1 : -1));
		BounceOffWalls();
	}

	#endregion

	#region Prepare
	
	public IEnumerator PrepareEnterState() {
		Vector3 towardKirby = kirby.transform.position - transform.position;
		if ((towardKirby.x > 0 && dir == Direction.Left) ||
		    (towardKirby.x < 0 && dir == Direction.Right)) {
			Flip();
		}
		updateXVelocity(0f);
		yield return new WaitForSeconds(0.5f);
		if (CurrentState.ToString() == State.Prepare.ToString()) {
			CurrentState = State.Flamethrower;
		}
	}
	
	#endregion

	#region Prepare
	
	public IEnumerator FlamethrowerEnterState() {
		UseAbility();
		yield return new WaitForSeconds(2f);
		CurrentState = State.Walk;
	}

	public IEnumerator FlamethrowerExitState() {
		StopAbility();
		yield return null;
	}
	
	#endregion

	#region implemented abstract members of EnemyBase

	protected override void goToDefaultState()
	{
		CurrentState = State.Walk;
	}

	#endregion
	
}
