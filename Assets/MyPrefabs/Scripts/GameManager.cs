using GoogleARCore.HelloAR;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager GameManager_instance;

    private InputController _inputController;    

    private PlayerMovement _playerMovement;
    private int _score;
    private int _pickups;
    private Renderer _rend;
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

    private float timer = 0.0f;
    private float maxTime = 20.0f;
    private bool snowIt = false;

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
                ColorObstacles(shaderFadeUpStart,1);
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
                ColorObstacles(shaderFadeDownStart,1);
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
            ColorObstacles(shaderValue,0);
            ColorPickups(shaderValue);
            shaderSetFinal = false;
        }

        if (snowIt)
        {
            timer += Time.deltaTime;
            SnowManager();
            if (timer > maxTime)
            {
                snowIt = false;
            }
        } else if (timer > 0)
        {
            timer -= Time.deltaTime * 4; // fadeout a lot faster
            SnowManager();
        }
    }

    private void SnowManager()
    {
        var snowLevel = timer / maxTime;
        if (snowLevel > 0.9f)
            snowLevel = 0.9f; // Limit the maximum value
        SnowTiles(snowLevel);
        SnowObstacles(snowLevel);
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

        snowIt = true;
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

    private void SnowTiles(float snowLevel)
    {
        foreach (var tile in GameObject.FindGameObjectsWithTag("Tile"))
        {
            _rend = tile.GetComponent<Renderer>();
            _rend.material.shader = Shader.Find("Unlit/GrayscaleTexture");
            _rend.material.SetFloat("_IsSnow", 1.0f);
            _rend.material.SetFloat("_SnowLevel", snowLevel);
        }
    }

    private void ColorObstacles(float colorLevel, float scale)
    {
        foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            _rend = obstacle.GetComponent<Renderer>();
            foreach (Material mat in _rend.materials)
            {
                mat.shader = Shader.Find("Unlit/GrayColorMove");    // <-- new shit
                mat.SetFloat("_ColorLevel", colorLevel);
                mat.SetFloat("_Scale", scale);                      // <-- new shit
            }
        }
    }
    
    private void SnowObstacles(float snowLevel)
    {
        foreach (GameObject obstacle in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            _rend = obstacle.GetComponent<Renderer>();
            foreach (Material mat in _rend.materials)
            {
                mat.shader = Shader.Find("Unlit/GrayscaleColor");
                mat.SetFloat("_IsSnow", 1.0f);
                mat.SetFloat("_SnowLevel", snowLevel);
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

    private void SnowPickups(float snowLevel)
    {
        foreach (GameObject pickup in GameObject.FindGameObjectsWithTag("Pickup"))
        {
            _rend = pickup.GetComponent<Renderer>();
            foreach (Material mat in _rend.materials)
            {
                mat.shader = Shader.Find("Unlit/GrayscaleColor");
                mat.SetFloat("_IsSnow", 1.0f);
                mat.SetFloat("_SnowLevel", snowLevel);
            }
        }
    }
}
