using UnityEngine;
using System.Collections;

public class NPCController : MonoBehaviour
{
    public bool isBreakingRule = false;
    public bool isDead = false;
    public bool isSmoking = false;
    public bool isNotWearingHardHat = false;

    public Sprite sprite1; // Sprite for breaking the rule
    public Sprite sprite2; // Sprite for not breaking the rule
    public Sprite sprite3; // Sprite for the dead state
    public Sprite warningSprite; // Sprite for the warning indicator

    public GameManager gameManager;
    private SpriteRenderer spriteRenderer;
    public DialogueManagerCorrection dialogueManager;
    public PlayerController2D playercontroller;
    public DesignatedArea designatedArea;

    private Coroutine timerCoroutine;
    private GameObject warningIndicator; // GameObject for the warning sprite
    private float initialWarningSize = 0.5f; // Initial scale of the warning sprite
    private float maxWarningSize = 1.5f; // Maximum scale of the warning sprite

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize the warning indicator
        warningIndicator = new GameObject("WarningIndicator");
        SpriteRenderer warningSpriteRenderer = warningIndicator.AddComponent<SpriteRenderer>();
        warningSpriteRenderer.sprite = warningSprite;
        warningSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder + 1; // Ensure the warning sprite renders above the NPC

        // Adjust position above the NPC - adjust the Y value as needed
        warningIndicator.transform.SetParent(transform);
        warningIndicator.transform.localPosition = new Vector3(0, spriteRenderer.bounds.size.y, 0); // Placed above the NPC by its height
        warningIndicator.transform.localScale = Vector3.one * initialWarningSize; // Start with initial size
        warningIndicator.SetActive(false); // Hide initially

        // Initialize NPC state based on whether it's in a safe zone
        bool inSafeZone = designatedArea != null && designatedArea.IsNpcInSafeZone(gameObject);

        if (!isDead)
        {
            if (!inSafeZone)
            {
                int randomBehavior = Random.Range(0, 3);

                if (randomBehavior == 0)
                {
                    AssignBreakingRuleState();
                }
                else
                {
                    ResetNpcState();
                }
            }
            else
            {
                ResetNpcState();
            }
        }
        else
        {
            spriteRenderer.sprite = sprite3;
        }
    }

    private void AssignBreakingRuleState()
    {
        if (gameObject.CompareTag("SmokerNPC"))
        {
            isSmoking = true;
            isNotWearingHardHat = false;
            spriteRenderer.sprite = sprite1;
        }
        else if (gameObject.CompareTag("HardHatNPC"))
        {
            isSmoking = false;
            isNotWearingHardHat = true;
            spriteRenderer.sprite = sprite1;
        }

        isBreakingRule = true;
        StartTimer();
    }

    private void ResetNpcState()
    {
        isBreakingRule = false;
        isSmoking = false;
        isNotWearingHardHat = false;
        spriteRenderer.sprite = sprite2;
        StopTimer();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController2D player = collision.GetComponent<PlayerController2D>();
            if (isDead)
            {
                Debug.Log("This NPC is dead and cannot interact.");
                return;
            }

            string[] initialDialogue = { "Am I doing something wrong?" };

            dialogueManager.SetLines(initialDialogue);
            dialogueManager.SetChoiceCallback(HandlePlayerChoice);

            playercontroller.canMove = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        dialogueManager.Dialoguebox.SetActive(false);
    }

    private void HandlePlayerChoice(int choiceIndex)
    {
        bool isCorrectChoice = false;
        string chosenAction = dialogueManager.GetChoiceText(choiceIndex);

        if (isBreakingRule)
        {
            if ((isSmoking && chosenAction == "Stop smoking") ||
                (isNotWearingHardHat && chosenAction == "Wear your hard hat"))
            {
                isCorrectChoice = true;
            }
        }
        else if (chosenAction == "nothing's wrong")
        {
            isCorrectChoice = true;
        }

        if (isCorrectChoice)
        {
            Debug.Log("Correct choice!");
            ResetNpcState();
            playercontroller.canMove = true;
        }
        else
        {
            Debug.Log("Wrong choice! 10 seconds penalty.");
            gameManager.AddTime();
            playercontroller.canMove = true;
        }

        dialogueManager.Dialoguebox.SetActive(false);
    }

    private void ChangeSprite(Sprite newSprite)
    {
        spriteRenderer.sprite = newSprite;
    }

    private void StartTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(TimerConsequence());
        warningIndicator.SetActive(true); // Show the warning indicator
        warningIndicator.transform.localScale = Vector3.one * initialWarningSize; // Reset size
    }

    private void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
        warningIndicator.SetActive(false); // Hide the warning indicator
    }

    private IEnumerator TimerConsequence()
    {
        float duration = 45f; // Duration before the NPC dies
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Update the warning indicator size
            float scale = Mathf.Lerp(initialWarningSize, maxWarningSize, elapsed / duration);
            warningIndicator.transform.localScale = Vector3.one * scale;

            yield return null; // Wait for the next frame
        }

        // Ensure the sprite changes to the dead state if still breaking the rule and not dead
        if (isBreakingRule && !isDead)
        {
            ChangeSprite(sprite3);
            isDead = true;
            isBreakingRule = false;
            gameManager.IncrementStrikes();
            warningIndicator.SetActive(false); // Hide the warning indicator since the NPC is dead
        }
    }

    private void OnEnable()
    {
        StartCoroutine(RandomRuleBreakingEvent());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator RandomRuleBreakingEvent()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(Random.Range(30f, 60f));

            if (!isDead && !isBreakingRule)
            {
                if (designatedArea == null || !designatedArea.IsNpcInSafeZone(gameObject))
                {
                    AssignBreakingRuleState();
                }
            }
            else
            {
                isSmoking = false;
                isNotWearingHardHat = false;
                isBreakingRule = false;
                isDead = false;
            }
        }
    }
}
