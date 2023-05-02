using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum TrainSide
{
    Left = 0,
    Right = 1,
}

public struct MailLoverConfig
{
    public PackageType DesiredPackageType;
    public int DesiredPackageCount;
}

public class EntitySquadCoordinator : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject EnemySquadPrefab;
    public GameObject MailLoverSquadPrefab;
    public GameObject TargetPrefab;

    [Header("Spawning")]
    [Range(0, 30.0f)]
    public float EnemyTargetMinDistanceFromTrain = 5.0f;
    public bool SpawningEnabled = true;
    [Range(0, 60.0f)]
    public float SpawnInterval = 5.0f;
    public float SpawnTimer;

    private float MailLoverSpawnInterval;
    public float MailLoverSpawnTimer;

    public int MaxEntityCount = 25;

    private TrainLevelManager _trainLevelManager;
    private Train _theTrain;
    private Inventory _theInventory;

    private GameObject _spawnedEntitiesContainer;
    private List<MailLoverConfig> _mailLoverConfigsToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        _theTrain = FindAnyObjectByType<Train>();
        _trainLevelManager = FindAnyObjectByType<TrainLevelManager>();
        _theInventory = FindAnyObjectByType<Inventory>();

        ResetState();
    }

    private Dictionary<PackageType, int> _cachedInventory = null;
    public void ResetState()
    {
        if (_spawnedEntitiesContainer != null)
        {
            Destroy(_spawnedEntitiesContainer);
            _spawnedEntitiesContainer = null;
        }

        _spawnedEntitiesContainer = new GameObject("Spawned Entities");
        _spawnedEntitiesContainer.transform.parent = transform;
        SpawnTimer = SpawnInterval;

        var multiplier = 1.0f;
        switch (_trainLevelManager.Level)
        {
            case Level.Two:
                multiplier = 0.9f;
                break;
            case Level.Three:
                multiplier = 0.8f;
                break;
            default:
                multiplier = 1.0f;
                break;
        }
        SpawnTimer = SpawnTimer * multiplier;

        _cachedInventory = null;
        _mailLoverConfigsToSpawn = GenerateMailLoverConfigs();
        MailLoverSpawnTimer = 0.1f;
        MailLoverSpawnInterval = (_trainLevelManager.LevelTime - 15.0f) / _mailLoverConfigsToSpawn.Count;
    }

    public void SpawnEnemySquad()
    {
        if (_spawnedEntitiesContainer.transform.childCount >= MaxEntityCount)
        {
            Debug.Log("Too many entities. Not spawning more enemies.");
            return;
        }
        SpawnSquad(EnemySquadPrefab);
    }

    public List<MailLoverConfig> GenerateMailLoverConfigs()
    {
        _cachedInventory = new Dictionary<PackageType, int>(_theInventory.PackageInventory);

        List<MailLoverConfig> configs = new List<MailLoverConfig>();

        while (_cachedInventory.Count > 0)
        {
            var randomPackageType = _cachedInventory.Keys.ToList()[Random.Range(0, _cachedInventory.Keys.Count)];
            var amountAvailable = _cachedInventory[randomPackageType];
            var randomAmountToGrab = Random.Range(1, amountAvailable + 1);

            _cachedInventory[randomPackageType] -= randomAmountToGrab;
            if (_cachedInventory[randomPackageType] <= 0)
            {
                _cachedInventory.Remove(randomPackageType);
            }

            var config = new MailLoverConfig
            {
                DesiredPackageCount = randomAmountToGrab,
                DesiredPackageType = randomPackageType
            };

            configs.Add(config);
        }

        return configs;
    }

    public void SpawnMailLoverSquad()
    {
        if (_mailLoverConfigsToSpawn.Count == 0) { return; }

        var squad = SpawnSquad(MailLoverSquadPrefab);
        if (squad == null) { return; }

        var mailLover = squad.GetComponentInChildren<MailLover>();
        if (mailLover == null) { return; }


        var config = _mailLoverConfigsToSpawn[0];
        _mailLoverConfigsToSpawn.RemoveAt(0);

        Debug.Log("Reserving mail lover for " + config.DesiredPackageCount + " " + config.DesiredPackageType.ToString());

        mailLover.DesiredPackageType = config.DesiredPackageType;
        mailLover.DesiredPackageAmount = config.DesiredPackageCount;
    }

    public GameObject SpawnSquad(GameObject squadPrefab)
    {
        if (!Application.isPlaying) { return null; }

        TrainSide side = TrainSide.Right;
        if (Random.Range(0, 1.0f) < 0.5f) { side = TrainSide.Left; }
        var spawnPosition = SelectSpawnPosition(side);
        var targetPosition = SelectTargetPosition(side);

        var EntitySquadGO = Instantiate(squadPrefab);
        EntitySquadGO.transform.parent = _spawnedEntitiesContainer.transform;
        EntitySquadGO.transform.position = spawnPosition;

        var targetGO = Instantiate(TargetPrefab);
        targetGO.transform.position = targetPosition;

        var EntitySquad = EntitySquadGO.GetComponent<EntitySquad>();
        EntitySquad.SetTarget(targetGO.transform);

        return EntitySquadGO;
    }

    public Vector3 SelectSpawnPosition(TrainSide side)
    {
        // Z Val
        bool spawnFromSide = Random.Range(0, 1.0f) > 0.66f;
        var forwardExtent = _theTrain.ForwardExtent();
        var aftExtent = _theTrain.AftExtent();

        Ray horizontalExtentRayTop = Camera.main.ViewportPointToRay(new Vector3(0.5f, 1f, 0));
        Ray horizontalExtentRayBottom = Camera.main.ViewportPointToRay(new Vector3(.5f, 0f, 0));
        var hitPointTop = CastToTheEternalPlane(horizontalExtentRayTop);
        var hitPointBottom = CastToTheEternalPlane(horizontalExtentRayBottom);
        var longitudinalRange = Vector3.Distance(hitPointTop, hitPointBottom);

        var zVal = 0.0f;
        if (spawnFromSide)
        {
            zVal = aftExtent.z + Random.Range(0, longitudinalRange);
        }
        else
        {
            bool spawnFromTop = Random.Range(0, 1.0f) > 0.5f;
            if (spawnFromTop)
            {
                zVal = forwardExtent.z + longitudinalRange;
            }
            else
            {
                zVal = aftExtent.z - longitudinalRange;
            }
        }


        // X Val
        Ray horizontalExtentRayLeft = Camera.main.ViewportPointToRay(new Vector3(0, 1, 0));
        Ray horizontalExtentRayRight = Camera.main.ViewportPointToRay(new Vector3(1, 1, 0));
        var hitPointLeft = CastToTheEternalPlane(horizontalExtentRayLeft);
        var hitPointRight = CastToTheEternalPlane(horizontalExtentRayRight);
        var maxLateralRange = Vector3.Distance(hitPointLeft, hitPointRight);
        var sign = side == TrainSide.Left ? -1 : 1;
        var xVal = 0.0f;
        if (spawnFromSide)
        {
            xVal = sign * maxLateralRange;
        }
        else
        {
            xVal = forwardExtent.x + sign * Random.Range(0, maxLateralRange);
        }

        return new Vector3(xVal, forwardExtent.y, zVal);
    }

    private Vector3 CastToTheEternalPlane(Ray ray)
    {
        int layerMask = 1 << LayerConstants.TheEternalPlane;

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow, 60.0f);
            return hit.point;
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.white, 60.0f);
            return Vector3.zero;
        }
    }

    public Vector3 SelectTargetPosition(TrainSide side)
    {
        Vector3 targetPosition = Vector3.zero;
        bool selectedValidSpawnPosition = false;
        int iterationCount = 0;
        const int maxIterationCount = 20;
        do
        {
            iterationCount += 1;

            // Z Val
            var forwardExtent = _theTrain.ForwardExtent();
            var aftExtent = _theTrain.AftExtent();
            var longitudinalRange = Vector3.Distance(forwardExtent, aftExtent);
            var zVal = aftExtent.z + Random.Range(0, longitudinalRange);

            // X Val
            var viewportPoint = Camera.main.WorldToViewportPoint(forwardExtent);
            Ray horizontalExtentRay = Camera.main.ViewportPointToRay(new Vector3(side == TrainSide.Left ? 0 : 1, viewportPoint.y, 0));
            var hitPoint = CastToTheEternalPlane(horizontalExtentRay);
            var maxLateralRange = Vector3.Distance(forwardExtent, hitPoint);
            var trainBuffer = EnemyTargetMinDistanceFromTrain;
            var sign = side == TrainSide.Left ? -1 : 1;
            var xVal = forwardExtent.x + sign * Random.Range(trainBuffer, maxLateralRange);

            targetPosition = new Vector3(xVal, forwardExtent.y, zVal);

            var targetPositionInViewportSpace = Camera.main.WorldToViewportPoint(targetPosition);
            if (targetPositionInViewportSpace.x > 0.05 
                && targetPositionInViewportSpace.x < 0.95 
                && targetPositionInViewportSpace.y > 0.05 
                && targetPositionInViewportSpace.y < 0.95)
            {
                selectedValidSpawnPosition = true;
            }
        } while (!selectedValidSpawnPosition && iterationCount < maxIterationCount);

        return targetPosition;
    }

    public void SpawnNextSquad()
    {
        SpawnEnemySquad();
    }

    // Update is called once per frame
    void Update()
    {
        if (_trainLevelManager.State != TrainLevelState.Gameplay) { return; }
        
        if (SpawningEnabled)
        {
            if (SpawnTimer < 0)
            {
                SpawnTimer += SpawnInterval;
                SpawnNextSquad();
            }

            SpawnTimer -= Time.deltaTime;

            if (MailLoverSpawnTimer < 0)
            {
                MailLoverSpawnTimer += MailLoverSpawnInterval;
                SpawnMailLoverSquad();
            }

            MailLoverSpawnTimer -= Time.deltaTime;
        }
    }
}
