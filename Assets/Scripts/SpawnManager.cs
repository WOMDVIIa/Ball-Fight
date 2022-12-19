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
    public GameObject[] enemyPrefab;
    public GameObject[] powerupPrefab;
    public GameObject[] activeEnemiesTable;
    public PlayerController playerController;

    //private int maxNumberOfEnemies = 10;
    private float spawnRange = 9.0f;
    private float missilesDelay = 1.25f;
    
    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.Find("Player(Clone)");
        SpawnPlayer();
        SpawnEnemyWave(waveNumber);
    }

    // Update is called once per frame
    void Update()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (enemyCount == 0)
        {            
            SpawnEnemyWave(waveNumber);
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

        waveNumber++;
        int powerupIndex = Random.Range(0, powerupPrefab.Length);
        Instantiate(powerupPrefab[powerupIndex], GenerateSpawnPosition(), powerupPrefab[0].transform.rotation);
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnPosX = Random.Range(-spawnRange, spawnRange);
        float spawnPosZ = Random.Range(-spawnRange, spawnRange);
        return new Vector3(spawnPosX, 0, spawnPosZ);
    }
}
