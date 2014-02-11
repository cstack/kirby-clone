using UnityEngine;
using System.Collections;

public class SparkAbility : Ability {
	public SparkProjectile sparkProjectilePrefab;
	public float shocksPerSecond = 50f;
	public float particleSpeed = 7f;

	// Use this for initialization
	public void Start () {
		StartCoroutine(ShootSparks());
	}

	private IEnumerator ShootSparks() {
		float timestep = 1 / shocksPerSecond;
		while (true) {
			ShootShock(Random.value * 360);
			yield return new WaitForSeconds(timestep);
		}
	}

	private void ShootShock(float angle) {
		// Shoot a shock at `angle` degrees
		SparkProjectile shock = Instantiate (sparkProjectilePrefab) as SparkProjectile;
		shock.origin = transform.position;
		shock.radius = GetComponent<CircleCollider2D>().radius;
		shock.friendly = friendly;
		
		Vector2 direction = Quaternion.AngleAxis (angle, Vector3.forward) * Vector2.up;
		shock.transform.position = transform.position + new Vector3(direction.x, direction.y, 0);
		shock.rigidbody2D.velocity = direction * particleSpeed;
	}

	public void OnTriggerEnter2D(Collider2D other) {
		if ((friendly && other.gameObject.tag == "enemy") ||
		    (!friendly && other.gameObject.tag == "kirby")) {
			other.SendMessage("TakeHit", gameObject);
		}
	}
}
