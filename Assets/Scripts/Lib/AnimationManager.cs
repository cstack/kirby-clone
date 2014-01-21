using AnimationEnums;
using System;
using System.Collections;
using UnityEngine;

public class AnimationManager {

	private Animator animator;

	private Enum state;

	public AnimationManager(Animator animator) {
		this.animator = animator;
	}

	public Enum State {
		set {
			state = value;
			animator.SetInteger("State", Convert.ToInt32(state));
			String animationName = value.ToString();
			animator.Play(animationName);
			animator.SetInteger("SubState", 0);
		}
	}

	public void animate(int subState) {
		animator.SetInteger("SubState", subState);
	}
}
