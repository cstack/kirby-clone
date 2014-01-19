using AnimationEnums;
using System;
using System.Collections;
using UnityEngine;

namespace AnimationEnums {
	public enum IdleOrWalking {
		Idle, Walking
	}

	public enum Jumping {
		Jumping, Spinning
	}
}

public class AnimationManager {

	private Animator animator;

	private int state;

	public AnimationManager(Animator animator) {
		this.animator = animator;
	}

	public int State {
		get {
			return state;
		}
		set {
			state = value;
			string stateName = ((Kirby.State) state).ToString();
			animator.Play(stateName);
		}
	}

	public void animate(int subState) {
		animator.SetInteger("SubState", subState);
	}
}
