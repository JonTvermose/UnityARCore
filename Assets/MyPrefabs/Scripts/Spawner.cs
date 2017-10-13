using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject[] Obstacles;

    public GameObject Pickup;

    public GameObject Player;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    void Start()
    {
    }

    public void SpawnAll(int[,] indicator, GameObject[,] tiles,Camera cam)
    {
        if (Obstacles == null || Pickup == null || Player == null)
            return;

        System.Random r = new System.Random();
        for (int i = 0; i < indicator.GetLength(0); i++)
        {
            for (int j = 0; j < indicator.GetLength(1); j++)
            {
                GameObject spawnObject = null;
                switch (indicator[i,j])
                {
                    case -1: spawnObject = Player; break;
                    case 1: spawnObject = Pickup; break;
                    case 2: spawnObject = Obstacles[r.Next(Obstacles.Length)]; break;
                    default: break;
                }
                if (spawnObject != null)
                {
                    GameObject tile = tiles[i,j];
                    GameObject spawnedObject = Instantiate(spawnObject, tile.transform.position, Quaternion.identity);
                    spawnedObject.transform.position += new Vector3(0f,0.045f,0f);
                    if (indicator[i, j]==-1)
                    {
                        spawnedObject.transform.LookAt(cam.transform);
                        spawnedObject.transform.rotation = Quaternion.Euler(0.0f, spawnedObject.transform.rotation.eulerAngles.y, spawnedObject.transform.rotation.z);
                    }
                    spawnedObjects.Add(spawnedObject);
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
