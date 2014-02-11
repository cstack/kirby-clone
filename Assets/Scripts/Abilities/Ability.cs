using UnityEngine;
using System.Collections;

public abstract class Ability : MonoBehaviour {
	public bool faceRight = false;
	public bool friendly = false;
	public bool permanent = false;
	public virtual void init() {}
}
