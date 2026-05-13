using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class GearUpManager : MonoBehaviour
{
    [SerializeField] private ItemTier[] itemTiers;
    [SerializeField] private Button confirmButton;
    
    private Random random = new ();
    private bool isPlayingWrongAnimation = false;

    private void Awake()
    {
        confirmButton.onClick.AddListener(ConfirmItems);
        foreach (ItemTier tier in itemTiers)
        {
            tier.WakeUp();
            tier.itemIndex = random.Next(0, tier.choices.Length);
            tier.UpdateUI();
        }
    }

    private void ConfirmItems()
    {
        foreach (ItemTier tier in itemTiers)
        {
            if (tier.itemIndex != 0)
            {
                if (!isPlayingWrongAnimation)
                    StartCoroutine(AnimateButtonWrong());
                Debug.Log("INCORRECT ITEMS");
                return;
            }
        }

        Debug.Log("CORRECT ITEMS!");
        SceneManager.LoadScene(5);
    }

    private IEnumerator AnimateButtonWrong()
    {
        isPlayingWrongAnimation = true;
        float duration = 2.0f;
        float elapsed = 0.0f;
        Image buttonImage = confirmButton.image;
        while (duration > elapsed)
        {
            elapsed += Time.deltaTime;
            buttonImage.color = (elapsed % 1 > 0.5f ? Color.red : Color.black);
            yield return null;
        }
        buttonImage.color = Color.black;
        isPlayingWrongAnimation = false;
    }

    [Serializable]
    private class ItemTier
    {
        [Tooltip("This string has no particular code purpose, mostly to keep overview from the editor.")]
        public string tierTitle;
        //CORRECT ITEM IS AT INDEX 0:
        public Item[] choices;
        
        [Space(5)]
        [Header("SEPARATE VARIABLES")]
        [NonSerialized] public int itemIndex = 0;
        public TMP_Text itemTitle;
        public Image renderer;
        public Button nextButton, previousButton;

        public void WakeUp()
        {
            nextButton.onClick.AddListener(delegate { ChangeItem(1); });
            previousButton.onClick.AddListener(delegate { ChangeItem(-1); });
        }

        private void ChangeItem(int indexChange)
        {
            itemIndex += indexChange;
            if (itemIndex < 0)
                itemIndex = choices.Length - 1;
            if (itemIndex >= choices.Length)
                itemIndex = 0;
            UpdateUI();
        }

        public void UpdateUI()
        {
            if (itemTitle is not null)
                itemTitle.text = choices[itemIndex].name;
            if (renderer is not null)
                renderer.sprite = choices[itemIndex].sprite;
            renderer.color = renderer.sprite == null ? Color.clear : Color.white;
        }
    }

    [Serializable]
    private class Item
    {
        public string name;
        public Sprite sprite;
    }
}
