using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonCombatManager : MonoBehaviour
{
    public enum CombatQuestionMethod
    {
        Manditory,
        Optional
    }

    private CombatQuestion combatQuestion;
    private DungeonHUD hud;
    private DungeonPlayer player;
    private DungeonCharacter enemy;
    private bool playerTurn = true;
    
    public CombatQuestionMethod method;
    [SerializeField] private TMP_Text valueDisplayText;
    [SerializeField] private int healAfterCombat = -1;

    private void Awake()
    {
        combatQuestion = FindObjectOfType<CombatQuestion>();
        player = FindObjectOfType<DungeonPlayer>();
        hud = FindObjectOfType<DungeonHUD>();

        valueDisplayText.gameObject.AddComponent<ValueDisplay>();
    }

    /// <summary>
    /// Will put the player in combat with the enemy. Called at the start of a fight
    /// </summary>
    /// <param name="pEnemy">Enemy that the player has to fight</param>
    public void StartCombat(DungeonCharacter pEnemy)
    {
        hud.ChangeView(DungeonHUD.Views.Combat);
        hud.Enemy = pEnemy;
        playerTurn = true;
        enemy = pEnemy;
        if (method == CombatQuestionMethod.Optional)
            hud.SetButtons(true);
        hud.SetHp(player, enemy, playerTurn);
        combatQuestion.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// Called when combat ends. If the player loses, the game resets. if the player wins the game goes back to the question rooms.
    /// </summary>
    public void EndCombat()
    {
        if (player.health <= 0)
        {
            PlayerPrefs.DeleteKey("DungeonData");
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }
        player.UpdateCooldowns(true);
        if (healAfterCombat > 0)
        {
            player.HealHealth(healAfterCombat);
            ValueDisplay.instance.ShowValue(healAfterCombat, Color.green, player.transform.position + Vector3.up);
        }

        hud.ChangeView(DungeonHUD.Views.Question);
        hud.Enemy = null;
        hud.SetHp();
        hud.SetButtons(false);
        hud.SetSkillText();
        DungeonManager.instance.OutOfCombat();
    }
    
    /// <summary>
    /// Will cycle between the turns of each character to act in combat
    /// </summary>
    public void NextTurn()
    {
        if (player is null || enemy is null || player.health <= 0 || enemy.health <= 0)
        {
            EndCombat();
            return;
        }
        
        playerTurn = !playerTurn;
        hud.SetHp(player, enemy, playerTurn);
        
        if (playerTurn)
        {
            combatQuestion.gameObject.SetActive(true);
            if (method == CombatQuestionMethod.Manditory) return; 
            hud.SetButtons(true);
        }
        else
        {
            StartCoroutine(ActivateSkill(enemy.ActRandom()));
        }
    }
    
    /// <summary>
    /// Coded animation of how an attack sequence goes.
    /// </summary>
    /// <param name="skill">The skill that gets used</param>
    public IEnumerator ActivateSkill(DungeonCharacter.Skill skill)
    {
        hud.SetSkillText(skill, true, false);
        yield return new WaitForSeconds(0.7f);

        if (playerTurn)
            yield return skill.AnimateSkill(player, enemy);
        else
            yield return  skill.AnimateSkill(enemy, player);

        hud.SetHp(player, enemy);

        yield return new WaitForSeconds(0.2f);

        if (player.health <= 0 || enemy.health <= 0)
        {
            EndCombat();
            yield break;
        }
        
        if (player.doubleBuff)
        {
            player.doubleBuff = false;
            yield return ActivateSkill(skill);
            yield break;
        }

        hud.SetSkillText();
        hud.SetButtons(false);
        NextTurn();
    }
    
    public void BuffPlayer()
    {
        player.doubleBuff = true;
        hud.SelectSkill(null);
    }

    public void UpdatePlayerCooldown()
    {
        player.UpdateCooldowns();
    }
}

public class ValueDisplay : MonoBehaviour
{
    private TMP_Text text;
    public static ValueDisplay instance;
    private IEnumerator coroutine;
    private void Awake()
    {
        if (instance != null) Destroy(this);
        instance = this;
        
        text = GetComponent<TMP_Text>();
        text.text = "";
    }

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    public void ShowValue(int pValue, Color pColor, Vector2 pPosition)
    {
        if (coroutine is not null)
            StopCoroutine(coroutine);
        coroutine = AnimateValue(pValue, pColor, pPosition);
        StartCoroutine(coroutine);
    }

    public IEnumerator AnimateValue(int pValue, Color pColor, Vector2 pPosition)
    {
        Vector3 startPos = Camera.main.WorldToScreenPoint(pPosition);
        transform.position = startPos;
        text.text = pValue.ToString();
        text.color = pColor;

        float elapsed = 0;
        float duration = 2f;
        Color noAlpha = pColor;
        noAlpha.a = 0;
        Vector3 endPos = Camera.main.WorldToScreenPoint(pPosition + Vector2.up * 2);

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed);
            text.color = Color.Lerp(pColor, noAlpha, elapsed);
            
            elapsed += 0.9f * Time.deltaTime;
            yield return null;
        }

        text.text = "";
    }
}
