using UnityEngine;
using System.Collections;

public class EnergyWhip : Ability {

	public EnergyWhipParticle particlePrefab;
	public float duration = 0.75f;

	private float startTime;

	#region implemented abstract members of Ability

	public override float getDuration()
	{
		return duration;
	}

	#endregion

	// Use this for initialization
	void Start () {
		startTime = Time.time;
		foreach (Transform child in transform) {
			child.GetComponent<EnergyWhipParticle>().friendly = friendly;
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
		float portionCompleted = (Time.time - startTime) / duration;
		float angle = portionCompleted * 90;
		if (faceRight) {
			angle *= -1;
		}
		transform.localEulerAngles = new Vector3 (0,0,angle);
	}

}
