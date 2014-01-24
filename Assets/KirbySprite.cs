using UnityEngine;
using System.Collections;

public class KirbySprite : MonoBehaviour {

	protected Kirby kirby;
	
	public void Start() {
		kirby = (Kirby) transform.parent.gameObject.GetComponent(typeof(Kirby));
	}
	public void OnFinishedInhaling() {
		Debug.Log("Setting state to " + Kirby.State.Inhaled);
		kirby.CurrentState = Kirby.State.Inhaled;
		Debug.Log("Now kirby's state is " + kirby.CurrentState);
	}
}
