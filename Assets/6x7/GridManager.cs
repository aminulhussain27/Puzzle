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
	void Start () 
	{
		//Geting the total coin present in the mission
		totalCoinCount = transform.Find("CoinParent").transform.childCount;

		GameManager.Instance.UpdateCoinCollection (0);
	}
		
}
