﻿using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject[] Obstacles;

    public GameObject[,] ObstacleArray;

    public GameObject Pickup;

    public GameObject Snowman;

    public GameObject Player;

    public GameObject Goal;

    public GameObject Obstacle;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    public Renderer _rend;

    void Awake()
    {
        //Instantiate(Player, gameObject.transform.position, Quaternion.identity);
    }

    void Start()
    {        
        _rend = Obstacle.GetComponent<Renderer>();
        foreach (Material mat in _rend.materials)
        {
            mat.shader = Shader.Find("Unlit/GrayscaleColor");
            mat.SetFloat("_ColorLevel", 0f);
        }

        _rend = Pickup.GetComponent<Renderer>();
        foreach (Material mat in _rend.materials)
        {            
            mat.shader = Shader.Find("Unlit/GrayscaleColor");
            mat.SetFloat("_ColorLevel", 0f);
        }

    }

    public void SpawnAll(int[,] indicator, GameObject[,] tiles, Camera cam, Anchor anchor)
    {
        if (Obstacles == null || Pickup == null || Player == null)
            return;

        ObstacleArray = new GameObject[indicator.GetLength(0), indicator.GetLength(1)];

        System.Random r = new System.Random();
        for (int i = 0; i < indicator.GetLength(0); i++)
        {
            for (int j = 0; j < indicator.GetLength(1); j++)
            {
                GameObject spawnObject = null;
                int indicate = indicator[i, j];
                switch (indicate)
                {
                    case -1: spawnObject = Player; break;
                    case 1: spawnObject = Pickup; break;
                    case 3: spawnObject = Goal; break;
                    case 4: spawnObject = Obstacle; break;
                    //case 3: spawnObject = Obstacles[r.Next(Obstacles.Length)]; break;
                    default: break;
                }
                // Spawn the snowmans!
                if (i == 3 && j == 3 || i == 10 && j == 10)
                {
                    spawnObject = Snowman;
                }
                if (spawnObject != null)
                {
                    GameObject tile = tiles[i,j];
                    GameObject spawnedObject = Instantiate(spawnObject, tile.transform.position, Quaternion.identity,anchor.transform);
                    spawnedObject.transform.position += new Vector3(0f,0.045f,0f);
                    spawnedObjects.Add(spawnedObject);
                    if (indicate == 1)
                    {
                        spawnedObject.transform.position -= new Vector3(0f, 0.025f, 0f);
                        spawnedObject.transform.LookAt(cam.transform);
                        spawnedObject.transform.rotation = Quaternion.Euler(0.0f, spawnedObject.transform.rotation.eulerAngles.y-90, spawnedObject.transform.rotation.z);

                    } else if (indicate == -1)
                    {
                        spawnedObject.transform.position -= new Vector3(0f, 0.025f, 0f);
                        spawnedObject.transform.LookAt(cam.transform);
                        spawnedObject.transform.rotation = Quaternion.Euler(0.0f, spawnedObject.transform.rotation.eulerAngles.y, spawnedObject.transform.rotation.z);
                    } else if (indicate == 4)
                    {
                        ObstacleArray[i, j] = spawnedObject;
                        //spawnedObject.transform.position -= new Vector3(0f, 0.035f, 0f);
                        //spawnedObject.transform.LookAt(cam.transform);
                        spawnedObject.transform.position -= new Vector3(0f, 0.035f, 0f);
                        spawnedObject.transform.rotation = Quaternion.Euler(0.0f,
                            spawnedObject.transform.rotation.eulerAngles.y, spawnedObject.transform.rotation.z);
                    }
                    else // Snemand
                    {
                        spawnedObject.transform.position -= new Vector3(0f, 0.045f, 0f);
                        spawnedObject.transform.LookAt(cam.transform);
                        spawnedObject.transform.rotation = Quaternion.Euler(0.0f, spawnedObject.transform.rotation.eulerAngles.y, spawnedObject.transform.rotation.z);
                    }
                }
            }
        }
    }

    public void DestroyAll()
    {
        foreach (GameObject item in spawnedObjects)
        {
            Destroy(item);
        }
        spawnedObjects = new List<GameObject>();
    }
}
