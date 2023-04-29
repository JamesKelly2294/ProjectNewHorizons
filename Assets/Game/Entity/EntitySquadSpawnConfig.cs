using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public static class BoundsExtensions
{
    public static float GetVolume(this Bounds b)
    {
        return b.size.x * b.size.y * b.size.z;
    }
}

public class EntitySquadSpawnConfig : MonoBehaviour
{
    [Header("Do Not Edit Manually!")]
    public List<Bounds> TargetVolumes;
    public List<Bounds> SpawnVolumes;
    
    private List<MeshRenderer> _targetVolumesMRs = new List<MeshRenderer>();
    private List<MeshRenderer> _spawnVolumesMRs = new List<MeshRenderer>();

    private bool _isDirty = false;

    private float _totalTargetVolume;
    public float TotalTargetVolume
    {
        get
        {
            if (_isDirty)
            {
                Recalculate();
            }

            return _totalTargetVolume;
        }
    }

    private float _totalSpawnVolume;
    public float TotalSpawnVolume
    {
        get
        {
            if (_isDirty)
            {
                Recalculate();
            }

            return _totalSpawnVolume;
        }
    }

    void Recalculate()
    {
        TargetVolumes = _targetVolumesMRs.Select(i => i.bounds).ToList();
        SpawnVolumes = _spawnVolumesMRs.Select(i => i.bounds).ToList();

        _totalTargetVolume = TargetVolumes.Select(i => i.GetVolume()).Aggregate((sum, next) => sum + next);
        _totalSpawnVolume = SpawnVolumes.Select(i => i.GetVolume()).Aggregate((sum, next) => sum + next);

        _isDirty = false;
    }

    // Start is called before the first frame update
    void Awake()
    {
        var targetVolumesParent = transform.Find("Target Volumes");
        CollectMeshRenderersForParent(targetVolumesParent, _targetVolumesMRs);

        var spawnVolumesParent = transform.Find("Spawn Volumes");
        CollectMeshRenderersForParent(spawnVolumesParent, _spawnVolumesMRs);

        Recalculate();
    }

    void CollectMeshRenderersForParent(Transform parent, List<MeshRenderer> collection)
    {
        if (parent == null)
        {
            return;
        }

        for (var i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            var spawnConfig = child.GetComponent<MeshRenderer>();
            if (spawnConfig != null)
            {
                collection.Add(spawnConfig);
            }
        }
    }
}
