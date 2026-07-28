using UnityEngine;

// Spawns a set of fish randomly within the water volume at game start.
// Fish are spawned near the surface (top 15 units of the water volume).
public class FishSpawner : MonoBehaviour
{
    public GameObject[] fishPrefabs; // array of fish prefabs to pick from randomly
    public int fishCount = 15;       // total number of fish to spawn
    public float spawnDepthRange = 8f; // how far down from top (or up from bottom) to spawn
    public bool spawnFromBottom;       // true = spawn near bottom, false = spawn near surface

    void Start()
    {
        SpawnFish();
    }

    // Finds the WaterBlock_50m trigger volume and spawns fish inside its bounds
    void SpawnFish()
    {
        if (fishPrefabs.Length == 0) return;

        GameObject water = GameObject.Find("WaterBlock_50m");
        if (water == null) return;

        BoxCollider box = water.GetComponent<BoxCollider>();
        if (box == null || !box.isTrigger) return;

        Bounds bounds = box.bounds;

        for (int i = 0; i < fishCount; i++)
        {
            GameObject prefab = fishPrefabs[Random.Range(0, fishPrefabs.Length)];

            float yTop, yBottom;
            if (spawnFromBottom)
            {
                // Spawn near the bottom of the water volume
                yBottom = bounds.min.y;
                yTop = yBottom + spawnDepthRange;
                if (yTop > bounds.max.y) yTop = bounds.max.y;
            }
            else
            {
                // Spawn near the surface
                yTop = bounds.max.y;
                yBottom = yTop - spawnDepthRange;
                if (yBottom < bounds.min.y) yBottom = bounds.min.y;
            }

            Vector3 pos = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(yBottom, yTop),
                Random.Range(bounds.min.z, bounds.max.z)
            );

            // Spawn as child of this spawner (keeps the hierarchy tidy)
            Instantiate(prefab, pos, Random.rotation, transform);
        }
    }
}
