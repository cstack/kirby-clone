using UnityEngine;
using System.Collections;

public class SparkAbility : MonoBehaviour {
	public GameObject sparkProjectilePrefab;
	public float duration = 2f;
	public float shocksPerSecond = 20f;

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
		GameObject shock = Instantiate (sparkProjectilePrefab) as GameObject;
		shock.transform.position = transform.position;
		
		Vector2 direction = Quaternion.AngleAxis (angle, Vector3.forward) * Vector2.up;
		shock.rigidbody2D.velocity = direction * 10f;
	}
}
