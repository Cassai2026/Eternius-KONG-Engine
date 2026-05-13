using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DungeonHUD : MonoBehaviour
{
    public enum Views
    {
        Question,
        Combat,
        None
    }

    public enum ProgressType
    {
        Progress,
        Danger
    }

    public static DungeonHUD instance;
    private DungeonCombatManager combatManager;

    [Header("VIEWS SETUP")]
    [SerializeField] private GameObject QuestionView;
    [SerializeField] private GameObject CombatView;

    [Header("QUESTION SETUP")]
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Animator dangerAnimator;
    [SerializeField] private Image questionHolder;
    private TMP_Text questionText;
    
    [Header("COMBAT SETUP")]
    [SerializeField] private CombatQuestion combatQuestion;
    [SerializeField] private Button[] buttons;
    [SerializeField] private TMP_Text attackText, descriptionText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Slider playerHPBar, enemyHPBar, playerSHDBar, enemySHDBar;
    
    [NonSerialized] public DungeonCharacter.Skill selectedSkill = null;
    
    private List<SkillButton> skillButtons = new ();
    private DungeonPlayer player;
    private DungeonCharacter enemy;
    private static readonly int Threat = Animator.StringToHash("Threat");
    private bool questionIsLerping = false;
    private bool questionUp;

    public DungeonCharacter Enemy
    {
        set => enemy = value;
    }

    public Slider ProgressionSlider => progressSlider;

    private void Awake()
    {
        //singleton setup:
        if (instance is not null) Destroy(gameObject);
        instance = this;
        
        combatManager = FindObjectOfType<DungeonCombatManager>();
        player = FindObjectOfType<DungeonPlayer>();

        questionText = questionHolder.GetComponentInChildren<TMP_Text>();

        DungeonItem.OnShowMap += ShowQuestion;
    }

    private void Start()
    {
        // Setting up the HUD for combat
        foreach (Button b in buttons)
        {
            SkillButton sB = b.AddComponent<SkillButton>();
            sB.Initialize(this);
            skillButtons.Add(sB);
        }
        
        SetButtons(false);
        SetHp();
        SetSkillText();
        confirmButton.onClick.AddListener(OnConfirmPress);
    }

    private void Update()
    {
        if (questionUp && Input.GetMouseButtonDown(0) && !questionIsLerping)
        {
            // Debug.Log("E");
            ShowQuestion();
        }
    }

    private void OnDestroy()
    {
        DungeonItem.OnShowMap -= ShowQuestion;
        //Making sure that the singleton does not exist beyond this scene
        if (instance == this)
            instance = null;
    }

    public void ChangeView(Views nextView)
    {
        QuestionView.SetActive(nextView == Views.Question);
        CombatView.SetActive(nextView == Views.Combat);
    }

    /// <summary>
    /// Will display teh skills of the player through interactive buttons
    /// </summary>
    /// <param name="hideAll"></param>
    public void SetButtons(bool show)
    {
        confirmButton.transform.parent.gameObject.SetActive(show);
        if (!show) return;
        confirmButton.gameObject.SetActive(false);

        for(int i = 0; i < skillButtons.Count; i++)
        {
            SkillButton sButton = skillButtons[i];
            sButton.gameObject.SetActive(i < player.skills.Count);
            if (i > player.skills.Count-1) continue;
            sButton.SetUp(player.skills[i]);
        }
    }

    /// <summary>
    /// Will update the HUD with the health of the player and enemy
    /// </summary>
    /// <param name="show">determines if the hp should be shown or hidden</param>
    public void SetHp(DungeonCharacter pPlayer = null, DungeonCharacter pEnemy = null, bool pPlayerTurn = true)
    {
        playerHPBar.gameObject.SetActive(pPlayer is not null);
        enemyHPBar.gameObject.SetActive(pEnemy is not null);
        
        if (player is not null && pEnemy is not null)
        {
            playerHPBar.value = (float)player.health / player.MaxHealth * 100f;
            
            enemyHPBar.value = (float)enemy.health / enemy.MaxHealth * 100f;
        }
    }

    /// <summary>
    /// Will update the HUD to display what skill is selected
    /// </summary>
    /// <param name="skill">what skill is selected</param>
    /// <param name="setName">Check if the skill name should be displayed</param>
    /// <param name="setDesc">Check if the skill description should be displayed</param>
    public void SetSkillText(DungeonCharacter.Skill skill = null, bool setName = true, bool setDesc = true)
    {
        if (skill is null)
        {
            attackText.text = "";
            descriptionText.text = "";
            return;
        }

        attackText.text = (setName) ? skill.name : "";
        descriptionText.text = (setDesc) ? skill.GetDescription(player.doubleBuff) : "";
    }

    /// <summary>
    /// Called by the button to lock in a skill and make it usable
    /// </summary>
    /// <param name="pSkill"></param>
    public void SelectSkill(DungeonCharacter.Skill pSkill)
    {
        selectedSkill = pSkill;
        confirmButton.gameObject.SetActive(pSkill.cooldownTracker <= 0);
        SetSkillText(selectedSkill, false, true);
    }

    /// <summary>
    /// Pressing the confirm button will turn off the buttons and activate the skill
    /// </summary>
    public void OnConfirmPress()
    {
        if (selectedSkill is null) return;
        SetButtons(false);
        SetSkillText();
        combatQuestion.gameObject.SetActive(false);
        StartCoroutine(combatManager.ActivateSkill(selectedSkill));
    }

    public IEnumerator ProgressSlider(ProgressType progressType, float slideTo, float slideSpeed = 0)
    {
        if (progressType == ProgressType.Danger)
        {
            dangerAnimator.SetFloat(Threat, slideTo);
            yield break;
        }

        float startVal  = progressSlider.value;
        float elapsed = 0f;
        
        while (Math.Abs(progressSlider.value - slideTo) > 0.1f)
        {
            progressSlider.value = Mathf.Lerp(startVal, slideTo, elapsed);
            elapsed += slideSpeed * Time.deltaTime;
            yield return null;
        }

        progressSlider.value = slideTo;
    }
    
    public void SetHolderQuestion(string question)
    {
        questionText.text = question;
    }

    private void ShowQuestion()
    {
        if (questionIsLerping) return;
        // Debug.Log("Gamer");
        float newPos = questionUp ? -Screen.height * 1.25f : Screen.height/2;
        questionIsLerping = true;
        StartCoroutine(LerpQuestion(newPos, 1f));
        questionUp = !questionUp;
    }

    private IEnumerator LerpQuestion(float newYPos, float duration)
    {
        player.ActivatePlayer(questionUp, true);
        Vector3 startPos = questionHolder.transform.position;
        Vector3 endPos = new (questionHolder.transform.position.x, newYPos, 0);

        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            t = t * t * (3f - 2f * t);
            
            questionHolder.transform.position = Vector3.Lerp(startPos, endPos, t);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        questionHolder.transform.position = endPos;
        questionIsLerping = false;
    }
}

public class SkillButton : MonoBehaviour
{
    private DungeonCharacter.Skill skill;
    private DungeonHUD manager;
    private TMP_Text buttonText;

    public void Initialize(DungeonHUD pManager)
    {
        manager = pManager;
        Button b = GetComponent<Button>();
        b.onClick.AddListener(OnPress);

        buttonText = GetComponentInChildren<TMP_Text>();
    }

    public void SetUp(DungeonCharacter.Skill pSkill)
    {
        skill = pSkill;
        buttonText.text = skill.name;
    }

    private void OnPress()
    {
        manager.SelectSkill(skill);
    }
}