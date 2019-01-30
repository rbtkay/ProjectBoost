using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketScript : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] float rcsThrust;
    [SerializeField] float mainThrust;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip explosion;
    [SerializeField] AudioClip finish;


    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem explosionParticles;
    [SerializeField] ParticleSystem finishParticles;


    // public GameObject particleSystem;
    Rigidbody shipBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending };
    State rocketState = State.Alive;

    bool isCollisionEnabled = true;

    // Use this for initialization
    void Start()
    {
        shipBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rocketState == State.Alive)
        {
            Thrust();
            Rotate();
        }

        if (Debug.isDebugBuild)
        {
            DebugKeys();
        }
    }

    private void DebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            isCollisionEnabled = !isCollisionEnabled;
        }
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        shipBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
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
        if (rocketState != State.Alive || !isCollisionEnabled)
            return;

        switch (other.gameObject.tag)
        {
            case "Friendly":
                print("it's alright");
                break;
            case "Fuel":
                print("Fuelling");
                break;
            case "Finish":
                StartFinishSequence();
                break;
            default:
                StartDeathSequence();
                // Destroy(gameObject);
                // GameObject particles = GameObject.Instantiate(particleSystem, transform.position, Quaternion.identity);
                break;
        }
    }

    private void StartDeathSequence()
    {
        rocketState = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(explosion);
        explosionParticles.Play();
        Invoke("ReloadGame", 2f);
    }

    private void StartFinishSequence()
    {
        rocketState = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(finish);
        finishParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }

    public void LoadNextScene()
    {
        if ((SceneManager.GetActiveScene().buildIndex + 1) == SceneManager.sceneCountInBuildSettings)
            Debug.Log("Game Finished!!");
        else
        {
            Debug.Log("Go the Next Level");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(0);
    }
}
