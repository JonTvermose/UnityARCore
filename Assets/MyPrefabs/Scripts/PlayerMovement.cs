﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float MovementSpeed = 1.0f;
    public int MaxTurnSpeed = 180; // Degrees / second

    private Stack<Vector3> _directions;
    private bool _isMoving, _isTurning;
    private Vector3 _currentTarget;
    private int _maxAngleDelta = 2; // Maximum error acceptable between 

	// Use this for initialization
	void Start () {
	    _currentTarget = transform.position;
        _directions = new Stack<Vector3>();
        // Test code
        //Stack<Vector3> stack = new Stack<Vector3>();
        //stack.Push(new Vector3(-5, 0, 0));
        //stack.Push(new Vector3(0, 0, 5));
        //stack.Push(new Vector3(4, 0, 0));
        //MovePlayer(stack);
    }
	
	// Update is called once per frame
	void Update () {
        // Dont pop if we have nothing to pop, or are in the process of changing position or rotation
	    if (_directions.Count > 0 && !_isMoving && !_isTurning)
	    {
	        // pop - get next target positions
	        _currentTarget += _directions.Pop();

            // init turn
	        _isTurning = true;

            // init movement and pause pop
	        _isMoving = false;
	    }
	    if (_isTurning)
	    {
            // Animate turning towards _currentTarget
	        Quaternion wantedRotation = Quaternion.LookRotation(transform.position - _currentTarget);
	        wantedRotation.eulerAngles += new Vector3(0, -90, 0);
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
	        gameObject.transform.position =
	            Vector3.Lerp(gameObject.transform.position, _currentTarget, MovementSpeed * Time.deltaTime);

            // Check if we reached our target position
	        if (Vector3.Distance(transform.position, _currentTarget) < 0.1f)
	        {
	            _isMoving = false;
	        }
	    }
	}

    // Start moving the player in the given directions. Y should always be 0
    public void MovePlayer(Stack<Vector3> directions)
    {
        _directions = directions;
    }
}
