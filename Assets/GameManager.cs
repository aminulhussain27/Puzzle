﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	

	private static GameManager instance = null;
	public static GameManager Instance { get { return instance; } }


	public GameObject controlPanel;
	public GameObject mainMenuPanel;
	public int currentLevel;
	public GameObject[] levelTileMap;

	public Button playButton;
	public Button quitButton;

	public Button upButton;
	public Button downButton;
	public Button leftButton;
	public Button rightButton;

	public Text coinCollectedText;

	private void Awake()
	{
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
		} else {
			instance = this;
		}


		playButton.onClick.RemoveAllListeners ();
		playButton.onClick.AddListener (() => {
			LoadLevel();
			});

		quitButton.onClick.RemoveAllListeners ();
		quitButton.onClick.AddListener (() => {
			Application.Quit();
		});
	}
	public void LoadLevel()
	{
		int randomNumber = Random.Range (0, 1);
		currentLevel = randomNumber;
		GameObject.Instantiate (levelTileMap [currentLevel]);
		mainMenuPanel.SetActive (false);
	}
	public void UpdateCoinCollection(int coinCollected)
	{
		coinCollectedText.text =coinCollected.ToString() + "/"+ GridManager.Instance.totalCoinCount.ToString();
		if(coinCollected == GridManager.Instance.totalCoinCount)
		{
			coinCollectedText.text = "Well done! \n All coin collected.";
		}
	}
}
