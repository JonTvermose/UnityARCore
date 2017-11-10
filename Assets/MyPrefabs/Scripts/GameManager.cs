using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GameManager_instance;

    private InputController _inputController;

    private GoalTreasure _goalTreasure;
    private PlayerMovement _playerMovement;
    private int _score;
    private int _pickups;
    public bool GameEnded { get; set; }

	// Use this for initialization
	void Start ()
	{
	    if (GameManager_instance == null)
	    {
	        GameManager_instance = this;
	        GameEnded = false;
	    }
	    else
	    {
	        Destroy(this);
	    }
	    _inputController = gameObject.GetComponent<InputController>();
	    _goalTreasure = GameObject.FindGameObjectWithTag("Treasure").GetComponent<GoalTreasure>();
	    _playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
	    _pickups = 0;
	    _score = 0;
	}
	
	// Update is called once per frame
	void Update () {
	    if (_goalTreasure != null)
	    {
	        if (_goalTreasure.IsPlaying())
	        {
	            // Game has finished
	            EndGame();
	        }
        }
	}

    public void EndGame()
    {
        GameEnded = true;
        _score = (50 - _inputController.TotalMoves()) * _pickups;
        // TODO - communicate score to Android app
    }

    public void PickupTrigger()
    {
        _pickups++;
    }
}
