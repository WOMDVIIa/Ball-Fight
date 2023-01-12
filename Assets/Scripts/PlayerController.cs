using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 500.0f;
    public bool hasPowerup = false;
    public bool hasKnockback = false;
    public bool hasMissiles = false;
    public bool missilesSpawnReady = true;
    public float powerupDuration = 7.0f;
    public float powerupDurationLeft;
    public int smashesLeft = 1;

    public GameObject projectilePrefab;

    private float powerupStrength = 15.0f;
    private float preSmashForce = 10.0f;
    private float postSmashForce = 30.0f;

    private Rigidbody playerRb;
    private SphereCollider playerCollider;
    private GameObject focalPoint;
    private SpawnManager spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<SphereCollider>();
        focalPoint = GameObject.Find("Focal Point");
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        spawnManager.jumpsNumberText.text = smashesLeft.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        SmashAttack();
    }

    private void PlayerMovement()
    {
        float forwardInput = Input.GetAxis("Vertical");
        playerRb.AddForce(focalPoint.transform.forward * forwardInput * speed * Time.deltaTime);
        spawnManager.indicatorClone.transform.position = transform.position + new Vector3(0, -0.5f, 0);

        if (transform.position.y < -10)
        {
            spawnManager.gameOver = true;
        }
    }

    private void SmashAttack()
    {
        if (Input.GetKeyDown(KeyCode.Space) && smashesLeft > 0)
        {
            StartCoroutine(SmashRoutine());
            smashesLeft -= 1;
            spawnManager.jumpsNumberText.text = smashesLeft.ToString();
        }
    }

    IEnumerator SmashRoutine()
    {
        SmashAwayEnemies(preSmashForce);
        playerRb.AddForce(Vector3.up * 50, ForceMode.Impulse);
        playerCollider.radius = 0.01f;
        yield return new WaitForSeconds(0.2f);
        playerRb.AddForce(Vector3.down * 100, ForceMode.Impulse);
        yield return new WaitForSeconds(0.2f);
        SmashAwayEnemies(postSmashForce);
        yield return new WaitForSeconds(0.1f);
        playerCollider.radius = 0.5f;
    }

    private void SmashAwayEnemies(float smashForce)
    {
        for (int i = 0; i < spawnManager.activeEnemiesTable.Length; i++)
        {
            if (spawnManager.activeEnemiesTable[i] != null)
            {
                Vector3 awayFromPlayer = spawnManager.activeEnemiesTable[i].gameObject.transform.position - transform.position;
                Rigidbody enemyRigidbody = spawnManager.activeEnemiesTable[i].gameObject.GetComponent<Rigidbody>();                               

                enemyRigidbody.AddForce(awayFromPlayer.normalized * (1 / awayFromPlayer.magnitude) * smashForce * enemyRigidbody.mass, ForceMode.Impulse);    
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasPowerup)
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
    }

    private void PowerupsRoutine(Collider other)
    {
        Destroy(other.gameObject);
        hasPowerup = true;
        spawnManager.indicatorClone.gameObject.SetActive(true);
        StartCoroutine(PowerupCountdownRoutine(powerupDuration));
    }

    public IEnumerator PowerupCountdownRoutine(float duration)
    {
        //yield return new WaitForSeconds(duration);
        powerupDurationLeft = duration;
        while (powerupDurationLeft > 0)
        {
            powerupDurationLeft -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
            spawnManager.powerupTimerText.text = powerupDurationLeft.ToString("0.0", new System.Globalization.CultureInfo("en-GB"));
        }
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

            Enemy enemyScript = collision.gameObject.GetComponent<Enemy>();
            if (enemyScript.isBoss)
            {
                enemyScript.hitPoints -= 3;
                if (enemyScript.hitPoints <= 0)
                {
                    //Destroy(collision.gameObject);
                    spawnManager.DestroyRandomWall();
                    enemyScript.hitPoints = enemyScript.bossHP;
                }
            }
        }
    }
}
