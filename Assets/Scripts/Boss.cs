using UnityEngine;
using System.Collections;

public class Boss : CharacterBase {

	public float inhaleStrength = 100f;
	public int health = 10;
	public float inhaleChance = 0.5f;

	private Kirby kirby;
	private float startTime;
	private float vel = 0f;

	public enum State {
		IdleOrWalking, Inhaling, Inhaled, Shooting, Swallowing, Knockback
	}

	new public void Start() {
		base.Start();
		kirby = (Kirby) GameObject.Find("Kirby").GetComponent(typeof(Kirby));
		CurrentState = State.IdleOrWalking;
		GetComponentInChildren<Animator>().speed = 0.5f;
		Flip();
	}

	public static void TakeDamage(GameObject boss) {
		Boss b = boss.GetComponent<Boss>();
		b.TakeDamage();
	}

	public void TakeDamage() {
		if (CurrentState.ToString() == State.Knockback.ToString()) {
			return;
		}
		health -= 1;
		Color c = GetComponentInChildren<SpriteRenderer>().color;
		c.g -= 0.05f;
		c.b -= 0.05f;
		GetComponentInChildren<SpriteRenderer>().color = c;
		CurrentState = State.Knockback;
	}

	public void TakeHit(GameObject particle) {
		TakeDamage();
	}

	public IEnumerator KnockbackEnterState() {
		yield return new WaitForSeconds(0.5f);
		CurrentState = State.IdleOrWalking;
	}

	public IEnumerator IdleOrWalkingEnterState() {
		yield return new WaitForSeconds(2f);
		TakeAction();
	}

	public void IdleOrWalkingUpdate() {
		updateXVelocity(vel);
	}

	public void IdleOrWalkingOnCollisionEnter2D(Collision2D collision) {
		CommonOnCollisionEnter2D(collision);
	}

	public void TakeAction() {
		float r = Random.value;
		if (r < inhaleChance) {
			CurrentState = State.Inhaling;
		} else {
			StartCoroutine(WalkAround());
		}
	}

	public IEnumerator WalkAround() {
		vel = -2;
		am.animate(1);
		yield return new WaitForSeconds (3f);
		vel = 0;
		am.animate(0);
		yield return new WaitForSeconds (3f);
		vel = 2;
		am.animate(1);
		yield return new WaitForSeconds (3f);
		vel = 0;
		am.animate(0);
		CurrentState = State.IdleOrWalking;
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

	public void CommonOnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.tag == "kirby") {
			kirby.TakeHit(gameObject);
		}
	}
}
