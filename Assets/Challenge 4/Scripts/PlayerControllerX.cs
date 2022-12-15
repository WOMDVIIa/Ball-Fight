using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    public bool hasPowerup;
    public int powerUpDuration = 5;

    public GameObject powerupIndicator;
    public ParticleSystem smokeParticle;

    private float speed;
    private float nominalSpeed = 500;
    private float boostedSpeed = 1000;
    private float normalStrength = 10; // how hard to hit enemy without powerup
    private float powerupStrength = 25; // how hard to hit enemy with powerup

    private Rigidbody playerRb;
    private GameObject focalPoint;
    //private Animator smokeAnimator;
    
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
        //smokeAnimator = GameObject.Find("Smoke_Particle").GetComponent<Animator>();
    }

    void Update()
    {
        // Add force to player in direction of the focal point (and camera)
        float verticalInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * verticalInput * speed * Time.deltaTime); 

        // Set powerup indicator position to beneath player
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.6f, 0);

        TurboBoost();
    }

    // If Player collides with powerup, activate powerup
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            hasPowerup = true;
            powerupIndicator.SetActive(true);
            StartCoroutine(PowerupCooldown());
        }
    }

    // Coroutine to count down powerup duration
    IEnumerator PowerupCooldown()
    {
        yield return new WaitForSeconds(powerUpDuration);
        hasPowerup = false;
        powerupIndicator.SetActive(false);
    }

    // If Player collides with enemy
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Rigidbody enemyRigidbody = other.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = other.gameObject.transform.position - transform.position;

            if (hasPowerup) // if have powerup hit enemy with powerup force
            {
                enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            }
            else // if no powerup, hit enemy with normal strength 
            {
                enemyRigidbody.AddForce(awayFromPlayer * normalStrength, ForceMode.Impulse);
            }
        }
    }

    private void TurboBoost()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            speed = boostedSpeed;
            smokeParticle.transform.position = transform.position + new Vector3(0, -0.6f, 0);            
            smokeParticle.Play();
        }
        else
        {
            speed = nominalSpeed;
        }
    }

}