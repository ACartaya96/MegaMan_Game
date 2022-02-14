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

    static int playerScore;
    static int finalScore;
    public int enemyCount = 0;
    public int jumpingCount = 0;
    public int blasterCount = 0;
    public int spawner = 0;

    float gameRestartTime;
    float gamePlayerReadyTime;

    public float gameRestartDelay = 5f;
    public float gamePlayerReadyDelay = 3f;

    TextMeshProUGUI playerScoreText;
    TextMeshProUGUI screenMessageText;

    Vector3 worldLeft;
    Vector3 worldRight;

    Animator animator;
    [SerializeField] public AnimationClip explosion;
    [SerializeField] GameObject[] enemyPrefab;


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
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    /*void Start()
    {
        
    }*/

    // Update is called once per frame
    void Update()
    {
        if (playerReady)
        {
            if (initReadyScreen)
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
            if (gamePlayerReadyTime < 0)
            {
                FreezePlayer(false);
                FreezeEnemies(false);
                screenMessageText.text = "";
                playerReady = false;
            }
            return;
        }

        if (playerScoreText != null)
        {
            playerScoreText.text = String.Format("<mspace=\"{0}\">{1:0000000}</mspace>", playerScoreText.fontSize,
                    playerScore);
        }

        if (!isGameOver)
        {
            RepositionEnemies();
        }
        else
        {
            gameRestartTime -= Time.deltaTime;
            if (gameRestartTime < 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        worldLeft = Camera.main.ViewportToWorldPoint(new Vector3(-0.1f, 0, 0));
        worldRight = Camera.main.ViewportToWorldPoint(new Vector3(1.1f, 0, 0));
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
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<EnemyController>().FreezeEnemy(freeze);
        }
    }

    private void FreezeBullets(bool freeze)
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Projectiles");
        foreach (GameObject bullet in bullets)
        {
            bullet.GetComponent<Bullet_Script>().FreezeBullet(freeze);
        }
    }
    private void RepositionEnemies()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {

            if (enemy.transform.position.x < worldLeft.x)
            {

                switch (enemy.name)
                {
                    case "HomingEnemy":

                        enemy.transform.position = new Vector3(worldRight.x, player.transform.position.y + UnityEngine.Random.Range(-0.1f, 2.0f), 0);
                        enemy.GetComponent<HomingEnemy>().ResetFollowingPath();
                        enemy.GetComponent<HomingEnemy>().state = 0;
                        break;
                }
            }
        }
    }

    public void SpawnEnemies(GameObject spawnZone)
    {
        Debug.Log(spawnZone.name.ToString());

        if (!playerReady)
        {

            switch (spawnZone.name)
            {
                case "Floor 1":
                    if (enemyCount < 1)
                    {
                        StartCoroutine(CallSpawner(enemyPrefab[0]));
                        enemyCount++;
                    }
                    break;
                case "Climb 1":

                    if (blasterCount < 14)
                    {
                        for (int x = 0; x < 14; x++)
                        {
                            StartCoroutine(CallSpawner(enemyPrefab[1]));
                            blasterCount++;
                        }
                    }
                    break;
                case "Floor 2":

                    if (blasterCount < 14)
                    {
                        for (int x = 0; x < 14; x++)
                        {
                            StartCoroutine(CallSpawner(enemyPrefab[1]));
                            blasterCount++;
                        }
                    }
                    if (jumpingCount < 4)
                    {
                        for (int x = 0; x < 4; x++)
                        {
                            StartCoroutine(CallSpawner(enemyPrefab[2]));
                            jumpingCount++;
                        }
                    }
                    break;
            }

        }
    }

    IEnumerator CallSpawner(GameObject enemy)
    {
        yield return new WaitForSeconds(1f);
        SpawnPrefab(enemy);
    }

    private void SpawnPrefab(GameObject enemy)
    {
        switch (enemy.name)
        {
            case "HomingEnemy":
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

                Vector3 spawnPos = new Vector3(worldRight.x, player.transform.position.y + UnityEngine.Random.Range(-0.1f, 2.0f), 0);


                GameObject homing = Instantiate(enemy.gameObject, spawnPos, Quaternion.identity);
                homing.name = enemy.name;
                break;
            case "BlasterEnemy":
                GameObject blaster = Instantiate(enemy.gameObject, SpawnContrroller.Instance.spawnPoints[spawner].transform.position, Quaternion.identity);
                blaster.name = enemy.name;
                switch (SpawnContrroller.Instance.spawnPoints[spawner].name)
                {
                    case "Left":
                        blaster.GetComponent<BlasterEnemyController>().blasterOrientation = BlasterEnemyController.BlasterOrientation.Left;
                        break;
                    case "Right":
                        blaster.GetComponent<BlasterEnemyController>().blasterOrientation = BlasterEnemyController.BlasterOrientation.Right;
                        break;
                    case "Bottom":
                        blaster.GetComponent<BlasterEnemyController>().blasterOrientation = BlasterEnemyController.BlasterOrientation.Bottom;
                        break;
                    case "Top":
                        blaster.GetComponent<BlasterEnemyController>().blasterOrientation = BlasterEnemyController.BlasterOrientation.Top;
                        break;
                }
                spawner++;
                break;
            case "JumpingEnemy":
                GameObject jumping = Instantiate(enemy.gameObject, SpawnContrroller.Instance.spawnPoints[spawner].transform.position, Quaternion.identity);
                spawner++;
                break;
        }

    }


    public void DespawnEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            if (worldLeft.x < enemy.transform.position.x)
            {
                Destroy(enemy.gameObject);

            }
        }
        enemyCount = 0;
        jumpingCount = 0;
        blasterCount = 0;
        spawner = 0;
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void InstantKill()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerController>().TakeDamage(player.GetComponent<PlayerController>().maxHealth);
    }

    public IEnumerator Defeated(GameObject player)
    {


        yield return new WaitForSeconds(1f);
        FreezeEnemies(true);
        FreezeBullets(true);
        Destroy(player);
        SceneManager.LoadScene(3);
        playerScore = finalScore;
    }

    public void WinGame()
    {
        SceneManager.LoadScene(2);
    }

    
}
    
