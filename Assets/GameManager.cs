using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	

	private static GameManager instance = null;
	public static GameManager Instance { get { return instance; } }

	public GameObject controlPanel;
	public GameObject mainMenuPanel;

	public GameObject[] levelTileMap;
	GameObject tileMapObject;

	public Button playButton;
	public Button quitButton;
	public Button upButton;
	public Button downButton;
	public Button leftButton;
	public Button rightButton;

	public Text coinCollectedText;

	public int currentLevel;

	public bool isGameOver;

	private void Awake()
	{
		if (instance != null && instance != this) 
		{
			Destroy (this.gameObject);
		}
		else 
		{
			instance = this;
		}
	}
	void Start()
	{
		isGameOver = false;
		currentLevel = 0;

		playButton.onClick.RemoveAllListeners ();
		playButton.onClick.AddListener (() => {
			LoadLevel();
		});

		quitButton.onClick.RemoveAllListeners ();
		quitButton.onClick.AddListener (() => {

			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
			#else
			Application.Quit();
			#endif
		});
	}

	public IEnumerator ShowGameOverPanelWithDelay(bool isWon)
	{
		yield return new WaitForSeconds (1.2f);
		if (isWon) 
		{
			mainMenuPanel.SetActive (true);
			mainMenuPanel.transform.Find("WinLooseText").GetComponent<Text>().text = "Mission Complete! \n Play Next Mission";
			currentLevel++;
		}
		else
		{
			mainMenuPanel.SetActive (true);
			mainMenuPanel.transform.Find("WinLooseText").GetComponent<Text>().text = "Mission Failed! \n Please try again";

			if (currentLevel > 0) 
			{
				currentLevel--;
			}
		}

		tileMapObject.SetActive (false);

		if(tileMapObject != null)
		{
			Destroy (tileMapObject);
			tileMapObject = null;
		}
	}

	public void LoadLevel()
	{
		isGameOver = false;
		if(currentLevel > levelTileMap.Length)
		{
			currentLevel = levelTileMap.Length - 1;
		}

		tileMapObject = GameObject.Instantiate (levelTileMap [currentLevel]);
		mainMenuPanel.SetActive (false);

		Debug.Log (tileMapObject);
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
