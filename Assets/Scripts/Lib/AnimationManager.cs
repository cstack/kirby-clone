using AnimationEnums;
using System.Collections;
using UnityEngine;

namespace AnimationEnums {
	public enum IdleOrWalking {
		IDLE, WALKING
	}
	
	public enum Direction {
		LEFT, RIGHT
	}
}

public class AnimationManager {

	private Animator animator;

	private Direction dir;
	private int state;

	public AnimationManager(Animator animator) {
		this.animator = animator;
		Dir = Direction.RIGHT;
	}

	public Direction Dir {
		get {
			return dir;
		}
		set {
			dir = value;
			animator.SetInteger("Direction", (int) dir);
		}
	}

	public int State {
		get {
			return state;
		}
		set {
			state = value;
			animator.SetInteger("State", value);
			animator.SetInteger("SubState", 0);
		}
	}

	public void animate(int subState) {
		animator.SetInteger("SubState", subState);
	}
}
