﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTreasure : MonoBehaviour {

    private GameObject _treasureTop;
    private ParticleSystem _treasureParticleSystem;
    private bool _isRotating;
    private bool _isInitiatet;
    private float _rotatedDegress;

    // Use this for initialization
    void Start()
    {
        _treasureTop = GameObject.FindGameObjectWithTag("TreasureTop");
        _treasureParticleSystem = gameObject.GetComponentInChildren<ParticleSystem>();
        _isRotating = false;
        _rotatedDegress = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isRotating)
        {
            float rotation = Time.deltaTime * 45;
            // Rotate around X at 45 degrees/second
            _treasureTop.transform.Rotate(rotation, 0, 0);

            _rotatedDegress += rotation;

            if (!GameManager.GameManager_instance.GameEnded)
            {
                GameManager.GameManager_instance.EndGame(); // Tell the game manager that we finished the game
            }

            // We are fully rotated, stop rotation, play animation
            if (_rotatedDegress > 180)
            {
                _isRotating = false;
                _treasureParticleSystem.Play();
            }
        }
    }

    public bool IsPlaying()
    {
        if (!_isInitiatet)
            return false;
        return _isRotating && _treasureParticleSystem.isPlaying;
    }

    public void TreasureFound()
    {
        _isInitiatet = true;
        _isRotating = true;
    }
}
