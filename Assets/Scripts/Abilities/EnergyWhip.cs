using UnityEngine;
using System.Collections;

public class EnergyWhip : MonoBehaviour {

	public GameObject particlePrefab;
	public float particleSpeed = 10f;
	public float startAngle = 45.0f;
	public float rotation = 90.0f;
	public int numParticles = 10;
	public float offset = 1f;

	// Use this for initialization
	void Start () {
		StartCoroutine(ShootParticles ());
	}

	IEnumerator ShootParticles() {
		for (int i = 0; i < numParticles; i++) {
			StartCoroutine(ShootParticle(startAngle + rotation * (float) i / (numParticles-1)));
			yield return new WaitForSeconds(0.1f);
		}
	}

	IEnumerator ShootParticle(float angle) {
		// Shoot a particle at `angle` degrees from horizontal
		GameObject particle = Instantiate (particlePrefab) as GameObject;
		particle.transform.position = transform.position;

		Vector2 direction = Quaternion.AngleAxis (angle, Vector3.forward) * Vector2.up;
		particle.rigidbody2D.velocity = direction * particleSpeed;
		particle.transform.position = (Vector2) transform.position + direction * offset;
		yield return new WaitForSeconds (0.3f);
		Destroy (particle);
	}
}
