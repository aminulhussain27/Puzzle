using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour 
{
	private static GridManager instance = null;


	public static GridManager Instance { get { return instance; } }


	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(this.gameObject);
		} else {
			instance = this;
		}
	}

	public int totalCoinCount;
	public GameObject exitDoor;
	void Start () 
	{
		totalCoinCount = transform.Find("CoinParent").transform.childCount;
		Debug.LogError (totalCoinCount);
		GameManager.Instance.UpdateCoinCollection (0);
		exitDoor.SetActive (false);
	}

	public void ShowGameOverPopUp(bool WinOrLoose)
	{
		if (WinOrLoose) 
		{
			Debug.LogError ("Win");
		}
		else
		{
			Debug.LogError ("Loose");
		}
	}

}
