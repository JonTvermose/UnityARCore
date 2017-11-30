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

    private bool _animateShader { get; set; }

    private bool shaderFadeUp = false;
    private bool shaderFadeDown = false;
    private bool shaderSetFinal = false;
    private float shaderFadeUpStart = 0f;
    private float shaderFadeUpLimit = 5f;
    private float shaderFadeDownStart = 5f;
    private float shaderFadeDownLimit = 0f;
    private float shaderChangeSpeed = 0.25f;
    private float shaderValue = 0;

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

        if (shaderFadeUp)
        {
            if (shaderFadeUpStart < shaderFadeUpLimit)
            {
                shaderFadeUpStart += shaderChangeSpeed;
                ColorTiles(shaderFadeUpStart);
                ColorObstacles(shaderFadeUpStart);
                ColorPickups(shaderFadeUpStart);
            } else
            {
                shaderFadeUp = false;
                shaderFadeDown = true;
            }
        }

        if (shaderFadeDown)
        {
            if (shaderFadeDownStart > shaderFadeDownLimit)
            {
                shaderFadeDownStart -= shaderChangeSpeed;
                ColorTiles(shaderFadeDownStart);
                ColorObstacles(shaderFadeDownStart);
                ColorPickups(shaderFadeDownStart);
            }
            else
            {
                shaderFadeDown = false;
                shaderSetFinal = true;
                shaderFadeDownStart = shaderFadeUpLimit;
            }
        }

        if (shaderSetFinal)
        {            
            shaderFadeUpStart = shaderValue;
            ColorTiles(shaderValue);
            ColorObstacles(shaderValue);
            ColorPickups(shaderValue);
            shaderSetFinal = false;
        }
    }

    public void EndGame()
    {
        GameEnded = true;
        _score = (50 - _inputController.TotalMoves()) * _pickups;
        SetHighScore(_score.ToString());
        Invoke("Quit", 8);
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
        shaderFadeUp = true;
        shaderValue = _pickups / 4f;
        shaderFadeDownLimit = (shaderValue);
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
