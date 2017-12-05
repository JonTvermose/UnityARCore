using GoogleARCore.HelloAR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private float MovementSpeed = 2.0f;
    private int MaxTurnSpeed = 180; // Degrees / second
    public float DistanceScaler = 0.104f;
    
    private Queue<Vector3> _directions;
    private bool _isMoving, _isTurning, _wait;
    private Vector3 _currentTarget;
    private int _maxAngleDelta = 2; // Maximum error acceptable between 
    public bool IsExecuting;

    public EventHandler DoneExecutingHandler;

    private Quaternion angleTarget;
    private Quaternion angleTarget0;
    private Quaternion angleTarget90;
    private Quaternion angleTarget180;
    private Quaternion angleTarget270;
    private float turnAngle;
    private float _rockLean;


    private HelloARController arCon;
    private int playerPosX = 0;
    private int playerPosZ = 0;
    private int[,] boardItemsArray;
    private Spawner _spawner;
    private Renderer _rend;

    private int goalX = -999;
    private int goalZ = -999;

    private GameObject _treasure;
    private GameObject _obstacle;

    // Use this for initialization
    void Start ()
    {
        GameObject sceneController = GameObject.Find("SceneController");
        _spawner = sceneController.GetComponent<Spawner>();
        _currentTarget = transform.position;
        _directions = new Queue<Vector3>();
        angleTarget0 = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, transform.rotation.z);
        angleTarget90 = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y -90, transform.rotation.z);
        angleTarget180 = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y + 180, transform.rotation.z);
        angleTarget270 = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y + 90, transform.rotation.z);
    }

    // Update is called once per frame
    void Update () {
        if (GameManager.GameManager_instance.GameEnded)
            return;

        // Dont pop if we have nothing to pop, or are in the process of changing position or rotation
        if (IsExecuting && !_isMoving && !_isTurning && !_wait)
	    {
	        // pop - get next target positions
	        var temp = _directions.Dequeue();
            // Scale length of movement
            Vector3 playPos = transform.position;
            Vector3 newPos;
            int obstacle;
            int i;
            int j;

            if (temp.x == 1)
            {
                i = playerPosX + 1;                                                         // <-- new shit
                j = playerPosZ;                                                             // <-- new shit
                obstacle = boardItemsArray[i, j];
                if (playerPosX < boardItemsArray.GetLength(0) - 1 && obstacle != 2 && obstacle != 4)
                {
                    playerPosX++;
                    angleTarget = angleTarget0;
                    newPos = arCon.tilesArray[playerPosX, playerPosZ].transform.position;
                    _currentTarget = new Vector3(newPos.x, transform.position.y,newPos.z);
                }
                else if (playerPosX < boardItemsArray.GetLength(0) - 1 && obstacle == 4)    // <-- new shit
                {                                                                           // <-- new shit
                    angleTarget = angleTarget0;                                             // <-- new shit
                    _rockLean = 1;                                                          // <-- new shit
                    _obstacle = _spawner.ObstacleArray[i, j];                               // <-- new shit
                    _wait = true;                                                           // <-- new shit
                }                                                                           // <-- new shit
            }
            else if (temp.x == -1)
            {
                i = playerPosX - 1;                                                         // <-- new shit
                j = playerPosZ;
                obstacle = boardItemsArray[i, j];
                if (playerPosX > 0 && obstacle != 2 && obstacle != 4)
                {
                    playerPosX--;
                    angleTarget = angleTarget180;
                    newPos = arCon.tilesArray[playerPosX, playerPosZ].transform.position;
                    _currentTarget = new Vector3(newPos.x, transform.position.y, newPos.z);
                }
                else if (playerPosX > 0 && obstacle == 4)                                   // <-- new shit
                {                                                                           // <-- new shit
                    angleTarget = angleTarget180;                                           // <-- new shit
                    _rockLean = -1;                                                         // <-- new shit
                    _obstacle = _spawner.ObstacleArray[i, j];                               // <-- new shit
                    _wait = true;                                                           // <-- new shit
                }                                                                           // <-- new shit
            }
            else if (temp.z == 1)
            {
                i = playerPosX;                                                         // <-- new shit
                j = playerPosZ + 1;
                obstacle = boardItemsArray[i, j];
                if (playerPosZ < boardItemsArray.GetLength(1) - 1 && obstacle != 2 && obstacle != 4)
                {
                    playerPosZ++;
                    angleTarget = angleTarget90;
                    newPos = arCon.tilesArray[playerPosX, playerPosZ].transform.position;
                    _currentTarget = new Vector3(newPos.x, transform.position.y, newPos.z);
                }
                else if (playerPosZ < boardItemsArray.GetLength(1) - 1 && obstacle == 4)    // <-- new shit
                {                                                                           // <-- new shit
                    angleTarget = angleTarget90;                                            // <-- new shit
                    _rockLean = 2;                                                          // <-- new shit
                    _obstacle = _spawner.ObstacleArray[i, j];                               // <-- new shit
                    _wait = true;                                                           // <-- new shit
                }                                                                           // <-- new shit
            }
            else if (temp.z == -1)
            {
                i = playerPosX;                                                         // <-- new shit
                j = playerPosZ - 1;
                obstacle = boardItemsArray[i, j];
                if (playerPosZ > 0 && obstacle != 2 && obstacle != 4)
                {
                    playerPosZ--;
                    angleTarget = angleTarget270;
                    newPos = arCon.tilesArray[playerPosX, playerPosZ].transform.position;
                    _currentTarget = new Vector3(newPos.x, transform.position.y, newPos.z);
                }
                else if (playerPosZ > 0 && obstacle == 4)                                   // <-- new shit
                {                                                                           // <-- new shit
                    angleTarget = angleTarget270;                                           // <-- new shit
                    _rockLean = -2;                                                         // <-- new shit
                    _obstacle = _spawner.ObstacleArray[i, j];                               // <-- new shit
                    _wait = true;                                                           // <-- new shit
                }                                                                           // <-- new shit
            }

            
            
         //   temp.x = temp.x > 0.1 ? DistanceScaler : 0;
	        //temp.z = temp.z > 0.1 ? DistanceScaler : 0;

            

            //_currentTarget += temp;
            
            // init turn
            _isTurning = true;

            // init movement and pause pop
	        _isMoving = false;

	        // Stop the loop
	        if (_directions.Count == 0)
	            IsExecuting = false;
        }
	    if (_isTurning)
	    {
            // Animate turning towards _currentTarget
            Quaternion wantedRotation = Quaternion.LookRotation(transform.position - _currentTarget);
            wantedRotation.eulerAngles += new Vector3(0, -90, 0);
            wantedRotation = angleTarget;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, wantedRotation, MaxTurnSpeed * Time.deltaTime);

            // Check if player is correctly rotated
            if (Quaternion.Angle(wantedRotation, transform.rotation) < _maxAngleDelta ) 
	        {
	            _isTurning = false;
	            _isMoving = true;
	        }
	    }
        if (_isMoving)
	    {
            if (playerPosX == goalX && playerPosZ == goalZ)
            {
                _treasure.GetComponent<GoalTreasure>().TreasureFound();
                _isMoving = false;
                return;
            }
            if (_wait)
            {
                StartCoroutine(WaitFor(2f));
            }

            // Animate moving towards _currentTarget
	        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, _currentTarget, MovementSpeed * Time.deltaTime);

            // Check if we reached our target position
	        if (Mathf.Abs(Vector3.Distance(transform.position, _currentTarget)) < 0.01f)
	        {
	            _isMoving = false;
	            if (!IsExecuting && DoneExecutingHandler != null)
	            {
	                DoneExecutingHandler.Invoke(this, new EventArgs());
	            }
	        }
	    }
	}

    private IEnumerator WaitFor(float seconds)
    {
        _rend = _obstacle.GetComponent<Renderer>();
        foreach (Material mat in _rend.materials)
        {
            mat.shader = Shader.Find("Unlit/GrayColorMove");    // <-- new shit
            mat.SetFloat("_Scale", 0.75f);                          // <-- new shit
            mat.SetFloat("_Lean", _rockLean);                   // <-- new shit
        }

        yield return new WaitForSeconds(seconds);

        foreach (Material mat in _rend.materials)
        {
            mat.shader = Shader.Find("Unlit/GrayColorMove");    // <-- new shit
            mat.SetFloat("_Scale", 0);                          // <-- new shit
            mat.SetFloat("_Lean", 0);                           // <-- new shit
        }
        _wait = false;
        _isMoving = false;
        _isTurning = false;
    }

    // Start moving the player in the given directions. Y should always be 0
    public void MovePlayer(Queue<Vector3> directions)
    {
        GameObject sceneController = GameObject.Find("SceneController");
        arCon = sceneController.GetComponent<HelloARController>();
        boardItemsArray = arCon.boardItemsArray;

        for (int x = 0; x < boardItemsArray.GetLength(0); x++)
        {
            for (int z = 0; z < boardItemsArray.GetLength(1); z++)
            {
                if (boardItemsArray[x, z] == 3)
                {
                    goalX = x;
                    goalZ = z;
                }
            }
        }
        _treasure = GameObject.FindGameObjectWithTag("Treasure");

        //float turnAngle = 90;
        //transform.rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y + turnAngle, transform.rotation.z);
        // Copy the stack to _directions. Looks silly, but is neccesary to keep the order
        _directions = new Queue<Vector3>(new Queue<Vector3>(directions));
        IsExecuting = true;
    }
}
