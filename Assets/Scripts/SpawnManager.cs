using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public int enemyCount;
    public int waveNumber = 1;

    public GameObject playerPrefab;
    public GameObject playerClone;
    public GameObject indicatorPrefab;
    public GameObject indicatorClone;
    public GameObject projectilePrefab;
    public GameObject activeProjectileClone;
    public GameObject bossPrefab;
    public GameObject[] enemyPrefab;
    public GameObject[] powerupPrefab;
    public GameObject[] activeEnemiesTable;
    public PlayerController playerController;

    [SerializeField] GameObject[] walls;

    //private int maxNumberOfEnemies = 10;
    int numberOfWalls = 6;
    int numberOfActiveWalls;
    private float spawnRange = 9.0f;
    private float missilesDelay = 1.25f;
    private bool bossLastWave = false;
    
    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayer();
        //SpawnEnemyWave(waveNumber);
        SpawnBoss();
    }

    // Update is called once per frame
    void Update()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (enemyCount == 0)
        {
            if ((waveNumber % 2 == 0) && !bossLastWave)
            {
                SpawnBoss();
            }
            else
            {
                SpawnEnemyWave(waveNumber);
                bossLastWave = false;
            }
        }

        if (playerController.hasMissiles && playerController.missilesSpawnReady)
        {
            playerController.missilesSpawnReady = false;
            StartCoroutine(NewMissiles());
        }
    }

    IEnumerator NewMissiles()
    {
        for (int i = 0; i < waveNumber; i++)
        {
            if (activeEnemiesTable[i] != null)
            {
                activeProjectileClone = Instantiate(projectilePrefab, playerClone.transform.position + new Vector3(0, 1.5f, 0), projectilePrefab.transform.rotation);
                ProjectileMovement projectileCloneMoveScript = activeProjectileClone.GetComponent<ProjectileMovement>();
                projectileCloneMoveScript.target = activeEnemiesTable[i];
            }
        }
        yield return new WaitForSeconds(missilesDelay);
        playerController.missilesSpawnReady = true;
    }

    void SpawnPlayer()
    {
        playerClone = Instantiate(playerPrefab, playerPrefab.transform.position, playerPrefab.transform.rotation);
        playerController = playerClone.GetComponent<PlayerController>();
        indicatorClone = Instantiate(indicatorPrefab, indicatorPrefab.transform.position, indicatorPrefab.transform.rotation);
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            int enemyIndex = Random.Range(0, enemyPrefab.Length);
            activeEnemiesTable[i] = Instantiate(enemyPrefab[enemyIndex], GenerateSpawnPosition(), enemyPrefab[0].transform.rotation);
        }

        if (waveNumber < 10)
        {
            waveNumber++;
            playerController.powerupDuration = 7.0f + 0.5f * waveNumber;
        }

        playerController.smashesLeft++;
        SpawnPowerup();
    }

    void SpawnPowerup()
    {
        int powerupIndex = Random.Range(0, powerupPrefab.Length);
        Instantiate(powerupPrefab[powerupIndex], GenerateSpawnPosition(), powerupPrefab[0].transform.rotation);
    }

    void SpawnBoss()
    {
        bossLastWave = true;
        ActivateWalls();
        activeEnemiesTable[0] = Instantiate(bossPrefab, GenerateSpawnPosition(), bossPrefab.transform.rotation);
        playerController.smashesLeft++;
        StartCoroutine(BossFight());
    }

    IEnumerator BossFight()
    {
        while (bossLastWave)
        {
            playerController.powerupDuration = 2.0f;
            SpawnPowerup();
            yield return new WaitForSeconds(10);
        }
    }

    void ActivateWalls()
    {
        for (int i = 0; i < numberOfWalls; i++)
        {
            walls[i].SetActive(true);
        }
        numberOfActiveWalls = numberOfWalls;
    }

    public void DestroyRandomWall()
    {
        if (numberOfActiveWalls > 0)
        {
            int wallToDestroy = Random.Range(0, 6);
            if (walls[wallToDestroy].active)
            {
                Debug.Log(wallToDestroy);
                walls[wallToDestroy].SetActive(false);
            }
            else
            {
                DestroyRandomWall();
            }
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);
        return new Vector3(spawnPosX, 0, spawnPosZ);
    }
}
