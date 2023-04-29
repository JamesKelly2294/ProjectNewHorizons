using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySquadCoordinator : MonoBehaviour
{
    [HideInInspector]
    public List<EntitySquadSpawnConfig> SpawnConfigs = new List<EntitySquadSpawnConfig>();

    public GameObject EnemySquadPrefab;
    public GameObject MailLoverSquadPrefab;
    public GameObject TargetPrefab;

    // Start is called before the first frame update
    void Start()
    {
        var spawnConfigsGO = transform.Find("Spawns");
        
        if (spawnConfigsGO != null)
        {
            for (var i = 0; i < spawnConfigsGO.transform.childCount; i++)
            {
                var child = spawnConfigsGO.transform.GetChild(i);
                var spawnConfig = child.GetComponent<EntitySquadSpawnConfig>();
                if (spawnConfig != null)
                {
                    SpawnConfigs.Add(spawnConfig);
                }
            }
        }
    }

    public void SpawnEnemySquad()
    {
        SpawnSquad(EnemySquadPrefab);
    }

    public void SpawnMailLoverSquad()
    {
        SpawnSquad(MailLoverSquadPrefab);
    }

    public void SpawnSquad(GameObject squadPrefab)
    {
        if (!Application.isPlaying) { return; }

        var spawnConfig = SelectEntitySquadSpawnConfig();
        if (spawnConfig == null) { Debug.LogError("No Spawn Config Found."); return; }

        var spawnPosition = SelectSpawnPosition(spawnConfig);
        var targetPosition = SelectTargetPosition(spawnConfig);

        var EntitySquadGO = Instantiate(squadPrefab);
        EntitySquadGO.transform.position = spawnPosition;

        var targetGO = Instantiate(TargetPrefab);
        targetGO.transform.position = targetPosition;

        var EntitySquad = EntitySquadGO.GetComponent<EntitySquad>();
        EntitySquad.SetTarget(targetGO.transform);
    }

    public EntitySquadSpawnConfig SelectEntitySquadSpawnConfig()
    {
        if (SpawnConfigs.Count == 0) { Debug.LogError("No Spawn Config Found."); return null; }
        return SpawnConfigs[Random.Range(0, SpawnConfigs.Count)];
    }

    public Vector3 SelectSpawnPosition(EntitySquadSpawnConfig spawnConfig)
    {
        if (spawnConfig.SpawnVolumes.Count == 0) { Debug.LogError("No Spawn Volumes Found."); return Vector3.zero; }

        var totalVolume = spawnConfig.TotalSpawnVolume;
        var randomWeight = Random.Range(0, totalVolume);

        Bounds selectedBounds = spawnConfig.SpawnVolumes[0];
        foreach(var volume in spawnConfig.SpawnVolumes)
        {
            totalVolume -= volume.GetVolume();
            if (randomWeight >= totalVolume)
            {
                selectedBounds = volume;
                break;
            }
        }
        
        return new Vector3(Random.Range(selectedBounds.min.x, selectedBounds.max.x), 1.0f, Random.Range(selectedBounds.min.z, selectedBounds.max.z));
    }

    public Vector3 SelectTargetPosition(EntitySquadSpawnConfig spawnConfig)
    {
        if (spawnConfig.TargetVolumes.Count == 0) { Debug.LogError("No Target Volumes Found."); return Vector3.zero; }

        var totalVolume = spawnConfig.TotalTargetVolume;
        var randomWeight = Random.Range(0, totalVolume);

        Bounds selectedBounds = spawnConfig.TargetVolumes[0];
        foreach (var volume in spawnConfig.TargetVolumes)
        {
            totalVolume -= volume.GetVolume();
            if (randomWeight >= totalVolume)
            {
                selectedBounds = volume;
                break;
            }
        }

        return new Vector3(Random.Range(selectedBounds.min.x, selectedBounds.max.x), 1.0f, Random.Range(selectedBounds.min.z, selectedBounds.max.z));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
