using UnityEngine;
using System.Collections;

public class EnergyWhip : Ability {

	public EnergyWhipParticle particlePrefab;
	public float particleSpeed = 10f;
	public float startAngle = 45.0f;
	public float rotation = 90.0f;
	public int numParticles = 10;
	public float offset = 2f;
	public float duration = 1f;

	#region implemented abstract members of Ability

	public override float getDuration()
	{
		return duration;
	}

	#endregion

	// Use this for initialization
	void Start () {
		StartCoroutine(ShootParticles ());
	}

	IEnumerator ShootParticles() {
		float timePerParticle = duration / numParticles;
		for (int i = 0; i < numParticles; i++) {
			ShootParticle(startAngle + rotation * (float) i / (numParticles-1));
			yield return new WaitForSeconds(timePerParticle);
		}
	}

	void ShootParticle(float angle) {
		// Shoot a particle at `angle` degrees from horizontal
		EnergyWhipParticle particle = Instantiate (particlePrefab) as EnergyWhipParticle;
		particle.transform.position = transform.position;
		particle.friendly = friendly;

		Vector3 axis = Vector3.forward;
		if (faceRight) {
			axis = Vector3.back;
		}
		Vector2 direction = Quaternion.AngleAxis (angle, axis) * Vector2.up;
		particle.rigidbody2D.velocity = direction * particleSpeed;
		particle.transform.position = (Vector2) transform.position + direction * offset;
	}
}
