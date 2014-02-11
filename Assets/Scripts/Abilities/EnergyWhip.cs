using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnergyWhip : Ability {

	public EnergyWhipParticle particlePrefab;

	private float startTime;
	private List<EnergyWhipParticle> particles = new List<EnergyWhipParticle>();
	private bool flickerOn;

	// Use this for initialization
	IEnumerator Start () {
		startTime = Time.time;
		foreach (Transform child in transform) {
			EnergyWhipParticle particle = child.GetComponent<EnergyWhipParticle>();
			if (particle) {
				particle.friendly = friendly;
				particles.Add(particle);
			}
		}

		// Flicker
		while (true) {
			int alpha = flickerOn ? 255 : 0;
			foreach (EnergyWhipParticle particle in particles) {
				SpriteRenderer renderer = particle.gameObject.GetComponent<SpriteRenderer>();
				Color color = renderer.color;
				color.a = alpha;
				renderer.color = color;
			}
			flickerOn = !flickerOn;
			yield return new WaitForSeconds(0.02f);
		}

	}

	public override void init()
	{
		base.init();
		if (faceRight) {
			transform.localScale = new Vector3(-1, 1, 1);
		}
	}

	void Update() {
		float portionCompleted = (Time.time - startTime) / 0.5f;
		float angle = portionCompleted * 90;
		if (faceRight) {
			angle *= -1;
		}
		transform.localEulerAngles = new Vector3 (0,0,angle);

		if (!permanent && portionCompleted > 1) {
			Destroy(gameObject);
		}
	}

}
