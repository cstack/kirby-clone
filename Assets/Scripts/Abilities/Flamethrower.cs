using UnityEngine;
using System.Collections;

public class Flamethrower : Ability {
	public FlameProjectile flameProjectilePrefab;
	public float duration = 2f;
	public float flamesPerSecond = 20f;
	public float angle = 15f;

	#region implemented abstract members of Ability
	
	public override float getDuration()
	{
		return duration;
	}
	
	#endregion
	
	// Use this for initialization
	public void Start () {
		StartCoroutine(ShootFlames());
	}
	
	private IEnumerator ShootFlames() {
		float timestep = 1 / flamesPerSecond;
		while (true) {
			ShootFlame(angle);
			yield return new WaitForSeconds(timestep);
			ShootFlame(0);
			yield return new WaitForSeconds(timestep);
			ShootFlame(-1*angle);
			yield return new WaitForSeconds(timestep);
		}
	}
	
	private void ShootFlame(float angle) {
		// Shoot a flame at `angle` degrees
		angle += 90 * (faceRight ? -1 : 1);
		FlameProjectile flame = Instantiate (flameProjectilePrefab) as FlameProjectile;
		flame.transform.position = transform.position - new Vector3(0f, 0.5f, 0f);
		if (faceRight) {
			flame.transform.position = flame.transform.position + new Vector3(1f, 0f, 0f);
			flame.transform.localScale = new Vector3(-1f, 1f, 1f);
		} else {
			flame.transform.position = flame.transform.position - new Vector3(1f, 0f, 0f);
		}
		flame.friendly = friendly;
		
		Vector2 direction = Quaternion.AngleAxis (angle, Vector3.forward) * Vector2.up;
		flame.rigidbody2D.velocity = direction * 10f;
	}
}
