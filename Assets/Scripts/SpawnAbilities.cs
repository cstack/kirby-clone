using UnityEngine;
using System.Collections;

public class SpawnAbilities : MonoBehaviour {

	public AbilityStar starPrefab;

	IEnumerator Start() {
		while (true) {
			AbilityStar star = Instantiate(starPrefab) as AbilityStar;
			int val = Random.Range(0, 2);
			if (val == 0) {
				star.ability = star.energyWhip;
				star.abilityCard = star.energyWhipCard;
			} else if (val == 1) {
				star.ability = star.spark;
				star.abilityCard = star.sparkCard;
			} else if (val == 2) {
				star.ability = star.flameThrower;
				star.abilityCard = star.flameThrowerCard;
			}
			Debug.Log(star.ability);
			star.transform.position = transform.position - new Vector3(0f, 0f, transform.position.z);
			float xOffset = camera.pixelWidth / 16 * (Random.value - 0.5f);
			star.transform.position += new Vector3(xOffset, 0f, 0f);
			star.transform.position += new Vector3(0f, 3f, 0f);
			yield return new WaitForSeconds(3f);
		}
	}
}
