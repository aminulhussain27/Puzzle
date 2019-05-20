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


	public Button playButton;
	public Button quitButton;
	public Button upButton;
	public Button downButton;
	public Button leftButton;
	public Button rightButton;

	public Text coinCollectedText;

	public int currentLevel;

	public bool isGameOver;

	private GameObject tileMapObject;//Need to Instantiate tileMap

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
		//Initializing the default data
		isGameOver = false;
		currentLevel = 0;

		//Button action for buttons in Main menu panel
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

	//this will start when game is over Either win or loose
	public IEnumerator ShowGameOverPanelWithDelay(bool isWon)
	{
		isGameOver = true;//Making game status as gameover
		SoundManager.Instance ().playSound (SoundManager.SOUND_ID.SFX_END);//Game over sound

		yield return new WaitForSeconds (1.1f);//waiting for some time to show the main panel
		if (isWon) 
		{
			mainMenuPanel.SetActive (true);
			mainMenuPanel.transform.Find("WinLooseText").GetComponent<Text>().text = "Mission Complete! \n Play Next Mission";
			//Winning the level Increasing the current level index to give tough level next
			currentLevel++;
		}
		else
		{
			mainMenuPanel.SetActive (true);
			mainMenuPanel.transform.Find("WinLooseText").GetComponent<Text>().text = "Mission Failed! \n Please try again";

			//keeping the current level index as minimum 0
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

	//Loading the map
	public void LoadLevel()
	{
		//Making the game status is not gameover as it starting from here
		isGameOver = false;
		if(currentLevel == levelTileMap.Length)
		{
			currentLevel = Random.Range(0, levelTileMap.Length);
		}

		tileMapObject = GameObject.Instantiate (levelTileMap [currentLevel]);
		mainMenuPanel.SetActive (false);
		SoundManager.Instance ().playSound (SoundManager.SOUND_ID.LOOP_BACKGROUND, 0.5f);
	}

	//Updaing UI when coin is being collected
	public void UpdateCoinCollection(int coinCollected)
	{
		coinCollectedText.text =coinCollected.ToString() + "/"+ GridManager.Instance.totalCoinCount.ToString();
		if(coinCollected == GridManager.Instance.totalCoinCount)
		{
			//All the coins collected, So giving some congratulation text
			coinCollectedText.text = "Well done! \n All coin collected.";
			GridManager.Instance.exitDoorAnim.enabled = true;
		}
	}
}
