using UnityEngine;
using System.Collections;

public class SparkProjectile : Projectile {
	public Vector3 origin;
	public float radius = 3f;

	public void Update() {
		if (Vector2.Distance(transform.position + new Vector3(0.5f,0.5f,0), origin) > radius) {
			Destroy(gameObject);
		}
	}

}
