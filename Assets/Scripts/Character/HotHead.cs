using System.Collections;
using AnimationEnums;

public class HotHead : EnemyBase {
	public float speed = 3f;
	public float range = 3f;
	public float timeBetweenAttacks = 3f;
	
	private enum State {
		Walk, Prepare, Flamethrower, Fireball
	}
	
	protected override void setPoints() {
		points = 300;
	}

	#region Walk

	public void WalkUpdate() {
		updateXVelocity(speed * (dir == Direction.Right ? 1 : -1));
		BounceOffWalls();
	}

	#endregion

	#region implemented abstract members of EnemyBase

	protected override void goToDefaultState()
	{
		CurrentState = State.Walk;
	}

	#endregion
	
}
