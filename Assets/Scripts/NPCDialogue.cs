using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueNPC : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionRange = 4f;

    [Header("Dialogue")]
    [TextArea(2, 5)]
    public string dialogueMessage = "Hello there!";

    public TMP_Text dialogueText;

    public float dialogueDuration = 3f;


    private Transform playerTransform;
    private bool playerInRange;
    private Coroutine dialogueCoroutine;


    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }


    void Update()
    {
        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            playerInRange = distance <= interactionRange;
        }


        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ShowDialogue();
        }
    }


    void ShowDialogue()
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
        }

        dialogueCoroutine = StartCoroutine(DisplayDialogue());
    }


    IEnumerator DisplayDialogue()
    {
        dialogueText.gameObject.SetActive(true);

        dialogueText.text = dialogueMessage;

        yield return new WaitForSeconds(dialogueDuration);

        dialogueText.gameObject.SetActive(false);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, interactionRange);
    }
}