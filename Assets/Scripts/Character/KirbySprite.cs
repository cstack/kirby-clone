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

	public void OnTriggerEnter2D(Collider2D other) {
		if (kirby.CurrentState.ToString() == Kirby.State.Inhaling.ToString()) {
			if (other.gameObject.tag == "enemy") {
				kirby.enemyCollisionCallback(other.gameObject);
			} else if (other.gameObject.tag == "abilitystar") {
				AbilityStar star = other.gameObject.GetComponent<AbilityStar>();
				kirby.InhaleAbility(star.ability, star.abilityCard);
			}
		}
	}
}
