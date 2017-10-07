using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Moves : MonoBehaviour
{
    public GameObject Player;
    public GameObject[] DirectionButtons;
    private Text[] _numberTexts;
    
    private Stack<Vector3> _directionStack;
    private PlayerMovement _playerMovement;

	// Use this for initialization
	void Start () {
	    _directionStack = new Stack<Vector3>();
	    _playerMovement = Player.GetComponent<PlayerMovement>();
	    _numberTexts = new Text[DirectionButtons.Length];
	    for (int i=0; i<DirectionButtons.Length; i++)
	    {
	        _numberTexts[i] = DirectionButtons[i].GetComponentInChildren<Text>();
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
        _directionStack.Push(dirVector);

        // Update the text on button
        int amountLeft = int.Parse(_numberTexts[direction - 1].text) - 1;
        _numberTexts[direction - 1].text = amountLeft.ToString();

        // Deactivate button if there no more moves
        if (amountLeft == 0)
        {
            DirectionButtons[direction - 1].GetComponent<Button>().interactable = false;
        }
    }

    public void ExecuteMoves()
    {
        _playerMovement.MovePlayer(_directionStack);
    }
}
