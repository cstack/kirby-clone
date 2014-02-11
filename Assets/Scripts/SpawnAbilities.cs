using UnityEngine;
using System.Collections;

public class SpawnAbilities : MonoBehaviour {

	public AbilityStar starPrefab;

	IEnumerator Start() {
		while (true) {
			AbilityStar star = Instantiate(starPrefab) as AbilityStar;
			star.transform.position = transform.position - new Vector3(0f, 0f, transform.position.z);
			float xOffset = camera.pixelWidth / 16 * (Random.value - 0.5f);
			Debug.Log("xOffset " + xOffset);
			star.transform.position += new Vector3(xOffset, 0f, 0f);
			star.transform.position += new Vector3(0f, 3f, 0f);
			Debug.Log(star.transform.position);
			yield return new WaitForSeconds(3f);
		}
	}
}
