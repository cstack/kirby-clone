using UnityEngine;
using System.Collections;

public class Boss : CharacterBase {

	public float inhaleStrength = 100f;

	private Kirby kirby;
	private float startTime;

	public enum State {
		IdleOrWalking, Inhaling, Inhaled, Shooting, Swallowing
	}

	new public void Start() {
		base.Start();
		kirby = (Kirby) GameObject.Find("Kirby").GetComponent(typeof(Kirby));
		CurrentState = State.IdleOrWalking;
		GetComponentInChildren<Animator>().speed = 0.5f;
		Flip();
	}

	public IEnumerator IdleOrWalkingEnterState() {
		yield return new WaitForSeconds(2f);
		TakeAction();
	}

	public void TakeAction() {
		CurrentState = State.Inhaling;
	}

	public IEnumerator InhalingEnterState() {
		startTime = Time.time;
		yield return null;
	}

	public void InhalingUpdate() {
		if (Time.time - startTime > 4f) {
			CurrentState = State.IdleOrWalking;
		}
		Attract(kirby);
	}

	public void InhalingOnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.tag == "kirby") {
			kirby.gameObject.SetActive(false);
			CurrentState = State.Inhaled;
		}
	}

	public IEnumerator InhaledEnterState() {
		yield return new WaitForSeconds (2f);
		CurrentState = State.Shooting;
	}

	public IEnumerator ShootingEnterState() {
		yield return new WaitForSeconds (0.2f);
		kirby.transform.position = transform.position + new Vector3 (-3f, 2f, 0f);
		kirby.gameObject.SetActive(true);
		kirby.TakeHit(gameObject);
		yield return new WaitForSeconds (0.3f);
		CurrentState = State.IdleOrWalking;
	}

	public void Attract(CharacterBase character) {
		Vector3 diff = transform.position - character.transform.position;
		character.rigidbody2D.AddForce(diff/diff.magnitude * inhaleStrength);
	}
}
