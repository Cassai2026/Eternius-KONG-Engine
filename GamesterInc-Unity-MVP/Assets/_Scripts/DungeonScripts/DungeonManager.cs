using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class DungeonManager : MonoBehaviour
{
    public enum enemySpawnType
    {
        AtQuestionWrong,
        Random,
        AfterThreshold
    }

    public static DungeonManager instance;

    [Header("ROOM TWEAKABLE VARIABLES")] 
    [SerializeField] private float lerpSpeed = 1;
    [SerializeField] private int pittyThreshHold = -1;
    [SerializeField] private bool countPittyInCombat = false;
    [SerializeField] private int questionsCorrectTillDone = -1;
    private int questionsLeftTillDone;
    [SerializeField] private List<DungeonQuestion> questions;
    private int questionIndex = 0;
    
    [Space(5)]
    [Header("ENEMY SPAWN METHOD AND FREQUENCY")]
    public enemySpawnType SpawnType;
    [Min(1)] [SerializeField] private int spawnerValue;
    [SerializeField] private bool enemyAtEnd = false;
    
    [Space(5)]
    [Header("REWARDS AND SPAWN ODDS")]
    [SerializeField] private List<DungeonReward> rewards = new();
    [SerializeField] [Range(0, 100)] private float rewardOdds;

    [Space(5)] 
    [Header("FEEDBACK ON QUESTIONS ANSWERED")]
    [SerializeField] private bool colorFeedback;
    [SerializeField] private bool soundFeedback;
    
    [Space(10)]
    [Header("SCENE SETUP")]
    [SerializeField] private DungeonPlayer player;
    [SerializeField] private DungeonCharacter[] enemies;
    [SerializeField] private DungeonRoom startRoom;
    [SerializeField] private DungeonRoom roomPrefab;
    [SerializeField] private GameObject endGameScreen;
    [SerializeField] private TMP_Text endGameText;
    [SerializeField] private Button leaveButton;

    [SerializeField] private AudioClip rightSound;
    [SerializeField] private AudioClip wrongSound;
    
    private Random random = new();
    private AudioSource audioSource;
    private DungeonRoom currentRoom;
    private Camera cam;
    
    private DungeonCombatManager combatManager;
    private CombatQuestion combatQuestion;
    private DungeonHUD dungeonHUD;
    private List<DungeonQuestion> wrongQuestions = new ();
    private string dataPath;
    private int mistakes = 0;
    private int roomsPassed = 0;
    private int totalQuestions;
    private bool lastFight = false;
    
    public List<DungeonQuestion> Questions => questions;
    public int PittyThreshHold => pittyThreshHold;
    public bool CountPittyInCombat => countPittyInCombat;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;
        
        cam = Camera.main;
        combatManager = FindObjectOfType<DungeonCombatManager>();
        combatQuestion = FindObjectOfType<CombatQuestion>();
        dungeonHUD = FindObjectOfType<DungeonHUD>();
        audioSource = GetComponent<AudioSource>();

        totalQuestions = questions.Count;

        dataPath = "DungeonData" + SceneManager.GetActiveScene().buildIndex;
        
        DungeonData dungeonData = JsonUtility.FromJson<DungeonData>(PlayerPrefs.GetString(dataPath));
        if (dungeonData is not null)
        {
            List<DungeonQuestion> questionData = dungeonData.questions;
            if (questionData is not null && questionData.Count > 0)
            {
                questions = questionData;
            }

            List<DungeonQuestion> wrongQuestionData = dungeonData.questionsWrong;
            if (wrongQuestionData is not null && wrongQuestionData.Count > 0)
            {
                wrongQuestions = wrongQuestionData;
            }

            player.health = dungeonData.playerHP;
        }
         
        questions = questions.OrderBy(x => random.Next()).ToList();
        if (questionsCorrectTillDone < 0)
            questionsCorrectTillDone = totalQuestions;
        questionsCorrectTillDone = Mathf.Min(questionsCorrectTillDone, totalQuestions);
        questionsLeftTillDone = totalQuestions - questionsCorrectTillDone; // This creates a number that the questions list needs to be to be done

        currentRoom = startRoom;
        currentRoom.isChecking = true;
        currentRoom.SetRoom(player, this);
        currentRoom.SetRoomQuestion(questions[questionIndex]);
        currentRoom.SetRoomToTraversable(false);
        dungeonHUD.SetHolderQuestion(questions[questionIndex].question);

        
        player.transform.SetParent(currentRoom.transform);
        
        leaveButton.onClick.AddListener(OnReturnPress);
        endGameScreen.SetActive(false);
        leaveButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        combatQuestion.Initiate(questions);
        dungeonHUD.ChangeView(DungeonHUD.Views.Question);
        if (SpawnType != enemySpawnType.AfterThreshold) StartCoroutine( dungeonHUD.ProgressSlider(DungeonHUD.ProgressType.Danger, -1, -1));
        
        
        StartCoroutine(dungeonHUD.ProgressSlider(DungeonHUD.ProgressType.Progress, 
            (totalQuestions - questions.Count) / (float)questionsCorrectTillDone * 100f, 1f));
    }
    
    private void OnDestroy()
    {
        //Avoid Singleton to go to different scenes.
        if (instance == this)
        {
            instance = null;
            SaveQuestionData();
        }
    }

    /// <summary>
    /// Takes the player into the next room after answering a question.
    /// </summary>
    /// <param name="direction">direction the player went in (and where the room should go)</param>
    /// <param name="questionRight">checks if the question was answered correctly</param>
    /// <returns></returns>
    public IEnumerator NextRoom(Vector2 direction, bool questionRight)
    {
        DungeonRoom.LastQuestionStatus lastQuestion;
        
        roomsPassed++;
        if (questionRight)
        {
            questions.RemoveAt(questionIndex);
            lastQuestion = DungeonRoom.LastQuestionStatus.Right;
            StartCoroutine(dungeonHUD.ProgressSlider(DungeonHUD.ProgressType.Progress, 
                (totalQuestions - questions.Count) / (float)questionsCorrectTillDone * 100f, 1f));
        } else {
            mistakes++;
            questions[questionIndex].answeredWrong++;
            lastQuestion = DungeonRoom.LastQuestionStatus.Wrong;
            if (!wrongQuestions.Contains(questions[questionIndex]))
                wrongQuestions.Add(questions[questionIndex]);
        }
        
        if (roomsPassed % spawnerValue == 0)
            StartCoroutine(dungeonHUD.ProgressSlider(DungeonHUD.ProgressType.Danger, 100f, 1.0f));
        else
            StartCoroutine(dungeonHUD.ProgressSlider(DungeonHUD.ProgressType.Danger, ((float)roomsPassed / spawnerValue) % 1f * 100f, 1.0f));
        
        if (!colorFeedback) lastQuestion = DungeonRoom.LastQuestionStatus.Null;
        if (soundFeedback && rightSound is not null && wrongSound is not null) audioSource.PlayOneShot(questionRight ? rightSound : wrongSound);
        
        if (questions.Count <= questionsLeftTillDone)
        {
            if (enemyAtEnd && wrongQuestions.Count > 0)
            {
                lastFight = true;
                questions.Add(new DungeonQuestion());
                combatQuestion.Questions = wrongQuestions;
            } else {
                yield return EndGame(direction);
                yield break;
            }
        }
        
        questionIndex = random.Next(0, questions.Count);
        
        DungeonRoom nextRoom = Instantiate(roomPrefab, new Vector2(currentRoom.size.x * direction.x, currentRoom.size.y * direction.y), Quaternion.identity);
        DungeonCharacter newEnemy = HandleEnemySpawner(questionRight);

        
        if (questionRight)
        {
            if (random.Next(1, 100) < rewardOdds)
            {
                DungeonReward reward = Instantiate(rewards[random.Next(0, rewards.Count-1)], nextRoom.transform);
                reward.transform.localPosition = Vector3.one * 2;
                reward.SetUp(player);
            }
        }
        
        nextRoom.SetRoom(player, this, newEnemy, lastQuestion);
        nextRoom.ShowQuestion(false);
        
        nextRoom.SetRoomToTraversable(true);
        currentRoom.SetRoomToTraversable(true);
        
        player.ActivatePlayer(false);
        player.transform.SetParent(nextRoom.transform);

        Vector2 nextRoomPos = nextRoom.transform.position;
        player.SetMoveToPos(nextRoomPos);
        Vector2 startPos = nextRoomPos;
        Vector2 endPos = nextRoomPos * -1;
        
        float elapsed = 0.0f;
        float duration = 3.0f;
        
        //Moves new room to center camera and player into new room
        while (elapsed < duration)
        {
            nextRoom.transform.position = Vector2.Lerp(startPos, Vector2.zero, elapsed / duration);
            currentRoom.transform.position = Vector3.Lerp(Vector3.zero, endPos, elapsed / duration);
            player.SetMoveToPos(nextRoom.transform.position);
            elapsed += lerpSpeed * Time.deltaTime;
            yield return null;
        }

        nextRoom.transform.position = Vector2.zero;
        nextRoom.SetRoomToTraversable(false);
        Destroy(currentRoom.gameObject);
        currentRoom = nextRoom;
        
        //If it's a combat room, the room will be set up for combat
        if (newEnemy is not null)
        {
            StartCoroutine(ZoomCamera(3));

            var enemyPosition = currentRoom.enemy.transform.position;
            player.SetMoveToPos(new Vector2(-enemyPosition.x, enemyPosition.y));
            player.SetCombatAnimations(false);

            combatManager.StartCombat(currentRoom.enemy);
            yield break;
        }
        
        currentRoom.isChecking = true;
        player.ActivatePlayer(true);
        currentRoom.SetRoomQuestion(questions[questionIndex]);
        dungeonHUD.SetHolderQuestion(questions[questionIndex].question);
    }

    /// <summary>
    /// Uses the enemyspawnertype enum to determine what enemy to spawn
    /// </summary>
    /// <returns>enemy to spawn</returns>
    private DungeonCharacter HandleEnemySpawner(bool questionRight)
    {
        if (lastFight)
            return enemies[random.Next(0, enemies.Length)];

        switch (SpawnType)
        {
            case(enemySpawnType.Random):
                if (random.Next(1, 100) > spawnerValue) return null;
                break;
            case(enemySpawnType.AtQuestionWrong):
                if (questionRight) return null;
                break;
            case(enemySpawnType.AfterThreshold):
                spawnerValue = Mathf.Max(1, spawnerValue);
                if (roomsPassed % spawnerValue != 0) return null;
                break;
        }
        return enemies[random.Next(0, enemies.Length)];
    }

    /// <summary>
    /// Lerps camera zoom to focus on combat or room answering
    /// </summary>
    /// <param name="zoomTo">amount the camera should zoom to</param>
    public IEnumerator ZoomCamera(float zoomTo)
    {
        float startSize = cam.orthographicSize;
        
        float elapsed = 0.0f;
        float duration = 3.0f;
        while (elapsed < duration)
        {
            cam.orthographicSize = Mathf.Lerp(startSize, zoomTo, elapsed / duration);
            elapsed += lerpSpeed * Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// Called to end combat, will set scene to go back to answering questions
    /// </summary>
    public void OutOfCombat()
    {
        player.SetCombatAnimations(false, false);
        currentRoom.SetRoomQuestion(questions[questionIndex]);
        StartCoroutine(ZoomCamera(5));
        StartCoroutine(dungeonHUD.ProgressSlider(DungeonHUD.ProgressType.Danger, 0, 1.0f));

        if (lastFight)
        {
            StartCoroutine(EndGame(Vector2.up));
            return;
        }

        currentRoom.isChecking = true;
        player.ActivatePlayer(true);
        currentRoom.ShowQuestion(true);
        dungeonHUD.SetHolderQuestion(questions[questionIndex].question);
    }

    /// <summary>
    /// Called when the player is done with the game
    /// </summary>
    /// <param name="direction"></param>
    private IEnumerator EndGame(Vector2 direction)
    {
        // wrongQuestionsString.Distinct().ToList();
        List<string> handledQuestions = new();
        string m = "Question(s) answered wrong: \n\n\n";
        if (wrongQuestions.Count == 0)
        {
            m += "NO MISTAKES MADE!!!";
        } else {
            foreach (DungeonQuestion wrong in wrongQuestions)
            {
                if (handledQuestions.Contains(m)) continue;
                m += $"{wrong.question}\n\nCorrect answer: {wrong.trueAnswer}\n\n------------------------------------------------------------\n";
                handledQuestions.Add(wrong.question);
            }
        }
        endGameText.text = m;
        endGameScreen.SetActive(true);

        
        currentRoom.ShowQuestion(false);
        currentRoom.SetRoomToTraversable(true);
        dungeonHUD.ChangeView(DungeonHUD.Views.None);
        SaveQuestionData();
        
        player.SetMoveToPos(new Vector2(20 * direction.x, 20 * direction.y));
        player.ActivatePlayer(false, false);

        if (MinigameManager.instance is not null)
            MinigameManager.instance.SetDungeonSceneDone(true);
        
        if (AchievementManager.Instance is not null)
        {
            AchievementManager.Instance.EarnAchievment("Dungeon Minigame");
            AchievementManager.Instance.EarnAchievment("Master Dungeon");
        }

        yield return new WaitForSeconds(5f);
        
        leaveButton.gameObject.SetActive(true);
    }

    private void OnReturnPress()
    {
        SceneManager.LoadScene("OverWorld");
    }

    private void SaveQuestionData()
    {
        if (questions.Count > questionsLeftTillDone)
        {
            DungeonData list = new(questions, wrongQuestions, player.health);
            string json = JsonUtility.ToJson(list, true);
            PlayerPrefs.SetString(dataPath, json);
        } else {
            PlayerPrefs.DeleteKey(dataPath);
        }
        PlayerPrefs.Save();
    }

    [Serializable]
    public class DungeonQuestion
    {
        public string question = "";
        public string trueAnswer = "true";
        public string[] wrongAnswers = new string[0];
        [NonSerialized] public int answeredWrong = 0;
        
        /// <returns>The list of questions, with the correct answer last</returns>
        public string[] GetAnswers(int threshHold = -1)
        {
            List<string> list = wrongAnswers.ToList();

            if (threshHold != -1 && list.Count > 1 && answeredWrong >= threshHold)
            {
                list.RemoveAt(0);
                if (list.Count > 1 && answeredWrong >= threshHold * 2)
                    list.RemoveAt(0);
            }
            
            list.Add(trueAnswer);
            return list.ToArray();
        }
    }

    [Serializable]
    public class DungeonData
    {
        public List<DungeonQuestion> questions;
        public List<DungeonQuestion> questionsWrong;
        public int playerHP;

        public DungeonData(List<DungeonQuestion> pQuestuons, List<DungeonQuestion> pQuestuonsWrong, int pPlayerHP)
        {
            questions = pQuestuons;
            questionsWrong = pQuestuonsWrong;
            playerHP = pPlayerHP;

        }
    }
}
