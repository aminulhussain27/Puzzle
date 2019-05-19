using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Handle the map
public class GridManager : MonoBehaviour 
{
	private static GridManager instance = null;
	public static GridManager Instance { get { return instance; } }

	public int totalCoinCount;

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(this.gameObject);
		} 
		else 
		{
			instance = this;
		}
	}


	void Start () 
	{
		//Geting the total coin present in the mission
		totalCoinCount = transform.Find("CoinParent").transform.childCount;

		GameManager.Instance.UpdateCoinCollection (0);
	}
		
}
