using UnityEngine;

// QuestNPC - A clickable / interactable NPC that gives the player a quest.
// Place this script on a capsule GameObject positioned on land.
// The player can either:
//   1. Press E when near the NPC (proximity-based)
//   2. Left-click on the NPC from a distance (raycast-based)
// When the quest is accepted, the QuestManager shows on-screen objectives
// to collect a target number of fish.
public class QuestNPC : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("How close the player must be to interact with the E key")]
    public float interactionRange = 4f;

    [Tooltip("Optional prompt text shown when player is in range")]
    public string interactionPrompt = "Press E to talk";

    [Header("Quest Configuration")]
    [Tooltip("Title displayed on the quest UI")]
    public string questTitle = "The Fisher's Task";

    [Tooltip("Description of what the player needs to do")]
    public string questDescription = "Spear and collect fish in the water to prove your worth!";

    [Tooltip("Number of fish the player must collect to complete the quest")]
    public int fishTarget = 15;

    [Tooltip("Message shown when the quest is completed")]
    public string completionMessage = "Well done! You've proven yourself a true fisherman!";

    // Cached references
    private Transform playerTransform;
    private bool playerInRange;
    private MonoBehaviour playerFpsController; // used to detect if player is underwater (disabled = underwater)

    void Start()
    {
        // Find the player in the scene by tag (cached once, not every frame)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            // Cache the FirstPersonController to check if player is on land (enabled) or underwater (disabled)
            playerFpsController = playerObj.GetComponent<StarterAssets.FirstPersonController>();
        }

        // Ensure the NPC has a CapsuleCollider so it can be clicked
        // The collider stays non-trigger so Physics.Raycast can detect it on click
        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (col == null)
        {
            col = gameObject.AddComponent<CapsuleCollider>();
            col.radius = 0.5f;
            col.height = 2f;
        }
    }

    void Update()
    {
        // --- Proximity check (every frame, distance-based) ---
        if (playerTransform != null)
        {
            float dist = Vector3.Distance(transform.position, playerTransform.position);
            playerInRange = dist <= interactionRange;
        }

        // --- Interaction Method 1: Press E when in range ---
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryGiveQuest();
        }

        // --- Interaction Method 2: Left-click on the NPC (raycast) ---
        // Only process clicks on land (when FPS controller is enabled) to avoid
        // conflicting with the spear attack used underwater.
        if (Input.GetMouseButtonDown(0) && (playerFpsController == null || playerFpsController.enabled))
        {
            // Cast a ray from the camera through the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                TryGiveQuest();
            }
        }
    }

    // Attempts to give the quest to the player via the QuestManager singleton
    void TryGiveQuest()
    {
        if (QuestManager.Instance != null && !QuestManager.Instance.IsQuestActive)
        {
            QuestManager.Instance.StartQuest(questTitle, questDescription, fishTarget, completionMessage);
            Debug.Log("[QuestNPC] Quest started: " + questTitle);
        }
        else if (QuestManager.Instance != null && QuestManager.Instance.IsQuestActive)
        {
            Debug.Log("[QuestNPC] A quest is already active.");
        }
    }

    // Visual feedback: show the interaction range in the Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, interactionRange);
    }
}
