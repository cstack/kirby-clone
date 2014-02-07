using UnityEngine;
using System.Collections;

public abstract class EnemyBase : CharacterBase {
	private float inhaleStrength = 15;
	public float distanceFromScreen = 0f;

	protected Kirby kirby;

	public int points;

	private enum State {
		BeingInhaled
	}

	public static void killEnemy(GameObject obj, bool doublePoints) {
		EnemyBase enemy = obj.GetComponent<EnemyBase>();
		enemy.kill(doublePoints);
	}
	
	new public void Start() {
		base.Start();
		setPoints();
		GameObject go = GameObject.Find ("Kirby");
		kirby = (Kirby) go.GetComponent(typeof(Kirby));
		goToDefaultState();
		dir = Direction.Left;
	}

	protected abstract void setPoints();

	private void kill(bool doublePoints) {
		int mult;
		if (doublePoints) {
			mult = 2;
		} else {
			mult = 1;
		}
		Kirby.score += points * mult;
		Destroy(gameObject);
		GameObject.Find("Score").GetComponent<Score>().updateScore(Kirby.score);
	}

	private float DistanceFromScreen() {
		Vector3 leftEdge = Camera.main.WorldToScreenPoint(transform.position);
		Vector3 rightEdge = Camera.main.WorldToScreenPoint(transform.position + new Vector3(1f, 0,  0));
		if (rightEdge.x < 0) {
			return Mathf.Abs(rightEdge.x);
		}
		if (leftEdge.x > Camera.main.pixelWidth) {
			return leftEdge.x - Camera.main.pixelWidth;
		}
		return 0f;
	}

	new public void Update() {
		base.Update();
		if (DistanceFromScreen() > distanceFromScreen) {
			Destroy(gameObject);
		} else if (transform.position.x <= 0) {
			Destroy(gameObject);
		}
	}

	new void OnTriggerEnter2D(Collider2D other) {
		base.OnTriggerEnter2D(other);
		if (other.gameObject.tag == "inhale") {
			CurrentState = State.BeingInhaled;
		}
	}

	protected abstract void goToDefaultState();
	
	protected IEnumerator BeingInhaledEnterState() {
		rigidbody2D.velocity = Vector3.zero;
		yield break;
	}

	protected void BeingInhaledUpdate() {
		if (kirby.CurrentState.ToString() != Kirby.State.Inhaling.ToString()) {
			goToDefaultState();
		} else {
			Vector2 vector = kirby.transform.position - transform.position;
			rigidbody2D.velocity = vector / vector.sqrMagnitude * inhaleStrength;
		}
	}

	protected float distanceToKirby() {
		return Vector2.Distance (transform.position, kirby.transform.position);
	}

	public void TakeHit(GameObject particle) {
		Destroy(gameObject);
	}

	protected void BounceOffWalls() {
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


}
