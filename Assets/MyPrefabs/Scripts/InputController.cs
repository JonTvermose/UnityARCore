using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputController : MonoBehaviour {

    public GameObject[] DirectionButtons;
    private Text[] _numberTexts;
    private Stack<Vector3> _directions;

    private GameObject _player;

    // Use this for initialization
    void Start () {
        _directions = new Stack<Vector3>();
        _numberTexts = new Text[DirectionButtons.Length];
        for (int i = 0; i < DirectionButtons.Length; i++)
        {
            _numberTexts[i] = DirectionButtons[i].GetComponentInChildren<Text>();
        }
        _directions.Push(new Vector3(1, 0, 0));
        _directions.Push(new Vector3(0, 0, 1));
        _directions.Push(new Vector3(1, 0, 0));
        _directions.Push(new Vector3(1, 0, 0));
        _directions.Push(new Vector3(0, 0, 1));
    }

    // Update is called once per frame
    void Update () {
	    if (_player == null)
	    {
	        _player = GameObject.FindGameObjectWithTag("Player");
            ExecuteMoves();
	    }
	}

    public void AddMove(int direction)
    {
        // Add direction vector
        Vector3 dirVector;
        switch (direction)
        {
            case 1: dirVector = new Vector3(1, 0, 0); break;
            case 2: dirVector = new Vector3(0, 0, 1); break;
            case 3: dirVector = new Vector3(-1, 0, 0); break;
            case 4: dirVector = new Vector3(0, 0, -1); break;
            default: dirVector = new Vector3(0, 0, 0); break;
        }
        _directions.Push(dirVector);

        // Update the text on button
        int amountLeft = int.Parse(_numberTexts[direction - 1].text) - 1;

        // Deactivate button if there no more moves
        if (amountLeft <= 0)
        {
            DirectionButtons[direction - 1].GetComponent<Button>().interactable = false;
            _numberTexts[direction - 1].text = "0";
        }
        else
        {
            // Reduce the amount with 1
            _numberTexts[direction - 1].text = amountLeft.ToString();
        }
    }

    public void ExecuteMoves()
    {
        var temp = _player.GetComponent<PlayerMovement>();
        if(temp != null)
            temp.MovePlayer(_directions);
    }
}
