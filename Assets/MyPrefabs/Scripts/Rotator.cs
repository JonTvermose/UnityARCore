using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{

    private int _rotationSpeed;

    void Awake()
    {
        var r = new System.Random();
        _rotationSpeed = r.Next(15, 90);
    }

	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(0,_rotationSpeed,0) * Time.deltaTime);
	}
}
