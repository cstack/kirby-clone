using UnityEngine;
using System.Collections;

public class SpawnAbilities : MonoBehaviour {

	public AbilityStar starPrefab;

	IEnumerator Start() {
		while (true) {
			AbilityStar star = Instantiate(starPrefab) as AbilityStar;
			Vector3 pos = star.transform.position;
			pos.x = camera.pixelWidth / 16 * Random.value;
			pos.y = camera.pixelHeight / 16;
			pos.z = -0.1f	;
			star.transform.position = pos;
			Debug.Log(pos);
			yield return new WaitForSeconds(3f);
		}
	}
}
