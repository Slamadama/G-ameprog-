using UnityEngine;

public class JournalUI : MonoBehaviour
{
    public GameObject journal;

    private void Start()
    {
        journal.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleJournal();
        }
    }

    public void ToggleJournal()
    {
        journal.SetActive(!journal.activeSelf);
    }
}
