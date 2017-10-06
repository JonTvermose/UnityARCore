using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject[] Obstacles;

    public GameObject Pickup;

    public GameObject Player;

    void SpawnAll(int[][] indicator, Vector3[][] positions)
    {
        if (Obstacles == null || Pickup == null || Player == null)
            return;

        System.Random r = new System.Random();
        for (int i = 0; i < indicator.GetLength(0); i++)
        {
            for (int j = 0; j < indicator.GetLength(1); j++)
            {
                GameObject spawnObject = null;
                switch (indicator[i][j])
                {
                    case -1: spawnObject = Player; break;
                    case 1: spawnObject = Pickup; break;
                    case 2: spawnObject = Obstacles[r.Next(Obstacles.Length)]; break;
                    default: break;
                }
                if (spawnObject != null)
                {
                    Instantiate(spawnObject, positions[i][j], Quaternion.identity);
                }
            }
        }
    }
}
