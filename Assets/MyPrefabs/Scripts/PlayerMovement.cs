﻿using GoogleARCore.HelloAR;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float MovementSpeed = 1.0f;
    public int MaxTurnSpeed = 180; // Degrees / second
    public float DistanceScaler = 0.104f;
    
    private Queue<Vector3> _directions;
    private bool _isMoving, _isTurning;
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

    private HelloARController arCon;
    private int playerPosX = 0;
    private int playerPosZ = 0;
    private int[,] boardItemsArray;

    // Use this for initialization
    void Start () {
	    _currentTarget = transform.position;
        _directions = new Queue<Vector3>();
        angleTarget0 = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, transform.rotation.z);
        angleTarget90 = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y -90, transform.rotation.z);
        angleTarget180 = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y + 180, transform.rotation.z);
        angleTarget270 = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y + 90, transform.rotation.z);
        GameObject sceneController = GameObject.Find("SceneController");
        arCon = sceneController.GetComponent<HelloARController>();
        boardItemsArray = arCon.boardItemsArray;
    }
	
	// Update is called once per frame
	void Update () {
        // Dont pop if we have nothing to pop, or are in the process of changing position or rotation
	    if (IsExecuting && !_isMoving && !_isTurning)
	    {
	        // pop - get next target positions
	        var temp = _directions.Dequeue();
            // Scale length of movement
            Vector3 playPos = transform.position;
            Vector3 newPos;

            if (temp.x == 1)
            {
                if (playerPosX < boardItemsArray.GetLength(0)-1 && boardItemsArray[playerPosX+1,playerPosZ]!=2)
                {
                    playerPosX++;
                    angleTarget = angleTarget0;
                    newPos = arCon.tilesArray[playerPosX, playerPosZ].transform.position;
                    _currentTarget = new Vector3(newPos.x, transform.position.y,newPos.z);
                }
            }
            else if (temp.x == -1)
            {
                if (playerPosX > 0 && boardItemsArray[playerPosX - 1, playerPosZ] != 2)
                {
                    playerPosX--;
                    angleTarget = angleTarget180;
                    newPos = arCon.tilesArray[playerPosX, playerPosZ].transform.position;
                    _currentTarget = new Vector3(newPos.x, transform.position.y, newPos.z);
                }
            }
            else if (temp.z == 1)
            {
                if (playerPosZ < boardItemsArray.GetLength(1) - 1 && boardItemsArray[playerPosX, playerPosZ + 1] != 2)
                {
                    playerPosZ++;
                    angleTarget = angleTarget90;
                    newPos = arCon.tilesArray[playerPosX, playerPosZ].transform.position;
                    _currentTarget = new Vector3(newPos.x, transform.position.y, newPos.z);
                }
            }
            else if (temp.z == -1)
            {
                if (playerPosZ > 0 && boardItemsArray[playerPosX, playerPosZ - 1] != 2)
                {
                    playerPosZ--;
                    angleTarget = angleTarget270;
                    newPos = arCon.tilesArray[playerPosX, playerPosZ].transform.position;
                    _currentTarget = new Vector3(newPos.x, transform.position.y, newPos.z);
                }
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

    // Start moving the player in the given directions. Y should always be 0
    public void MovePlayer(Queue<Vector3> directions)
    {
        //float turnAngle = 90;
        //transform.rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y + turnAngle, transform.rotation.z);
        // Copy the stack to _directions. Looks silly, but is neccesary to keep the order
        _directions = new Queue<Vector3>(new Queue<Vector3>(directions));
        IsExecuting = true;
    }
}

