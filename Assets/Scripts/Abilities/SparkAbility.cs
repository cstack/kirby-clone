using UnityEngine;
using System.Collections;

public class SparkAbility : Ability {
	public SparkProjectile sparkProjectilePrefab;
	public float shocksPerSecond = 1f;
	public float particleSpeed = 2f;
	public float distanceFromCenter = 0.7f;


	// Use this for initialization
	public void Start () {
		StartCoroutine(ShootSparks());
	}

	private IEnumerator ShootSparks() {
		float timestep = 1 / shocksPerSecond;
		while (true) {
			ShootShock(Random.Range(0, 12) * 30);
			yield return new WaitForSeconds(timestep);
		}
	}

	private void ShootShock(float angle) {
		// Shoot a shock at `angle` degrees
		SparkProjectile shock = Instantiate (sparkProjectilePrefab) as SparkProjectile;
		shock.friendly = friendly;
		
		Vector2 direction = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up;

		shock.transform.position = transform.position + new Vector3(direction.x, direction.y, 0) * distanceFromCenter -
			- Vector3.left * 0.5f - Vector3.down * 0.5f;
		shock.rigidbody2D.velocity = direction * particleSpeed;
	}

	public void OnTriggerEnter2D(Collider2D other) {
		if ((friendly && other.gameObject.tag == "enemy") ||
		    (!friendly && other.gameObject.tag == "kirby") ||
		    other.gameObject.tag == "boss") {
			other.SendMessage("TakeHit", gameObject);
		}
	}
}
