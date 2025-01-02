using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Mirror.BouncyCastle.Security;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float initialGameSpeed = 5f;
    public float gameSpeed {  get; private set; }
    public float gameSpeedIncrease = 0.1f;

    public TextMeshProUGUI gameOverText;
    public Button retryButton;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hiScoreText;
    public TextMeshProUGUI hiScoreLabel;

    private List<Player> playerList = new List<Player>();
    [SerializeField] private ObjectSpawner spawner;

    private float score = 0;
    private float hiScore = 0;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();
        GetAllPlayers();

        foreach(Obstacle obstacle in obstacles)
        {
            Destroy(obstacle.gameObject);
        }

        gameSpeed = initialGameSpeed;
        score = 0;
        enabled = true;

        foreach(var p in playerList)
        {
            p.gameObject.SetActive(true);
        }
        spawner.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);   
    }

    public void GameOver()
    {
        gameSpeed = 0f;
        enabled = false;

        foreach(var p in playerList)
        {
            p.gameObject.SetActive(false);
        }
        spawner.gameObject.SetActive(false);
        gameOverText.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
        if(hiScore < score)
        {
            hiScore = score;
            hiScoreText.text = Mathf.FloorToInt(hiScore).ToString("D5");
        }
    }

    private void Update()
    {
        gameSpeed += gameSpeedIncrease * Time.deltaTime;
        score += gameSpeed * Time.deltaTime;
        scoreText.text = Mathf.FloorToInt(score).ToString("D5");
    }

    [Server]
    public void GetAllPlayers()
    {
        Invoke(nameof(FindPlayers), 1f); 
    }

    public void FindPlayers()
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");

        playerList = new List<Player>();
        foreach (var playerObject in allPlayers)
        {
            Player playerScript = playerObject.GetComponent<Player>(); 
            if (playerScript != null)
            {
                playerList.Add(playerScript);
            }
        }

        Debug.Log("Found " + playerList.Count + " players.");
    }
}
