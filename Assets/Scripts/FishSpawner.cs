using UnityEngine;

[System.Serializable]
public class FishSpawnGroup
{
    public GameObject fishPrefab;
    public int count = 5;
    public float depthFromSurface = 0f;
    public float spawnDepthRange = 5f;
}

public class FishSpawner : MonoBehaviour
{
    public FishSpawnGroup[] spawnGroups;
    public float spawnRadius = 50f;

#if UNITY_EDITOR
    void Reset()
    {
        spawnGroups = new FishSpawnGroup[3];
        spawnGroups[0] = new FishSpawnGroup
        {
            fishPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Alstra Infinite/Fish - PolyPack/Prefabs/FishV1.prefab"),
            count = 20,
            depthFromSurface = 3f,
            spawnDepthRange = 4f
        };
        spawnGroups[1] = new FishSpawnGroup
        {
            fishPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Alstra Infinite/Fish - PolyPack/Prefabs/FishV2.prefab"),
            count = 10,
            depthFromSurface = 15f,
            spawnDepthRange = 6f
        };
        spawnGroups[2] = new FishSpawnGroup
        {
            fishPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Alstra Infinite/Fish - PolyPack/Prefabs/FishV3.prefab"),
            count = 5,
            depthFromSurface = 35f,
            spawnDepthRange = 6f
        };
    }
#endif

    void Start()
    {
        SpawnFish();
    }

    void SpawnFish()
    {
        if (spawnGroups.Length == 0) return;

        GameObject water = GameObject.Find("WaterBlock_50m");
        if (water == null) return;

        BoxCollider box = water.GetComponent<BoxCollider>();
        if (box == null || !box.isTrigger) return;

        Bounds bounds = box.bounds;
        float waterSurfaceY = bounds.max.y;
        Vector3 center = transform.position;

        foreach (FishSpawnGroup group in spawnGroups)
        {
            if (group.fishPrefab == null) continue;

            float centerY = waterSurfaceY - group.depthFromSurface;
            float yBottom = centerY - group.spawnDepthRange * 0.5f;
            float yTop = centerY + group.spawnDepthRange * 0.5f;
            yBottom = Mathf.Max(yBottom, bounds.min.y);
            yTop = Mathf.Min(yTop, bounds.max.y);

            for (int i = 0; i < group.count; i++)
            {
                Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
                Vector3 pos = new Vector3(
                    Mathf.Clamp(center.x + randomCircle.x, bounds.min.x, bounds.max.x),
                    Random.Range(yBottom, yTop),
                    Mathf.Clamp(center.z + randomCircle.y, bounds.min.z, bounds.max.z)
                );

                Instantiate(group.fishPrefab, pos, Random.rotation, transform);
            }
        }
    }
}
