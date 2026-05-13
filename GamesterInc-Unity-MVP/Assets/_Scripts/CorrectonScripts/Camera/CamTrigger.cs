using System.Collections;
using UnityEngine;

public class CamTrigger : MonoBehaviour
{
    public Vector3 newCamPos, newPlayerpos;
    public float transitionDuration = 1.0f; // Duration for the transition

    private CamController camControl;
    private PlayerController2D playerMovement;

    void Start()
    {
        camControl = Camera.main.GetComponent<CamController>();
        playerMovement = FindObjectOfType<PlayerController2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(SmoothTransition(other));
        }
    }

    private IEnumerator SmoothTransition(Collider2D player)
    {
        Vector3 initialCamMinPos = camControl.minPos;
        Vector3 initialCamMaxPos = camControl.maxPos;
        Vector3 targetCamMinPos = camControl.minPos + newCamPos;
        Vector3 targetCamMaxPos = camControl.maxPos + newCamPos;

        Vector3 initialPlayerPos = player.transform.position;
        Vector3 targetPlayerPos = player.transform.position + newPlayerpos;

        float elapsedTime = 0f;
        
        while (elapsedTime < transitionDuration)
        {
            camControl.minPos = Vector3.Lerp(initialCamMinPos, targetCamMinPos, elapsedTime / transitionDuration);
            camControl.maxPos = Vector3.Lerp(initialCamMaxPos, targetCamMaxPos, elapsedTime / transitionDuration);
            player.transform.position = Vector3.Lerp(initialPlayerPos, targetPlayerPos, elapsedTime / transitionDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
    }
}