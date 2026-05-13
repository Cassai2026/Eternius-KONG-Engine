using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonItem : MonoBehaviour
{
    public enum ItemType
    {
        Crushable,
        Map
    }

    [SerializeField] private ItemType type;
    [SerializeField] private Sprite pickUpSprite;
    [SerializeField] private AudioClip clip;
    private SpriteRenderer renderer;
    private AudioSource source;

    public static event Action OnShowMap; 

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        source = gameObject.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        switch (type)
        {
            case(ItemType.Crushable):
                if (col.gameObject.CompareTag("Player"))
                {
                    renderer.sprite = pickUpSprite;
                    if (clip is not null)
                        source.PlayOneShot(clip);
                    clip = null;
                }
                break;
            case(ItemType.Map):
                OnShowMap?.Invoke();
                break;
        }
    }
}
 