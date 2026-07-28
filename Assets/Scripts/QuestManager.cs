using UnityEngine;
using UnityEngine.UI;

// QuestManager - A persistent singleton that manages quest state and UI.
// It creates its own Canvas and UI elements at runtime so no manual setup is needed.
// When a quest is active, it displays the quest title, description, and fish progress.
// When the target is reached, it shows a completion message then hides the UI.
public class QuestManager : MonoBehaviour
{
    [Header("Quest State (read-only)")]
    [Tooltip("Whether a quest is currently in progress")]
    public bool IsQuestActive { get; private set; }

    // Singleton instance so other scripts (e.g., CollisionDetection) can call FishCollected()
    // Auto-creates itself with a new GameObject if none exists in the scene.
    public static QuestManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("QuestManager");
                _instance = go.AddComponent<QuestManager>();
            }
            return _instance;
        }
    }
    private static QuestManager _instance;

    // --- Runtime quest data ---
    private string currentTitle;
    private string currentDescription;
    private int currentTarget;
    private string currentCompletionMessage;
    private int currentCount;

    // --- UI references (created in CreateQuestUI) ---
    private GameObject canvasGO;
    private GameObject questPanel;
    private Text questTitleText;
    private Text questDescriptionText;
    private Text questProgressText;
    private Text completionText;

    void Awake()
    {
        // Singleton: ensure only one QuestManager exists
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Create the quest UI hierarchy
        CreateQuestUI();
    }

    // Builds a Canvas with all required UI elements at runtime
    void CreateQuestUI()
    {
        // --- Canvas (screen-space overlay) ---
        canvasGO = new GameObject("QuestCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // --- Quest Panel (semi-transparent background at top-center of screen) ---
        questPanel = new GameObject("QuestPanel");
        questPanel.transform.SetParent(canvasGO.transform, false);

        RectTransform panelRect = questPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 1f);
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.pivot = new Vector2(0.5f, 1f);
        panelRect.anchoredPosition = new Vector2(0f, -20f);
        panelRect.sizeDelta = new Vector2(420f, 160f);

        Image panelImage = questPanel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.75f);

        // --- Quest Title Text ---
        GameObject titleGO = new GameObject("TitleText");
        titleGO.transform.SetParent(questPanel.transform, false);
        questTitleText = titleGO.AddComponent<Text>();
        questTitleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        questTitleText.fontSize = 22;
        questTitleText.fontStyle = FontStyle.Bold;
        questTitleText.color = Color.white;
        questTitleText.alignment = TextAnchor.MiddleCenter;
        questTitleText.text = "";

        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 0.6f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        // --- Quest Description Text ---
        GameObject descGO = new GameObject("DescriptionText");
        descGO.transform.SetParent(questPanel.transform, false);
        questDescriptionText = descGO.AddComponent<Text>();
        questDescriptionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        questDescriptionText.fontSize = 16;
        questDescriptionText.color = Color.white;
        questDescriptionText.alignment = TextAnchor.MiddleCenter;
        questDescriptionText.text = "";

        RectTransform descRect = descGO.GetComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0f, 0.3f);
        descRect.anchorMax = new Vector2(1f, 0.6f);
        descRect.offsetMin = Vector2.zero;
        descRect.offsetMax = Vector2.zero;

        // --- Quest Progress Text (e.g., "Fish: 5 / 15") ---
        GameObject progressGO = new GameObject("ProgressText");
        progressGO.transform.SetParent(questPanel.transform, false);
        questProgressText = progressGO.AddComponent<Text>();
        questProgressText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        questProgressText.fontSize = 18;
        questProgressText.color = Color.yellow;
        questProgressText.alignment = TextAnchor.MiddleCenter;
        questProgressText.text = "";

        RectTransform progressRect = progressGO.GetComponent<RectTransform>();
        progressRect.anchorMin = new Vector2(0f, 0f);
        progressRect.anchorMax = new Vector2(1f, 0.3f);
        progressRect.offsetMin = Vector2.zero;
        progressRect.offsetMax = Vector2.zero;

        // --- Completion Text (shown separately when quest is done) ---
        GameObject compGO = new GameObject("CompletionText");
        compGO.transform.SetParent(canvasGO.transform, false);
        completionText = compGO.AddComponent<Text>();
        completionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        completionText.fontSize = 28;
        completionText.fontStyle = FontStyle.Bold;
        completionText.color = Color.green;
        completionText.alignment = TextAnchor.MiddleCenter;
        completionText.text = "";

        RectTransform compRect = compGO.GetComponent<RectTransform>();
        compRect.anchorMin = new Vector2(0f, 0.4f);
        compRect.anchorMax = new Vector2(1f, 0.6f);
        compRect.offsetMin = Vector2.zero;
        compRect.offsetMax = Vector2.zero;

        // Start with everything hidden
        questPanel.SetActive(false);
        completionText.gameObject.SetActive(false);
    }

    // Called by QuestNPC when the player accepts the quest
    public void StartQuest(string title, string description, int targetCount, string completionMsg)
    {
        // Store quest data
        currentTitle = title;
        currentDescription = description;
        currentTarget = targetCount;
        currentCompletionMessage = completionMsg;
        currentCount = 0;
        IsQuestActive = true;

        // Update and show the UI
        UpdateQuestUI();
        questPanel.SetActive(true);
        completionText.gameObject.SetActive(false);

        Debug.Log("[QuestManager] Quest started: Collect " + targetCount + " fish");
    }

    // Called by CollisionDetection each time a fish is destroyed
    public void FishCollected()
    {
        // Ignore fish kills when no quest is active
        if (!IsQuestActive) return;

        currentCount++;

        // Clamp so we don't exceed the target (safety check)
        if (currentCount > currentTarget)
            currentCount = currentTarget;

        // Refresh the on-screen progress
        UpdateQuestUI();

        Debug.Log("[QuestManager] Fish collected: " + currentCount + " / " + currentTarget);

        // Check if the player has reached the target
        if (currentCount >= currentTarget)
        {
            CompleteQuest();
        }
    }

    // Refreshes the UI text elements with current quest data
    void UpdateQuestUI()
    {
        if (questTitleText != null)
            questTitleText.text = currentTitle;

        if (questDescriptionText != null)
            questDescriptionText.text = currentDescription;

        if (questProgressText != null)
            questProgressText.text = "Fish: " + currentCount + " / " + currentTarget;
    }

    // Called when the fish target is reached
    void CompleteQuest()
    {
        IsQuestActive = false;

        // Show the completion message in the center of the screen
        if (completionText != null)
        {
            completionText.text = currentCompletionMessage;
            completionText.gameObject.SetActive(true);
        }

        // Hide the progress panel
        if (questPanel != null)
            questPanel.SetActive(false);

        Debug.Log("[QuestManager] Quest completed: " + currentTitle);
    }
}
