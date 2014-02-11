using UnityEngine;
using System.Collections;

public class SparkProjectile : Projectile {
	public float delta = 0.001f;

	private float startTime;

	void Start() {
		startTime = Time.time;
	}

	public void Update() {
		if (Time.time - delta > startTime) {
			Destroy(gameObject);
		}
	}

}
