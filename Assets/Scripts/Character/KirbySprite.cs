using UnityEngine;
using System.Collections;

public class KirbySprite : MonoBehaviour {

	protected Kirby kirby;
	
	public void Start() {
		kirby = (Kirby) transform.parent.gameObject.GetComponent(typeof(Kirby));
	}

	public void OnFinishedInhaling() {
		kirby.OnFinishedInhaling();
	}

	public void OnFinishedShooting() {
		kirby.OnFinishedShooting();
	}
}
