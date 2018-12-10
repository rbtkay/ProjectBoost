﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketScript : MonoBehaviour
{
    [SerializeField] float rcsThrust;
    [SerializeField] float mainThrust;

    Rigidbody shipBody;
    AudioSource engineSound;

    enum State { Alive, Dying, Transcending };
    State rocketState = State.Alive;

    // Use this for initialization
    void Start()
    {
        shipBody = GetComponent<Rigidbody>();
        engineSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Thrust();
        Rotate();
    }


    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            shipBody.AddRelativeForce(Vector3.up * mainThrust);

            if (!engineSound.isPlaying)
            {
                engineSound.Play();
            }
        }
        else
        {
            engineSound.Stop();
        }
    }

    private void Rotate()
    {
        shipBody.freezeRotation = true;

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        shipBody.freezeRotation = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "Friendly":
                print("it's alright");
                break;
            case "Fuel":
                print("Fuelling");
                break;
            case "Finish":
                rocketState = State.Transcending;
                Invoke("LoadNextScene", 2f);
                break;
            default:
                print("Dead");
                break;
        }
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }

    public void ReloadLevel(){
        // if(SceneManager.GetActiveScene
    }
}