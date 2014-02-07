using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {
	public GameObject[] nums = new GameObject[7];
	public Sprite[] sprites = new Sprite[10];

	public void updateScore(int score) {
		int[] digits = calcDigits(score);
		setDigits(digits);
	}

	private int[] calcDigits(int score) {
		int[] digits = new int[7];
		for (int i = 0; i < 7; i++) {
			digits[i] = score % 10;
			score /= 10;
		}
		return digits;
	}

	private void setDigits(int[] digits) {
		for (int i = 0; i < digits.Length; i++) {
			nums[i].GetComponent<SpriteRenderer>().sprite = sprites[digits[i]];
		}
	}
}
