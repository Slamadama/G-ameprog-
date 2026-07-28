using UnityEngine;
using TMPro;
using System.Collections;

public class FishDiscovery : MonoBehaviour
{
    public GameObject lionfishImage;
    public GameObject plecoImage;
    public GameObject earlImage;
    public GameObject fishImage;
    public GameObject doriImage;
    public GameObject jeryImage;
    public GameObject mudskiperImage;
    public GameObject manateImage;
    public GameObject garImage;

    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    public MusicManager musicManager;

    private int fishDiscovered = 0;
    public int totalFish = 3;

    private bool lionfishFound = false;
    private bool plecoFound = false;
    private bool earlFound = false;
    private bool fishFound = false;
    private bool doriFound = false;
    private bool jeryFound = false;
    private bool mudskiperFound = false;
    private bool manateFound = false;
    private bool garFound = false;

    private bool bossSpawned = false;

    public TMP_Text dialogueText;
    public string bossSpawnMessage = "A terrifying creature emerges!";
    public float bossMessageTime = 3f;

    public void DiscoverFish(string fishName)
    {
        if (fishName == "Lionfish" && !lionfishFound)
        {
            lionfishFound = true;
            lionfishImage.SetActive(true);
            fishDiscovered++;
        }

        if (fishName == "Pleco" && !plecoFound)
        {
            plecoFound = true;
            plecoImage.SetActive(true);
            fishDiscovered++;
        }

        if (fishName == "Earl" && !earlFound)
        {
            earlFound = true;
            earlImage.SetActive(true);
            fishDiscovered++;
        }

        if (fishName == "Fish" && !fishFound)
        {
            fishFound = true;
            fishImage.SetActive(true);
            fishDiscovered++;
        }

        if (fishName == "Dori" && !doriFound)
        {
            doriFound = true;
            doriImage.SetActive(true);
            fishDiscovered++;
        }

        if (fishName == "Jery" && !jeryFound)
        {
            jeryFound = true;
            jeryImage.SetActive(true);
            fishDiscovered++;
        }

        if (fishName == "Mudskiper" && !mudskiperFound)
        {
            mudskiperFound = true;
            mudskiperImage.SetActive(true);
            fishDiscovered++;
        }

        if (fishName == "Manate" && !manateFound)
        {
            manateFound = true;
            manateImage.SetActive(true);
            fishDiscovered++;
        }

        if (fishName == "Gar" && !garFound)
        {
            garFound = true;
            garImage.SetActive(true);
            fishDiscovered++;
        }

        CheckJournalComplete();
    }

    void CheckJournalComplete()
    {
        if (fishDiscovered >= totalFish && !bossSpawned)
        {
            SpawnBoss();
        }
    }

    IEnumerator ShowBossMessage()
    {
        dialogueText.gameObject.SetActive(true);

        dialogueText.text = bossSpawnMessage;

        yield return new WaitForSeconds(bossMessageTime);

        dialogueText.gameObject.SetActive(false);
    }

    void SpawnBoss()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.HideQuest();
        }

        bossSpawned = true;

        Instantiate(
            bossPrefab,
            bossSpawnPoint.position,
            bossSpawnPoint.rotation
        );

        musicManager.PlayBossMusic();

        StartCoroutine(ShowBossMessage());

        Debug.Log("Boss Spawned.");
    }

}