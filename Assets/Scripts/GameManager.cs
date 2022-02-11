using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    bool isGameOver;
    bool playerReady;
    bool initReadyScreen;

    int playerScore;

    float gameRestartTime;
    float gamePlayerReadyTime;

    public float gameRestartDelay = 5f;
    public float gamePlayerReadyDelay = 3f;

    TextMeshProUGUI playerScoreText;
    TextMeshProUGUI screenMessageText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    /*void Start()
    {
        
    }*/

    // Update is called once per frame
    void Update()
    {
        if(playerReady)
        {
            if(initReadyScreen)
            {
                FreezePlayer(true);
                FreezeEnemies(true);
                screenMessageText.alignment = TextAlignmentOptions.Center;
                screenMessageText.alignment = TextAlignmentOptions.Top;
                screenMessageText.fontStyle = FontStyles.UpperCase;
                screenMessageText.text = "\n\n\n\nREADY";
                initReadyScreen = false;
            }

            gamePlayerReadyTime -= Time.deltaTime;
            if(gamePlayerReadyTime < 0)
            {
                FreezePlayer(false);
                FreezeEnemies(false);
                screenMessageText.text = "";
                playerReady = false;
            }
            return;
        }

        if(playerScoreText != null)
        {
            playerScoreText.text = String.Format("<mspace=\"{0}\">{1:0000000}</mspace>", playerScoreText.fontSize,
                    playerScore);
        }

        if(!isGameOver)
        {
            RepositionEnemies();
        }
        else
        {
            gameRestartTime -= Time.deltaTime;
            if(gameRestartTime < 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BeginGame();
    }

    private void BeginGame()
    {
        isGameOver = false;
        playerReady = true;
        initReadyScreen = true;
        gamePlayerReadyTime = gamePlayerReadyDelay;
        playerScoreText = GameObject.Find("PlayerScore").GetComponent<TextMeshProUGUI>();
        screenMessageText = GameObject.Find("ScreenMessage").GetComponent<TextMeshProUGUI>();
        SoundManager.Instance.MusicSource.Play();
    }

    public void AddScorePoints(int points)
    {
        playerScore += points;
    }

    private void FreezePlayer(bool freeze)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.GetComponent<PlayerController>().FreezeInput(freeze);
        }

    }

    private void FreezeEnemies(bool freeze)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            enemy.GetComponent<EnemyController>().FreezeEnemy(freeze);
        }
    }

    private void FreezeBullets(bool freeze)
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullets");
        foreach(GameObject bullet in bullets)
        {
            bullet.GetComponent<Bullet_Script>().FreezeBullet(freeze);
        }
    }
    private void RepositionEnemies()
    {
        Vector3 worldLeft = Camera.main.ViewportToWorldPoint(new Vector3(-0.1f, 0, 0));
        Vector3 worldRight = Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0, 0));

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            if(enemy.transform.position.x < worldLeft.x)
            {
                switch (enemy.name)
                {
                    case "HomingEnemy":
                        enemy.transform.position = new Vector3(worldRight.x, UnityEngine.Random.Range(-1.0f, 1.0f), 0);
                        enemy.GetComponent<HomingEnemy>().ResetFollowingPath();
                        enemy.GetComponent<HomingEnemy>().state = 0;
                        break;
                }
            }
        }
    }

    private void SpawnEnemies()
    {
        GameObject spawnZone = GameObject.FindGameObjectWithTag("Spawn Zones");
        switch(spawnZone.name)
        {
            case "Floor 1":
                GameObject [] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach(GameObject enemy in enemies)
                {
                    if (enemies.Length < 6)
                    {
                       
                    }
                }
                break;
        }
    }
}
