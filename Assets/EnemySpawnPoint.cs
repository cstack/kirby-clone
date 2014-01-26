using UnityEngine;
using System.Collections;

public class EnemySpawnPoint : MonoBehaviour {

	public GameObject enemyPrefab;
	public GameObject enemy;

	private bool isOnScreen = false;

	private void SpawnEnemy() {
		enemy = GameObject.Instantiate(enemyPrefab) as GameObject;
		enemy.transform.position = transform.position;
	}

	private void OnEnterScreen() {
		if (enemy == null) {
			SpawnEnemy();
		}
	}

	private void CheckOnScreen() {
		bool wasOnScreen = isOnScreen;
		Vector3 positionInScreen = Camera.main.WorldToScreenPoint(transform.position);
		isOnScreen = (positionInScreen.x >= 0 && positionInScreen.x <= Camera.main.pixelWidth);

		if (!wasOnScreen && isOnScreen) {
			OnEnterScreen();
		}
	}
	
	// Update is called once per frame
	void Update () {
		CheckOnScreen();
	}
}
