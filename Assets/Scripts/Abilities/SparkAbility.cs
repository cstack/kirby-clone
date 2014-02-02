using UnityEngine;
using System.Collections;

public class SparkAbility : Ability {
	public SparkProjectile sparkProjectilePrefab;
	public float duration = 2f;
	public float shocksPerSecond = 20f;

	#region implemented abstract members of Ability

	public override float getDuration()
	{
		return duration;
	}

	#endregion

	// Use this for initialization
	public void Start () {
		StartCoroutine(ShootSparks());
	}

	private IEnumerator ShootSparks() {
		float timestep = duration / shocksPerSecond;
		float start = Time.time;
		while (Time.time - start < duration) {
			ShootShock(Random.value * 360);
			yield return new WaitForSeconds(timestep);
		}
		Destroy(gameObject);
	}

	private void ShootShock(float angle) {
		// Shoot a shock at `angle` degrees
		SparkProjectile shock = Instantiate (sparkProjectilePrefab) as SparkProjectile;
		shock.transform.position = transform.position;
		shock.friendly = friendly;
		
		Vector2 direction = Quaternion.AngleAxis (angle, Vector3.forward) * Vector2.up;
		shock.rigidbody2D.velocity = direction * 10f;
	}
}
