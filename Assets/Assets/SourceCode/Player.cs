using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;	

public class Player : MonoBehaviour {

    public Tilemap groundTilemap;
    public Tilemap obstaclesTilemap;

	public int coinCount = 0;

	public bool isMoving = false;

	public bool onCooldown = false;


	private int horizontal = 0;
	private int vertical = 0;

    private float moveTime = 0.1f;

    void Start () 
	{
		//Giving the 4 Directional button action
		GameManager.Instance.upButton.onClick.RemoveAllListeners ();
		GameManager.Instance.upButton.onClick.AddListener (() => {
				vertical = 1;
			});

		GameManager.Instance.downButton.onClick.RemoveAllListeners ();
		GameManager.Instance.downButton.onClick.AddListener (() => {
				vertical = -1;
			});

		GameManager.Instance.leftButton.onClick.RemoveAllListeners ();
		GameManager.Instance.leftButton.onClick.AddListener (() => {
				horizontal = -1;
			});

		GameManager.Instance.rightButton.onClick.RemoveAllListeners ();
		GameManager.Instance.rightButton.onClick.AddListener (() => {
				horizontal = 1;
			});
	}



	void Update () 
	{
        //We do nothing if the player is still moving.
		if (isMoving || onCooldown || GameManager.Instance.isGameOver ) 
			return;

        //To player move directions for Editor
		#if UNITY_EDITOR
        //To get move directions
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));
		#endif
        //We can't go in both directions at the same time making one as zero
        if ( horizontal != 0 )
            vertical = 0;
      
        //If there's a direction, we are trying to move.
        if (horizontal != 0 || vertical != 0)
        {
			//Taking a break from input To complete move animation
            StartCoroutine(actionCooldown(0.2f));
            Move(horizontal, vertical);
			horizontal = vertical = 0;
        }
	}

    private void Move(int xDir, int yDir)
    {
        Vector2 startCell = transform.position;
        Vector2 targetCell = startCell + new Vector2(xDir, yDir);

        bool isOnGround = getCell(groundTilemap, startCell) != null; //If the player is on the ground
        bool hasGroundTile = getCell(groundTilemap, targetCell) != null; //If target Tile has a ground
        bool hasObstacleTile = getCell(obstaclesTilemap, targetCell) != null; //if target Tile has an obstacle

        //If the player starts their movement from a ground tile.
		if (isOnGround) 
		{
			//If the front tile is a walkable ground tile, the player moves here.
			if (hasGroundTile && !hasObstacleTile) 
			{
				StartCoroutine (SmoothMovement (targetCell));
			} 
			else
				StartCoroutine (BlockedMovement (targetCell));
               
			if (!isMoving)
				StartCoroutine (BlockedMovement (targetCell));
		} 
    }

    private IEnumerator SmoothMovement(Vector3 end)
    {
		//If already moving lets not start one more move coroutine
        while (isMoving) 
			yield return null;
		//Player status changed to Moving
        isMoving = true;

        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        float inverseMoveTime = 1 / moveTime;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, end, inverseMoveTime * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            yield return null;
        }

        isMoving = false;
    }

    //Blocked Movement and playing Animation Sounds
    private IEnumerator BlockedMovement(Vector3 end)
    {
        //while (isMoving) yield return null;
		SoundManager.Instance ().playSound (SoundManager.SOUND_ID.SFX_COLLIDE_WITH_BLOCKER);
        isMoving = true;

        Vector3 originalPos = transform.position;

        end = transform.position + ((end - transform.position) / 3);
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        float inverseMoveTime = (1 / (moveTime*2) );

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, end, inverseMoveTime * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            yield return null;
        }

        sqrRemainingDistance = (transform.position - originalPos).sqrMagnitude;
        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, originalPos, inverseMoveTime * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - originalPos).sqrMagnitude;

            yield return null;
        }
        isMoving = false;
    }


    private IEnumerator actionCooldown(float cooldown)
    {
        onCooldown = true;

        while ( cooldown > 0f )
        {
            cooldown -= Time.deltaTime;
            yield return null;
        }
        onCooldown = false;
    }
		

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if ( coll.tag == "exitTag")
        {
			if(GridManager.Instance.totalCoinCount == coinCount)
			{
				SoundManager.Instance ().playSound (SoundManager.SOUND_ID.SFX_END);
				GameManager.Instance.isGameOver = true;
				StartCoroutine (GameManager.Instance.ShowGameOverPanelWithDelay (true));
			}
			else
			{
				SoundManager.Instance ().playSound (SoundManager.SOUND_ID.SFX_COLLIDE_WITH_BLOCKER);
			}
        }
        else if ( coll.tag == "coinTag")
        {
            coinCount++; 
			GameManager.Instance.UpdateCoinCollection (coinCount);
            coll.gameObject.SetActive(false);

			SoundManager.Instance ().playSound (SoundManager.SOUND_ID.SFX_COIN_PICKUP);
        }

		if ( coll.tag == "enemyTag")
		{

//			return;

			StartCoroutine (GameManager.Instance.ShowGameOverPanelWithDelay (false));
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

}
