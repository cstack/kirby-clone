using UnityEngine;
using System.Collections;

public class DestroyOnAwake : MonoBehaviour {

	void Awake() {
		Destroy(this.gameObject);
	}
}
