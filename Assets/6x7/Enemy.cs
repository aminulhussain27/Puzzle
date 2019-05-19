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


	void Start () 
	{
		if(enemyType == EnemyType.PATROLLER)
		{
			InvokeRepeating ("RandomMoveForPatroller", 3, 2);
		}
		if(enemyType == EnemyType.FOLLWER)
		{
			InvokeRepeating ("FollowPlayer", 2, 1);
		}

	}
	int randomMoveX;
	int randomMoveY;

	int patrolMove = 1;
	int ReverseMovememt()
	{
		Vector2 startCell = transform.position;
		Vector2 targetCell = startCell + new Vector2 (0, patrolMove);
		bool hasGroundTile = getCell (groundTilemap, targetCell) != null;

		if (!hasGroundTile) 
		{
			patrolMove *= -1;
		}
			return patrolMove;

	}
	void RandomMoveForPatroller()
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


	void FollowPlayer()
	{
		if(GameManager.Instance.isGameOver)
		{
			CancelInvoke ();
		}

		Vector3 targetCoordinate = FindPlayerDirection ();

		bool hasGroundTile = getCell (groundTilemap, transform.position + targetCoordinate) != null; //If target Tile has a ground
		bool hasObstacleTile = getCell (obstaclesTilemap, transform.position + targetCoordinate) != null; //if target Tile has an obstacle

		if(!hasGroundTile || hasObstacleTile)
		{
		
			Debug.LogError (targetCoordinate);

		}


		Move (targetCoordinate);
	}

	Vector2 FindPlayerDirection()
	{
		Vector2 distaceVector = player.transform.position - transform.position;
		Vector2 whereToMove = Vector2.zero;

		if(Mathf.Abs(distaceVector.x) > Mathf.Abs( distaceVector.y))
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
		else if(Mathf.Abs(distaceVector.x) < Mathf.Abs(distaceVector.y))
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
		else if(distaceVector.x != 0)
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
		else if(distaceVector.y != 0)
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
//		Debug.LogError (distaceVector +"    " + whereToMove);
		return whereToMove;
	}

	private IEnumerator SmoothMovement(Vector3 end)
	{
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
		float inverseMoveTime = 1 / 0.1f;

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
