using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;	
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour {

	public EnemyType enemyType;
	public Player player;

	public Tilemap groundTilemap;
	public Tilemap obstaclesTilemap;

	int randomMoveX;
	int randomMoveY;
	int patrolMove = 1;

	void Start () 
	{
		if(enemyType == EnemyType.PATROLLER)
		{
			//Patrolling taking steps at 2 sec gap
			InvokeRepeating ("MovementForPatroller", 3, 2);
		}
		if(enemyType == EnemyType.FOLLWER)
		{
			//Actively following at a time gap of 1.4
			InvokeRepeating ("FollowPlayer", 2, 0.9f);
		}
	}

	#region PATROLLER
	int ReverseMovememt()
	{
		Vector2 startCell = transform.position;
		Vector2 targetCell = startCell + new Vector2 (0, patrolMove);
		bool hasGroundTile = getCell (groundTilemap, targetCell) != null;

		//If reaches the End tile, Reversing the direction
		if (!hasGroundTile) 
		{
			patrolMove *= -1;
		}
		return patrolMove;
	}

	void MovementForPatroller()
	{
		if(GameManager.Instance.isGameOver)
		{
			CancelInvoke ();
		}
		randomMoveX = 0;
		randomMoveY = 1 * ReverseMovememt();

		if(randomMoveX != 0)
		{
			randomMoveY = 0;
		}
		Move (new Vector2(randomMoveX, randomMoveY));
	}
		
	#endregion

	#region FOLLOWER

	void FollowPlayer()
	{
		//If game is over Stopping all the Invoke function
		if(GameManager.Instance.isGameOver)
		{
			CancelInvoke ();
		}

		Vector3 targetCoordinate = FindPlayerDirection ();//Getting the Player coordinate, where to move
		Move (targetCoordinate);
	}

	Vector2 FindPlayerDirection()
	{
		//Calculating the distace from player and Enemy
		Vector2 distaceVector = player.transform.position - transform.position;
		Vector2 whereToMove = Vector2.zero;

		if(Mathf.Abs(distaceVector.x) > Mathf.Abs( distaceVector.y) || Mathf.Abs(distaceVector.x) < Mathf.Abs(distaceVector.y))
		{
			//As both direction Enemy needs to move Hence randomly moving
			int randomDirection = Random.Range (1, 100);
			if (randomDirection % 2 == 0)
			{
				if (distaceVector.x > 0) 
				{
					whereToMove = new Vector2 (1, 0);
				}
				else if(distaceVector.x < 0)
				{
					whereToMove = new Vector2 (-1, 0);
				}
			}
			else
			{
				if (distaceVector.y > 0) 
				{
					whereToMove = new Vector2 (0, 1);
				}
				else if(distaceVector.y < 0)
				{
					whereToMove = new Vector2 (0, -1);
				}
			}
		}

		//In case X distance and Y distace is equal and Not on the same position as player
		else if(distaceVector.x != 0 || distaceVector.y != 0)
		{
			int randomDirection = Random.Range (1, 100);
			if (randomDirection % 2 == 0) 
			{		
				if (distaceVector.x > 0) 
				{
					whereToMove = new Vector2 (1, 0);
				} 
				else 
				{
					whereToMove = new Vector2 (-1, 0);
				}
			} 
			else 
			{
				if (distaceVector.y > 0) 
				{
					whereToMove = new Vector2 (0, 1);
				} 
				else 
				{
					whereToMove = new Vector2 (0, -1);
				}
			}
		}
//		if(enemyType == EnemyType.FOLLWER)
//			Debug.LogError (whereToMove +  "    " + distaceVector);
		return whereToMove;
	}
	#endregion

	#region LAZY_ENEMY
	//Player came inside the area
	private void OnTriggerEnter2D(Collider2D coll)
	{
		if (enemyType == EnemyType.LAZY && coll.tag == "Player")
		{
			transform.GetComponent<Animator> ().enabled = true;
			//Follow the player with some frequency of time
			InvokeRepeating ("FollowPlayer", 0.9f, 0.9f);
		}
	}
	//Is player went outside my Zone, Lets not follow him
	private void OnTriggerExit2D(Collider2D coll)
	{
		if (enemyType == EnemyType.LAZY && coll.tag == "Player") 
		{
			transform.GetComponent<Animator> ().enabled = false;
			CancelInvoke ();
		}
	}


	#endregion

	void Move(Vector2 moveDirection)
	{
		Vector2 startCell = transform.position;
		Vector2 targetCell = startCell + moveDirection;//new Vector2 (randomMoveX, randomMoveY);

		bool isOnGround = getCell (groundTilemap, startCell) != null; //If the player is on the ground

		bool hasGroundTile = getCell (groundTilemap, targetCell) != null; //If target Tile has a ground
		bool hasObstacleTile = getCell (obstaclesTilemap, targetCell) != null; //if target Tile has an obstacle

		if (hasGroundTile && !hasObstacleTile) 
		{
			StartCoroutine (SmoothMovement (targetCell));
		}
	}

	private IEnumerator SmoothMovement(Vector3 end)
	{
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
		float inverseMoveTime = 1 / 0.1f;
		//Remaining distace is greater than almost zero
		while (sqrRemainingDistance > float.Epsilon)
		{
			Vector3 newPosition = Vector3.MoveTowards(transform.position, end, inverseMoveTime * Time.deltaTime);
			transform.position = newPosition;
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;

			yield return null;
		}
	}

	private TileBase getCell(Tilemap tilemap, Vector2 cellWorldPos)
	{
		return tilemap.GetTile(tilemap.WorldToCell(cellWorldPos));
	}
	private bool hasTile(Tilemap tilemap, Vector2 cellWorldPos)
	{
		return tilemap.HasTile(tilemap.WorldToCell(cellWorldPos));
	}

	public enum EnemyType
	{
		LAZY,
		PATROLLER,
		FOLLWER,
	}
}
