﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InputController : MonoBehaviour {

    public GameObject[] DirectionButtons;
    private Text[] _numberTexts;
    private Queue<Vector3> _directions;
    private int _repeats;
    public GameObject RepeatButton;
    private Text _repeatText;

    private GameObject _player;

    // Use this for initialization
    void Start () {
        _directions = new Queue<Vector3>();
        _numberTexts = new Text[DirectionButtons.Length];
        _repeats = 0;
        for (int i = 0; i < DirectionButtons.Length; i++)
        {
            _numberTexts[i] = DirectionButtons[i].GetComponentInChildren<Text>();
        }
        _repeatText = RepeatButton.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update () {
	    if (_player == null)
	    {
	        _player = GameObject.FindGameObjectWithTag("Player");
        }
	}

    public void RepeatMoves()
    {
        _repeats++;
        _repeatText.text = "Gentag sekvens (" + _repeats + ")";
    }

    public void AddMove(int direction)
    {
        if (_player != null)
        {
            if (_player.GetComponent<PlayerMovement>().IsExecuting)
                return;
        }
        if (GameManager.GameManager_instance.GameEnded)
            return;

        // Add direction vector
        Vector3 dirVector;
        switch (direction)
        {
            case 1: dirVector = new Vector3(0, 0, 1); break;
            case 2: dirVector = new Vector3(1, 0, 0); break;
            case 3: dirVector = new Vector3(0, 0, -1); break;
            case 4: dirVector = new Vector3(-1, 0, 0); break;
            default: dirVector = new Vector3(0, 0, 0); break;
        }
        _directions.Enqueue(dirVector);

        // Update the text on button
        int amountLeft = int.Parse(_numberTexts[direction - 1].text) + 1;
        _numberTexts[direction - 1].text = amountLeft.ToString();
    }

    /// <summary>
    /// Retrieve the total number of moves made by the player, since the last "ResetMoves()" has been called.
    /// </summary>
    public int TotalMoves()
    {
        int total = _numberTexts.Sum(x => int.Parse(x.text));
        return total;
    }

    /// <summary>
    /// Reset the number of moves the player has made.
    /// </summary>
    public void ResetMoves()
    {
        foreach (var numberText in _numberTexts)
        {
            numberText.text = "0";
        }
    }

    public void ExecuteMoves()
    {
        var temp = _player.GetComponent<PlayerMovement>();
        if (temp != null)
        {
            temp.DoneExecutingHandler += ExecuteHandler;

            if (_repeats > 0)
            {
                // Copy queue
                var tempQueue = new Queue<Vector3>(new Queue<Vector3>(_directions));
                // Repeat instructions
                for (int i = 0; i < _repeats; i++)
                {
                    foreach (var dir in tempQueue)
                    {
                        _directions.Enqueue(dir);
                    }
                }
            }

            temp.MovePlayer(_directions);
            // Clear stack as the stack will be copied in PlayerMovement
            _directions.Clear();
            _repeats = 0;
        }
        SetButtonsInteractable(false);
    }

    public void SetButtonsInteractable(bool value)
    {
        foreach (GameObject button in DirectionButtons)
        {
            button.GetComponent<Button>().interactable = value;
        }
    }

    public void ExecuteHandler(object sender, EventArgs args)
    {
        SetButtonsInteractable(true);
    }
}
