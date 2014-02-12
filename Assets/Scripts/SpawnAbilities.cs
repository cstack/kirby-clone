using UnityEngine;
using System.Collections;

public class SpawnAbilities : MonoBehaviour {

	public AbilityStar starPrefab;

	IEnumerator Start() {
		while (true) {
			AbilityStar star = Instantiate(starPrefab) as AbilityStar;
			star.transform.position = transform.position - new Vector3(0f, 0f, transform.position.z);
			int val = Random.Range(0, 3);
			Debug.Log("Random star " + val);
			if (val == 0) {
				star.ability = star.energyWhip;
				star.abilityCard = star.energyWhipCard;
				star.transform.position += new Vector3(0f, 5f, 0f);
			} else if (val == 1) {
				star.ability = star.spark;
				star.abilityCard = star.sparkCard;
				star.transform.position += new Vector3(-3f, 3f, 0f);
			} else if (val == 2) {
				star.ability = star.flameThrower;
				star.abilityCard = star.flameThrowerCard;
				star.transform.position += new Vector3(3f, 3f, 0f);
			}

			yield return new WaitForSeconds(3f);
		}
	}
}
