using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
    public Transform Target;
    public float HideDistance;
    public Quest quest; // Reference to the quest system

    private void Start()
    {
        SetChildrenActive(false); // Initially hide the arrow
    }

    void Update()
    {
        if (quest.isActive && Target != null) // Check if the quest is active and target exists
        {
            var dir = Target.position - transform.position;

            if (dir.magnitude < HideDistance)
            {
                SetChildrenActive(false);
            }
            else
            {
                SetChildrenActive(true);

                // Calculate rotation angle based on direction to target
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                // Adjust rotation based on player's facing direction
                Vector3 localScale = transform.parent.localScale;
                if (localScale.x < 0)
                {
                    // Player is facing left, so rotate the arrow by 180 degrees
                    angle += 180f;
                }

                // Apply the rotation
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
        else
        {
            SetChildrenActive(false); // Hide the arrow if quest is not active or target doesn't exist
        }
    }

    void SetChildrenActive(bool value)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(value);
        }
    }
}