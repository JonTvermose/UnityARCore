using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject[] Obstacles;

    public GameObject Pickup;

    public GameObject Player;

    public GameObject Goal;

    public GameObject Obstacle;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Awake()
    {
        //Instantiate(Player, gameObject.transform.position, Quaternion.identity);
    }

    void Start()
    {
    }

    public void SpawnAll(int[,] indicator, GameObject[,] tiles, Camera cam)
    {
        if (Obstacles == null || Pickup == null || Player == null)
            return;

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
                if (spawnObject != null)
                {
                    GameObject tile = tiles[i,j];
                    GameObject spawnedObject = Instantiate(spawnObject, tile.transform.position, Quaternion.identity);
                    spawnedObject.transform.position += new Vector3(0f,0.045f,0f);
                    spawnedObjects.Add(spawnedObject);
                    if (indicate == -1)
                    {
                        spawnedObject.transform.position -= new Vector3(0f, 0.025f, 0f);
                        spawnedObject.transform.LookAt(cam.transform);
                        spawnedObject.transform.rotation = Quaternion.Euler(0.0f, spawnedObject.transform.rotation.eulerAngles.y, spawnedObject.transform.rotation.z);
                    } else if (indicate == 4)
                    {
                        spawnedObject.transform.position -= new Vector3(0f, 0.035f, 0f);
                        spawnedObject.transform.LookAt(cam.transform);
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
