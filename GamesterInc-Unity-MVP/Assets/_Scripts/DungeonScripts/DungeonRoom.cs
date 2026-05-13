using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class DungeonRoom : MonoBehaviour
{
    public enum LastQuestionStatus
    {
        Right,
        Wrong,
        Null
    }

    [SerializeField] private TMP_Text[] answerText;
    [SerializeField] private GameObject[] doors;
    [SerializeField] private SpriteRenderer doorOverlay;

    [SerializeField] private GameObject[] decorations;
    [SerializeField] [Range(0, 100)] private float decorationSpawnChance;
    
    private Random random = new ();
    private DungeonPlayer player;
    private DungeonManager manager;
    private string correctAnswer = "";
    
    public Vector2 size;
    [NonSerialized] public bool isChecking = false;
    [NonSerialized] public DungeonCharacter enemy;

    private void Update()
    {
        if (!isChecking || player is null) return;
        //Check if player is close enough to one of the sides of the room to select the answer and go to the next room.
        for (int i = 0; i < answerText.Length; i++)
        {
            TMP_Text text = answerText[i];
            if (!text.gameObject.activeSelf || text.text == "") continue;
            if (Vector2.Distance(player.transform.position, text.transform.position) > 1) continue;
            isChecking = false;

            Vector2 direction = text.transform.position.normalized;
            direction.x = Mathf.Clamp(direction.x * 10, -1, 1);
            direction.y = Mathf.Clamp(direction.y * 10, -1, 1);
            
            StartCoroutine(manager.NextRoom(direction, text.text == correctAnswer));
        }
    }

    public void SetRoom(DungeonPlayer pPlayer, DungeonManager pManager, DungeonCharacter pEnemy = null,
        LastQuestionStatus lastQuestion = LastQuestionStatus.Null)
    {
        player = pPlayer;
        manager = pManager;
        
        if (pEnemy is not null)
        {
            DungeonCharacter newEnemy = Instantiate(pEnemy, transform);
            newEnemy.transform.localPosition = Vector3.right*2; //HARDCODED, COULD MAKE ENEMY MOVE INTO POSITION LIKE THE PLAYER
            enemy = newEnemy;
        }

        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        switch (lastQuestion)
        {
            case (LastQuestionStatus.Right):
                renderer.color = doorOverlay.color = new Color(0.7f, 1, 0.7f);
                break;
            case (LastQuestionStatus.Wrong):
                renderer.color = doorOverlay.color = new Color(1f, 0.7f, 0.7f);
                break;
        }

        foreach (GameObject obj in decorations)
        {
            if (random.Next(0, 100) > decorationSpawnChance) continue;
            int randX = random.Next((int)-size.x/4, (int)size.x/4);
            int randY = random.Next((int)-size.y/4, (int)size.y/4);
            GameObject decor = Instantiate(obj, transform);
            decor.transform.localPosition = new Vector3(randX, randY, 1);
        }
    }

    /// <summary>
    /// Used to give the room questions and answers.
    /// </summary>
    /// <param name="pQuestion"></param>
    /// <param name="pPlayer"></param>
    /// <param name="pManager"></param>
    public void SetRoomQuestion(DungeonManager.DungeonQuestion pQuestion)
    {
        correctAnswer = pQuestion.trueAnswer;
        string[] answers = pQuestion.GetAnswers(manager.PittyThreshHold).OrderBy(x => random.Next()).ToArray();
        
        for (int i = 0; i < answerText.Length; i ++)
        {
            TMP_Text text = answerText[i];
            text.gameObject.SetActive(i < answers.Length);
            doors[i].SetActive(i >= answers.Length);
            if (i >= answers.Length)
            {
                text.text = "";
                continue;
            }
            text.text = answers[i];
        }
    }

    /// <summary>
    /// Used in combat to hide the questions to get an empty room.
    /// </summary>
    /// <param name="show">check if the texts should be shown or hidden.</param>
    public void ShowQuestion(bool show)
    {
        for (int i = 0; i < answerText.Length; i++)
        {
            TMP_Text text = answerText[i];
            text.gameObject.SetActive(show);
            doors[i].SetActive(text.text == "");
        }
    }

    public void SetRoomToTraversable(bool show)
    {
        doorOverlay.enabled = show;

        float doorOverlayDist = doorOverlay.transform.position.z;

        foreach (GameObject door in doors)
        {
            float newDist = show ? doorOverlayDist - 1 : 5;
            
            door.transform.position = new Vector3(door.transform.position.x, door.transform.position.y, newDist);
        }
    }
}
