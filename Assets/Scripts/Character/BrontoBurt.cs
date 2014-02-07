using UnityEngine;
using System.Collections;

public class BrontoBurt : EnemyBase {

	public float period = 1f;
	public float amplitude = 3f;
	public float speed = 2f;

	public enum Strategy {
		SineWave, FlyAway
	}
	public Strategy strategy = Strategy.SineWave;

	private float startTime;
	private bool flyAway;

	private enum State {
		Begin, FastFlap, SlowFlap, Fall
	}

	#region implemented abstract members of EnemyBase

	protected override void goToDefaultState()
	{
		if (kirby.transform.position.x - transform.position.x > 0) {
			Flip();
			updateXVelocity(speed);
		} else {
			updateXVelocity(speed * -1);
		}
		CurrentState = State.Begin;
	}

	#endregion

	public IEnumerator BeginEnterState() {
		startTime = Time.time;
		switch (strategy) {
		case (Strategy.SineWave):
			StartCoroutine(SineWaveStrategy());
			break;
		case (Strategy.FlyAway):
			CurrentState = State.SlowFlap;
			break;
		}
		yield return null;
	}

	public IEnumerator SineWaveStrategy() {
		CurrentState = State.Fall;
		yield return new WaitForSeconds(period/2);
		CurrentState = State.FastFlap;
		yield return new WaitForSeconds(period);
		for (int i = 0; i < 2; i++) {
			CurrentState = State.Fall;
			yield return new WaitForSeconds(period);
			CurrentState = State.FastFlap;
			yield return new WaitForSeconds(period);
		}
		flyAway = true;
		CurrentState = State.FastFlap;
	}
	
	#region FastFlap
	
	public void FastFlapUpdate () {
		if (flyAway) {
			updateYVelocity(rigidbody2D.velocity.y + Time.deltaTime * 2);
		} else {
			float t = Time.time - startTime;
			updateYVelocity(amplitude * Mathf.Sin(t * Mathf.PI / period) * -1);
		}
	}

	#endregion

	#region FastFlap
	
	public void FallUpdate () {
		float t = Time.time - startTime;
		updateYVelocity(amplitude * Mathf.Sin(t * Mathf.PI / period) * -1);
	}

	#endregion

	#region SlowFlap
	
	public void SlowFlapUpdate () {
		updateYVelocity(rigidbody2D.velocity.y + Time.deltaTime * 2);
	}

	#endregion
	
}

