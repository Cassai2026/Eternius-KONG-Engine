using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Random = System.Random;

public class DungeonCharacter : MonoBehaviour
{
    [Header("CHARACTER STATS")]
    [SerializeField] private int maxHealth;
    [NonSerialized] public int health = 0;
    [NonSerialized] public int shields = 0;
    [NonSerialized] public int dodge = 0;
    private int dodgeValue = 0;
    
    public List<Skill> skills = new ();

    private Random random = new();
    private SpriteRenderer spriteRenderer;
    
    [NonSerialized] public bool doubleBuff = false;

    public int MaxHealth => maxHealth;

    protected virtual void Start()
    {
        if (health <= 0)
            health = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// For enemy behaviour, will select a random skill to use
    /// </summary>
    /// <returns>One of the character's skills</returns>
    public Skill ActRandom()
    {
        List<Skill> newSkills = skills;
        foreach (Skill skill in newSkills)
        {
            if (skill.cooldownTracker > 0) newSkills.Remove(skill);
        }

        if (newSkills.Count == 0) return skills[0];

        return newSkills[random.Next(0, newSkills.Count)];
    }

    public void UpdateCooldowns(bool allToZero = false)
    {
        foreach (Skill skill in skills)
        {
            if (allToZero)
                skill.cooldownTracker = 0;
            else 
                skill.cooldownTracker = Mathf.Max(0, skill.cooldownTracker - 1);
        }
    }

    /// <summary>
    /// Called to do damage to a character. Character gets cleaned up if hp reaches 0
    /// </summary>
    /// <param name="damage">Amount of damage to do to the character</param>
    public void TakeDamage(int damage)
    {
        if (dodge > 0)
        {
            dodge--;
            if (random.Next(0, 100) < dodgeValue) return;
        }

        int finalDamage = Mathf.Max(0, damage - shields);
        shields = Mathf.Max(0, shields - damage);

        health = Mathf.Clamp(health - finalDamage, 0, maxHealth);
        ValueDisplay.instance.ShowValue(finalDamage, Color.red, transform.position + Vector3.up);
        if (health <= 0)
        {
            Die();
            Debug.Log($"{gameObject.name} died");
        }

        
        if (finalDamage > 0)
            StartCoroutine(AnimateDamage(0.2f));
    }

    /// <summary>
    /// Used to restore health to the character.
    /// </summary>
    /// <param name="value">Amount of health to restore.</param>
    public void HealHealth(int value)
    {
        health = Mathf.Clamp(health + value, 0, maxHealth);
    }

    /// <summary>
    /// For immersia, will make the character blink a couple of times when taking damage.
    /// </summary>
    /// <param name="duration">Duration of blinking</param>
    public IEnumerator AnimateDamage(float duration)
    {
        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(duration);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(duration);
        }
    }

    /// <summary>
    /// Things the character has to do when their health reaches 0
    /// </summary>
    private void Die()
    {
        Destroy(gameObject);
    }

    //TODO: MAKE IT SO PLAYER CAN BE GIVEN SKILLS THROUGHOUT GAME
    [Serializable]
    public class Skill
    {
        public enum Type
        {
            Damage,
            Heal,
            Shield,
            Dodge,
            Buff
        }
        
        public string name;
        [Tooltip("Use '/value' to insert the skill's value into the description \nUse '/cooldown' to show total cooldown \nUse '/tracker' to show current remaining cooldown time")]
        [TextArea] public string description;
        public int value;
        public Type type;
        public int cooldown = 0;
        [NonSerialized] public int cooldownTracker = 0;

        /// <returns>The description of the skill. if using the "/value" keyword, will replace the word with the value assigned to the skill.</returns>
        public string GetDescription(bool doubleBuff = false)
        {
            string s = description.Replace("/value", value.ToString());
            s = s.Replace("/cooldown", cooldown.ToString());
            s = s.Replace("/tracker", cooldownTracker > 0 ? $"(Time remaining: {cooldownTracker})" : "");
            return s;
        }

        /// <summary>
        /// Uses the skill, based off of the values they have been assigned to.
        /// </summary>
        /// <param name="self">The character using the skill</param>
        /// <param name="toHit">The enemy that the skill can potentially effect</param>
        public void UseSkill(DungeonCharacter self, DungeonCharacter toHit)
        {
            switch (type)
            {
                case(Type.Damage):
                    toHit.TakeDamage(value);
                    break;
                case(Type.Heal):
                    self.HealHealth(value);
                    break;
                case(Type.Shield):
                    self.shields += value;
                    break;
                case (Type.Dodge):
                    self.dodge += 1;
                    self.dodgeValue = value;
                    break;
                case (Type.Buff):
                    self.doubleBuff = true;
                    break;
            }
            self.UpdateCooldowns();
            cooldownTracker = cooldown;
        }

        /// <summary>
        /// Lerp animates the user's skill uses.
        /// </summary>
        /// <param name="self">The user of the skill</param>
        /// <param name="toHit">The enemy to hit</param>
        public IEnumerator AnimateSkill(DungeonCharacter self, DungeonCharacter toHit)
        {
            Vector2 pos = self.transform.position;
            Vector2 scale = self.transform.localScale;
            switch (type)
            {
                case(Type.Damage):
                    if (self.GetType() == typeof(DungeonPlayer))
                    {
                        DungeonPlayer player = self as DungeonPlayer;
                        player.SetCombatAnimations(true);
                        yield return new WaitForSeconds(0.7f);
                        UseSkill(self, toHit);
                        yield return new WaitForSeconds(0.3f);
                    }
                    else
                    {
                        yield return AnimateMoveTo(self, new Vector2(0, pos.y), 0.2f);
                        UseSkill(self, toHit);
                        yield return AnimateMoveTo(self, pos, 0.2f);
                    }
                    break;
                case(Type.Heal):
                    yield return AnimateMoveTo(self, new Vector2(pos.x, pos.y + 1), 0.2f);
                    yield return AnimateMoveTo(self, pos, 0.2f);
                    yield return AnimateMoveTo(self, new Vector2(pos.x, pos.y + 1), 0.2f);
                    yield return AnimateMoveTo(self, pos, 0.2f);
                    UseSkill(self, toHit);
                    ValueDisplay.instance.ShowValue(value, Color.green, self.transform.position + Vector3.up);
                    break;
                case(Type.Shield):
                    yield return AnimateScaleTo(self, new Vector2(scale.x + 0.2f, scale.y + 0.2f), 0.2f);
                    yield return AnimateScaleTo(self, scale, 0.2f);        
                    yield return AnimateScaleTo(self, new Vector2(scale.x + 0.2f, scale.y + 0.2f), 0.2f);
                    yield return AnimateScaleTo(self, scale, 0.2f);
                    UseSkill(self, toHit);
                    ValueDisplay.instance.ShowValue(value, Color.blue, self.transform.position + Vector3.up);
                    break;
                case (Type.Dodge):
                    yield return AnimateMoveTo(self, new Vector2(pos.x + 1, pos.y), 0.1f);
                    yield return AnimateMoveTo(self, new Vector2(pos.x - 1, pos.y), 0.2f);
                    yield return AnimateMoveTo(self, pos, 0.1f);
                    UseSkill(self, toHit);
                    ValueDisplay.instance.ShowValue(value, Color.white, self.transform.position + Vector3.up);
                    break;
                case (Type.Buff):
                    yield return AnimateScaleTo(self, new Vector2(scale.x + 0.2f, scale.y + 0.2f), 0.2f);
                    yield return AnimateScaleTo(self, scale, 0.2f);        
                    yield return AnimateScaleTo(self, new Vector2(scale.x + 0.2f, scale.y + 0.2f), 0.2f);
                    yield return AnimateScaleTo(self, scale, 0.2f);
                    UseSkill(self, toHit);
                    ValueDisplay.instance.ShowValue(2, Color.yellow, self.transform.position + Vector3.up);
                    break;
            }
        }

        /// <summary>
        /// Animates the player's position to the assigned point.
        /// </summary>
        /// <param name="self">Character to animate</param>
        /// <param name="moveTo">Position to animate to</param>
        /// <param name="animDuration">Duration of the animation</param>
        public IEnumerator AnimateMoveTo(DungeonCharacter self, Vector2 moveTo, float animDuration)
        {
            Vector2 startPos = self.transform.position;
            float elapsed = 0.0f;
            while (elapsed < animDuration)
            {
                self.transform.position = Vector2.Lerp(startPos, moveTo, elapsed/animDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            self.transform.position = moveTo;
        }

        /// <summary>
        /// Component used to animate the size of the character;
        /// </summary>
        /// <param name="self">Character to animate</param>
        /// <param name="scaleTo">Size to animate to</param>
        /// <param name="animDuration">Duration of the animation</param>
        public IEnumerator AnimateScaleTo(DungeonCharacter self, Vector2 scaleTo, float animDuration)
        {
            Vector2 startScale = self.transform.localScale;
            float elapsed = 0.0f;
            while (elapsed < animDuration)
            {
                self.transform.localScale = Vector2.Lerp(startScale, scaleTo, elapsed/animDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            self.transform.localScale = scaleTo;
        }
    }
}
