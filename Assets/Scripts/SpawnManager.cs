using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    public int enemyCount;
    public int waveNumber;
    public bool gameOver;

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
    public TextMeshProUGUI jumpsNumberText;
    public TextMeshProUGUI powerupTimerText;

    [SerializeField] GameObject[] walls;
    [SerializeField] TextMeshProUGUI waveNumberText;
    [SerializeField] Button restartButton;

    //private int maxNumberOfEnemies = 10;
    int numberOfWalls = 6;
    int numberOfActiveWalls;
    int bossEveryXWaves = 4;
    float spawnRange = 9.0f;
    float missilesDelay = 1.25f;
    bool bossLastWave = false;
    
    // Start is called before the first frame update
    void Start()
    {
        gameOver = false;
        SpawnPlayer();
        SpawnEnemyWave(waveNumber);
        //SpawnBoss();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver)
        {
            enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if (enemyCount == 0)
            {
                IncreaseWaveNumberAndSmashesLeft();

                if ((waveNumber % bossEveryXWaves == 0) && !bossLastWave)
                {
                    SpawnBoss();
                }
                else
                {
                    if (waveNumber > activeEnemiesTable.Length)
                    {
                        SpawnEnemyWave(activeEnemiesTable.Length);
                    }
                    else
                    {
                        SpawnEnemyWave(waveNumber);
                    }
                    bossLastWave = false;
                }
            }

            if (playerController.hasMissiles && playerController.missilesSpawnReady)
            {
                playerController.missilesSpawnReady = false;
                StartCoroutine(NewMissiles());
            }
        }
        else
        {
            restartButton.gameObject.SetActive(true);
        }
    }

    IEnumerator NewMissiles()
    {
        for (int i = 0; i < activeEnemiesTable.Length; i++)
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

    void IncreaseWaveNumberAndSmashesLeft()
    {
        waveNumber++;
        playerController.smashesLeft++;
        jumpsNumberText.text = playerController.smashesLeft.ToString();
    }


    void SpawnPlayer()
    {
        playerClone = Instantiate(playerPrefab, playerPrefab.transform.position, playerPrefab.transform.rotation);
        playerController = playerClone.GetComponent<PlayerController>();
        indicatorClone = Instantiate(indicatorPrefab, indicatorPrefab.transform.position, indicatorPrefab.transform.rotation);
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        waveNumberText.text = waveNumber.ToString();
        SetWallsActive(false);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            int enemyIndex = Random.Range(0, enemyPrefab.Length);
            activeEnemiesTable[i] = Instantiate(enemyPrefab[enemyIndex], GenerateSpawnPosition(), enemyPrefab[0].transform.rotation);
        }

        if (waveNumber < 10)
        {
            playerController.powerupDuration = 7.0f + 0.5f * waveNumber;
        }
        else
        {
            playerController.powerupDuration = 7.0f + 0.5f * activeEnemiesTable.Length;
        }

        SpawnPowerup();
    }

    void SpawnPowerup()
    {
        int powerupIndex = Random.Range(0, powerupPrefab.Length);
        Instantiate(powerupPrefab[powerupIndex], GenerateSpawnPosition(), powerupPrefab[0].transform.rotation);
    }

    void SpawnBoss()
    {
        waveNumberText.text = "BOSS";
        bossLastWave = true;
        SetWallsActive(true);
        activeEnemiesTable[0] = Instantiate(bossPrefab, GenerateSpawnPosition(), bossPrefab.transform.rotation);
        StartCoroutine(BossFight());
        StartCoroutine(playerController.PowerupCountdownRoutine(0));
    }

    IEnumerator BossFight()
    {
        playerController.powerupDuration = 2.0f;

        while (bossLastWave)
        {
            SpawnPowerup();
            yield return new WaitForSeconds(10);
        }
    }

    void SetWallsActive(bool status)
    {
        for (int i = 0; i < numberOfWalls; i++)
        {
            walls[i].SetActive(status);
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

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
