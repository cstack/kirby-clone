using UnityEngine;
using System.Collections;

public class LivesRemaining : MonoBehaviour {

	public Sprite[] numberSprites;

	public void setLivesRemaining(int livesRemaining) {
		int firstDigit = livesRemaining / 10;
		int secondDigit = livesRemaining % 10;

		GameObject firstNum = transform.Find("FirstNum").gameObject;
		GameObject secondNum = transform.Find("SecondNum").gameObject;
		firstNum.GetComponent<SpriteRenderer>().sprite = numberSprites[firstDigit];
		secondNum.GetComponent<SpriteRenderer>().sprite = numberSprites[secondDigit];
	}
}
