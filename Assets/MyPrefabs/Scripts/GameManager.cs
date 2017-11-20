using GoogleARCore.HelloAR;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GameManager_instance;

    private InputController _inputController;
    
    private PlayerMovement _playerMovement;
    private int _score;
    private int _pickups;
    public Renderer _rend;
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
	    _playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
	    _pickups = 0;
	    _score = 0;
        
        
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void EndGame()
    {
        GameEnded = true;
        _score = (50 - _inputController.TotalMoves()) * _pickups;
        SetHighScore(_score.ToString());
        Invoke("Quit", 5);
    }

    private void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Saves the highscore on the phone and shows a toast message to the user by calling the method "setHighScore(string score)" in android
    /// </summary>
    private void SetHighScore(string highScore)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            unityActivity.Call("setHighScore", highScore);
        }
    }

    public void PickupTrigger()
    {
        _pickups++;

        float cl = 1f - (_pickups / 4f);
        ColorTiles(cl);
        ColorObstacles(cl);
        ColorPickups(cl);
    }

    private void ColorTiles(float colorLevel)
    {
        foreach (var tile in GameObject.FindGameObjectsWithTag("Tile"))
        {
            _rend = tile.GetComponent<Renderer>();
            _rend.material.shader = Shader.Find("Unlit/GrayscaleTexture");
            _rend.material.SetFloat("_ColorLevel", colorLevel);
        }
    }

    private void ColorObstacles(float colorLevel)
    {
        foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            _rend = obstacle.GetComponent<Renderer>();
            foreach (Material mat in _rend.materials)
            {
                mat.shader = Shader.Find("Unlit/GrayscaleColor");
                mat.SetFloat("_ColorLevel", colorLevel);
            }
        }
    }

    private void ColorPickups(float colorLevel)
    {
        foreach (GameObject pickup in GameObject.FindGameObjectsWithTag("Pickup"))
        {
            _rend = pickup.GetComponent<Renderer>();
            foreach (Material mat in _rend.materials)
            {
                mat.shader = Shader.Find("Unlit/GrayscaleColor");
                mat.SetFloat("_ColorLevel", colorLevel);
            }
        }
    }
}
