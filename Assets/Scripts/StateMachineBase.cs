using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class StateMachineBase : MonoBehaviour {

	public String CurrentStateName;

	public AnimationManager am;

	private Dictionary<string, Delegate> delegateCache = new Dictionary<string, Delegate>();
	private Action DoUpdate = DoNothing;
	private Action DoLateUpdate = DoNothing;
	private Action DoFixedUpdate = DoNothing;
	private Action<Collider> DoOnTriggerEnter = DoNothingCollider;
	private Action<Collider> DoOnTriggerStay = DoNothingCollider;
	private Action<Collider> DoOnTriggerExit = DoNothingCollider;
	private Action<Collider2D> DoOnTriggerEnter2D = DoNothingCollider2D;
	private Action<Collider2D> DoOnTriggerStay2D = DoNothingCollider2D;
	private Action<Collider2D> DoOnTriggerExit2D = DoNothingCollider2D;
	private Action<Collision> DoOnCollisionEnter = DoNothingCollision;
	private Action<Collision> DoOnCollisionStay = DoNothingCollision;
	private Action<Collision> DoOnCollisionExit = DoNothingCollision;
	private Action<Collision2D> DoOnCollisionEnter2D = DoNothingCollision2D;
	private Action<Collision2D> DoOnCollisionStay2D = DoNothingCollision2D;
	private Action<Collision2D> DoOnCollisionExit2D = DoNothingCollision2D;
	private Action DoOnMouseEnter = DoNothing;
	private Action DoOnMouseUp = DoNothing;
	private Action DoOnMouseDown = DoNothing;
	private Action DoOnMouseOver = DoNothing;
	private Action DoOnMouseExit = DoNothing;
	private Action DoOnMouseDrag = DoNothing;
	private Action DoOnGUI = DoNothing;
	private Func<IEnumerator> ExitState = DoNothingCoroutine;

	private Enum currentState;

	private SpriteRenderer spriteRenderer;
	private BoxCollider2D boxCollider2D;

	static IEnumerator DoNothingCoroutine() {
		yield break;
	}
	
	static void DoNothing() {
	}
	
	static void DoNothingCollider(Collider other) {
	}
	
	static void DoNothingCollision(Collision other) {
	}

	static void DoNothingCollision2D(Collision2D other) {
	}

	static void DoNothingCollider2D(Collider2D other) {
	}

	void Awake() {
		boxCollider2D  = GetComponent<BoxCollider2D>();
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		am             = new AnimationManager(GetComponentInChildren<Animator>());
	}
	
	public Enum CurrentState {
		get {
			return currentState;
		}
		set {
			currentState = value;
			am.State = value;
			ConfigureCurrentState();
			CurrentStateName = currentState.ToString();
		}
	}

	/**
	 * Sets all delegates for new state
	 */
	void ConfigureCurrentState() {
		if (ExitState != null) {
			StartCoroutine(ExitState());
		}

		DoUpdate             = ConfigureDelegate<Action>("Update", DoNothing);
		DoOnGUI              = ConfigureDelegate<Action>("OnGUI", DoNothing);
		DoLateUpdate         = ConfigureDelegate<Action>("LateUpdate", DoNothing);
		DoFixedUpdate        = ConfigureDelegate<Action>("FixedUpdate", DoNothing);
		DoOnMouseUp          = ConfigureDelegate<Action>("OnMouseUp", DoNothing);
		DoOnMouseDown        = ConfigureDelegate<Action>("OnMouseDown", DoNothing);
		DoOnMouseEnter       = ConfigureDelegate<Action>("OnMouseEnter", DoNothing);
		DoOnMouseExit        = ConfigureDelegate<Action>("OnMouseExit", DoNothing);
		DoOnMouseDrag        = ConfigureDelegate<Action>("OnMouseDrag", DoNothing);
		DoOnMouseOver        = ConfigureDelegate<Action>("OnMouseOver", DoNothing);
		DoOnTriggerEnter     = ConfigureDelegate<Action<Collider>>("OnTriggerEnter", DoNothingCollider);
		DoOnTriggerExit      = ConfigureDelegate<Action<Collider>>("OnTriggerExit", DoNothingCollider);
		DoOnTriggerStay      = ConfigureDelegate<Action<Collider>>("OnTriggerEnter", DoNothingCollider);
		DoOnTriggerEnter2D   = ConfigureDelegate<Action<Collider2D>>("OnTriggerEnter2D", DoNothingCollider2D);
		DoOnTriggerExit2D    = ConfigureDelegate<Action<Collider2D>>("OnTriggerExit2D", DoNothingCollider2D);
		DoOnTriggerStay2D    = ConfigureDelegate<Action<Collider2D>>("OnTriggerStay2D", DoNothingCollider2D);
		DoOnCollisionEnter   = ConfigureDelegate<Action<Collision>>("OnCollisionEnter", DoNothingCollision);
		DoOnCollisionExit    = ConfigureDelegate<Action<Collision>>("OnCollisionExit", DoNothingCollision);
		DoOnCollisionStay    = ConfigureDelegate<Action<Collision>>("OnCollisionStay", DoNothingCollision);
		DoOnCollisionEnter2D = ConfigureDelegate<Action<Collision2D>>("OnCollisionEnter2D", DoNothingCollision2D);
		DoOnCollisionExit2D  = ConfigureDelegate<Action<Collision2D>>("OnCollisionExit2D", DoNothingCollision2D);
		DoOnCollisionStay2D  = ConfigureDelegate<Action<Collision2D>>("OnCollisionStay2D", DoNothingCollision2D);

		Func<IEnumerator> enterState = ConfigureDelegate<Func<IEnumerator>>("EnterState", DoNothingCoroutine);
		ExitState = ConfigureDelegate<Func<IEnumerator>>("ExitState", DoNothingCoroutine);

		EnableGUI();
		StartCoroutine(enterState());
	}

	/**
	 * Uses reflection to get delegate method for state. Naming convention is StateMethod.
	 */
	T ConfigureDelegate<T>(string methodName, T Default) where T : class {
		string delegateName = currentState.ToString() + methodName;
		Delegate d;

		// Check the cache first
		delegateCache.TryGetValue(delegateName, out d);

		if (d == null) {
			// Delegate wasn't found in cache, so use reflection
			var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public |
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod;
			var method = GetType().GetMethod(delegateName, flags);
			if (method != null) {
				d = Delegate.CreateDelegate(typeof(T), this, method);
			} else {
				d = Default as Delegate;
			}
			delegateCache.Add(delegateName, d);
		}
		return d as T;
	}
	
	protected void EnableGUI() {
		useGUILayout = DoOnGUI != DoNothing;
	}
	
	public void Update() {
		// Update box collider size based on sprite
		/*Vector3 size = spriteRenderer.bounds.size;
		Vector2 newSize = new Vector2(size.x, size.y);
		boxCollider2D.size = newSize;*/

		DoUpdate();
	}
	
	void LateUpdate() {
		DoLateUpdate();
	}
	
	void OnMouseEnter() {
		DoOnMouseEnter();
	}
	
	void OnMouseUp() {
		DoOnMouseUp();
	}
	
	void OnMouseDown() {
		DoOnMouseDown();
	}
	
	void OnMouseExit() {
		DoOnMouseExit();
	}
	
	void OnMouseDrag() {
		DoOnMouseDrag();
	}
	
	void FixedUpdate() {
		DoFixedUpdate();
	}

	void OnTriggerEnter(Collider other) {
		DoOnTriggerEnter(other);
	}

	public void OnTriggerExit(Collider other) {
		DoOnTriggerExit(other);
	}

	public void OnTriggerStay(Collider other) {
		DoOnTriggerStay(other);
	}

	public void OnTriggerEnter2D(Collider2D other) {
		DoOnTriggerEnter2D(other);
	}
	
	public void OnTriggerExit2D(Collider2D other) {
		DoOnTriggerExit2D(other);
	}
	
	void OnTriggerStay2D(Collider2D other) {
		DoOnTriggerStay2D(other);
	}

	void OnCollisionEnter(Collision other) {
		DoOnCollisionEnter(other);
	}

	void OnCollisionExit(Collision other) {
		DoOnCollisionExit(other);
	}

	void OnCollisionStay(Collision other) {
		DoOnCollisionStay(other);
	}

	public void OnCollisionEnter2D(Collision2D other) {
		DoOnCollisionEnter2D(other);
	}
	
	void OnCollisionExit2D(Collision2D other) {
		DoOnCollisionExit2D(other);
	}
	
	void OnCollisionStay2D(Collision2D other) {
		DoOnCollisionStay2D(other);
	}

	void OnGUI() {
		DoOnGUI();
	}
}
