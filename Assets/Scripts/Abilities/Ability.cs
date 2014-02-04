using UnityEngine;
using System.Collections;

public abstract class Ability : MonoBehaviour {
	public bool faceRight = false;
	public bool friendly = false;
	public abstract float getDuration();
	public virtual void init() {}
}
