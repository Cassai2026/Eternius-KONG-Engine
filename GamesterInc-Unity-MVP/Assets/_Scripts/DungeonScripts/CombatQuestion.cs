using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class CombatQuestion : MonoBehaviour
{
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button[] answerButtons;
    
    private Random random = new();
    private Image background;
    private Color baseColor;
    
    [SerializeField] private List<DungeonManager.DungeonQuestion> questions = new();
    private List<CombatAnswer> combatAnswers = new();
    private string correctAnswer;
    
    private DungeonCombatManager combatManager;
    private DungeonManager.DungeonQuestion currentQuestion = null;

    public List<DungeonManager.DungeonQuestion> Questions
    {
        set => questions = value;
    }

    private void Awake()
    {
        combatManager = FindObjectOfType<DungeonCombatManager>();
        background = GetComponent<Image>();
        baseColor = background.color;
    }

    public void Initiate(List<DungeonManager.DungeonQuestion> pQuestions)
    {
        questions = pQuestions;

        foreach (Button button in answerButtons)
        {
            CombatAnswer answer = button.AddComponent<CombatAnswer>();
            answer.Initiate(this);
            combatAnswers.Add(answer);
        }

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EnableButtons(true);
        SetRandomQuestion();
    }

    private void SetRandomQuestion()
    {
        if (questions.Count == 0) return;
        
        background.color = baseColor;
        
        currentQuestion = DungeonManager.instance.Questions[random.Next(0, questions.Count)];
        correctAnswer = currentQuestion.trueAnswer;
        questionText.text = currentQuestion.question;
        string[] answers = currentQuestion.GetAnswers(DungeonManager.instance.PittyThreshHold).OrderBy(x => random.Next()).ToArray();

        for (int i = 0; i < combatAnswers.Count; i ++)
        {
            CombatAnswer button = combatAnswers[i];
            button.gameObject.SetActive(i < answers.Length);
            if (i >= answers.Length) continue;
            button.SetUp(answers[i]);
        }

        StartCoroutine(SetLayout());
    }

    private IEnumerator SetLayout()
    {
        GameObject obj = combatAnswers[0].transform.parent.gameObject;
        obj.SetActive(false);
        yield return new WaitForEndOfFrame();
        obj.SetActive(true);
    }

    public IEnumerator AnswerQuestion(string pAnswer)
    {       
        EnableButtons(false);
        if (pAnswer != correctAnswer && DungeonManager.instance.CountPittyInCombat)
            currentQuestion.answeredWrong++;
        
        if (pAnswer == correctAnswer && combatManager.method == DungeonCombatManager.CombatQuestionMethod.Optional)
        {
            combatManager.BuffPlayer();
            Debug.Log("ANSWER CORRECT.");
        }

        background.color = pAnswer == correctAnswer ? Color.green : Color.red;

        yield return new WaitForSeconds(2.0f);

        background.color = baseColor;

        if (combatManager.method == DungeonCombatManager.CombatQuestionMethod.Manditory)
        {
            if (pAnswer == correctAnswer)
                DungeonHUD.instance.SetButtons(true);
            else
            { 
                combatManager.UpdatePlayerCooldown();
                combatManager.NextTurn();
            }

        }

        gameObject.SetActive(false);
    }

    private void EnableButtons(bool enable)
    {
        foreach (CombatAnswer button in combatAnswers)
        {
            button.EnableButton(enable);
        }
    }
}

public class CombatAnswer : MonoBehaviour
{
    private string answer;
    private TMP_Text buttonText;
    private CombatQuestion questionParent;
    private Button button;

    public void Initiate(CombatQuestion pParent)
    {
        buttonText = GetComponentInChildren<TMP_Text>();
        questionParent = pParent;
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonPress);
    }

    public void SetUp(string pAnswer)
    {
        answer = pAnswer;
        buttonText.text = answer;
    }

    private void OnButtonPress()
    {
        StartCoroutine(questionParent.AnswerQuestion(answer));
    }

    public void EnableButton(bool enable)
    {
        button.interactable = enable;
    }
}