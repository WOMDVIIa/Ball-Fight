using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 500.0f;
    public bool hasPowerup = false;
    public bool hasKnockback = false;
    public bool hasMissiles = false;
    public bool missilesSpawnReady = false;

    public GameObject projectilePrefab;

    private float powerupStrength = 15.0f;
    private float powerupDuration = 17.0f;

    private Rigidbody playerRb;
    private GameObject focalPoint;
    private SpawnManager spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * forwardInput * speed * Time.deltaTime);
        spawnManager.indicatorClone.transform.position = transform.position + new Vector3(0, -0.5f, 0);    
    }

    private void OnTriggerEnter(Collider other)
    {
        PowerupsRoutine(other);

        if (other.CompareTag("Knockback"))
        {
            hasKnockback = true;
        }
        else if (other.CompareTag("Missiles"))
        {
            hasMissiles = true;
            missilesSpawnReady = true;
        }
    }

    private void PowerupsRoutine(Collider other)
    {
        Destroy(other.gameObject);
        hasPowerup = true;
        spawnManager.indicatorClone.gameObject.SetActive(true);
        StartCoroutine(PowerupCountdownRoutine());
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(powerupDuration);
        hasPowerup = false;
        hasKnockback = false;
        hasMissiles = false;
        spawnManager.indicatorClone.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && hasKnockback)
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = collision.gameObject.transform.position - transform.position;

            enemyRigidbody.AddForce(awayFromPlayer * powerupStrength * enemyRigidbody.mass, ForceMode.Impulse);
            Debug.Log("Collided with " + collision.gameObject.name + " while powerup set to " + hasPowerup);
        }
    }
}
