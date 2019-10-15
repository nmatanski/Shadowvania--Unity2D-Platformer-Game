using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    //Config

    [SerializeField]
    private Player player;
    public Player Player
    {
        get
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            }
            return player;
        }
        set { player = value; }
    }

    [SerializeField]
    private int playerLivesCapacity = 3;
    public int PlayerLivesCapacity
    {
        get { return playerLivesCapacity; }
        set { playerLivesCapacity = value; }
    }

    [SerializeField]
    private int currentPlayerLives = 3;
    public int CurrentPlayerLives
    {
        get { return currentPlayerLives; }
        set { currentPlayerLives = value; }
    }

    [SerializeField]
    private int gold = 0;

    [SerializeField]
    private string lastCheckpointSceneOnDeath = "Main Menu";
    public string LastCheckpointSceneOnDeath
    {
        get { return lastCheckpointSceneOnDeath; }
        set { lastCheckpointSceneOnDeath = value; }
    }

    [SerializeField]
    private Vector2 lastCheckpointOnHit = new Vector2(0, 0);
    public Vector2 LastCheckpointOnHit
    {
        get { return lastCheckpointOnHit; }
        set { lastCheckpointOnHit = value; }
    }


    [SerializeField]
    private TextMeshProUGUI livesText;

    [SerializeField]
    private TextMeshProUGUI goldText;

    //State

    private bool hasDied = false;
    private bool hasHit = false;

    public bool HasEntered { get; set; } = false;

    public string ExitUsed { get; set; }

    public Vector2 LastCheckpointOnDeath { get; set; } = Vector2.zero;



    private void Start()
    {
        livesText.text = CurrentPlayerLives.ToString();
        goldText.text = gold.ToString();
    }

    private void Awake()
    {
        int sessionsCount = FindObjectsOfType<GameSession>().Length;
        if (sessionsCount > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
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
        print("hasentered = " + HasEntered);
        if (hasDied)
        {
            print(LastCheckpointOnDeath);
            if (LastCheckpointOnDeath != Vector2.zero)
            {
                Player.transform.position = LastCheckpointOnDeath;
            }

            livesText.text = currentPlayerLives.ToString();
            goldText.text = gold.ToString();

            hasDied = false;
        }
        else if (hasHit)
        {
            Player.transform.position = LastCheckpointOnHit;
            hasHit = false;
        }
        else if (HasEntered)
        {
            foreach (var exit in FindObjectsOfType<RoomExit>())
            {
                if (exit.Name == ExitUsed)
                {
                    Player.transform.position = exit.transform.GetChild(0).position;
                    break;
                }
            }

            HasEntered = false;
        }
        ///TODO: else if check which entrance did the player enter the new scene and spawn it there and not the default position
        ///you should save the exit portal BEFORE the scene load and here get the portal's coords for the next scene - serializefield vector2 on the portal for the next scene
    }



    public void ProcessPlayerDeath()
    {
        if (CurrentPlayerLives > 1)
        {
            TakeLife();
        }
        else
        {
            ResetSessionToLastCheckpoint(); //respawn the player with full health on the last checkpoint
        }
    }

    public void AddToGold(int goldAmount)
    {
        gold += goldAmount;
        goldText.text = gold.ToString();
    }

    private void ResetSessionToLastCheckpoint()
    {
        hasDied = true;
        SceneManager.LoadScene(LastCheckpointSceneOnDeath); ///TODO: Respawn on/near the checkpoint
        //Destroy(gameObject); ///TODO: Do not destroy the progress, just respawn
        CurrentPlayerLives = PlayerLivesCapacity;
        ///TODO: gold /= 2;
        gold /= 2;
    }

    private void TakeLife()
    {
        CurrentPlayerLives--;
        livesText.text = CurrentPlayerLives.ToString();
        if (LastCheckpointOnHit != Vector2.zero)
        {
            hasHit = true;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
