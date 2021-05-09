using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    [SerializeField]
    private CubeEntity _cubePrefab;
    [SerializeField]
    private Transform _spawnTransform01;
    [SerializeField]
    private Transform _spawnTransform02;

    private List<CubeEntity> _enemyCubeEntities = new List<CubeEntity>();
    private MergeContainer _activeMergeContainer = new MergeContainer();

    private Utils _utils = new Utils();
    private CubeGenerateManager _cubeGenerateManager = new CubeGenerateManager();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GenerateCube(1, _spawnTransform01.position);
        GenerateCube(1, _spawnTransform02.position);
    }

    private void OnMergingAction(CubeEntity mergingCube)
    {
        //Debug.LogWarning("Merging cube: ");
        //Debug.LogWarning("Name:" + mergingCube.gameObject.name);
        //Debug.LogWarning("Status:" + mergingCube.Status);

        if (_activeMergeContainer.HasRoom())
        {
            _activeMergeContainer.AddEntity(mergingCube);
            return;
        }
        else
        {
            _activeMergeContainer = new MergeContainer();
            _activeMergeContainer.AddEntity(mergingCube);
        }
    }

    private void OnCubeEntityEnteredArea(CubeEntity placedCube)
    {
        if (placedCube.Owner == Owner.Enemy)
            _enemyCubeEntities.Add(placedCube);

        //Debug.LogWarning("OnCubeEntityEnteredArea: ");
        //Debug.LogWarning("Name:" + placedCube.gameObject.name);
        //Debug.LogWarning("Owner:" + placedCube.Owner);
        //Debug.LogWarning("Placed Owner:" + placedCube.PlacedAreaOwner);
        //Debug.LogWarning("Status:" + placedCube.Status);
        //Debug.LogWarning("Total number at area: " + GetTotalNumberInArea(placedCube.PlacedAreaOwner));

        int maxNumberInArea = _utils.GetMaxNumberInArea(_enemyCubeEntities);
        int[] randomPowers = _utils.GenerateRandomPowers(maxNumberInArea);

        DOVirtual.DelayedCall(0.3f, () =>
        {
            GenerateCube(randomPowers[0], _spawnTransform01.position);
            GenerateCube(randomPowers[1], _spawnTransform02.position);
        });
    }

    private void OnCubeEntityShooted(CubeEntity shootedCube)
    {
    }

    private void OnMergingActionFinished(CubeEntity liveCube, CubeEntity mergedCube)
    {
        Debug.LogWarning("OnMergingActionFinished:");

        if (mergedCube.Owner == Owner.Enemy)
            _enemyCubeEntities.Remove(mergedCube);

        mergedCube.Destroy();

        Debug.LogWarning("Total number at area: " + _utils.GetTotalNumberInArea(_enemyCubeEntities));
    }

    private void OnCubeDestroyed(CubeEntity destroyedCube)
    {
        if (destroyedCube.Owner == Owner.Enemy && _enemyCubeEntities.Contains(destroyedCube))
            _enemyCubeEntities.Remove(destroyedCube);

        //Debug.LogWarning("OnCubeDestroyed:");
        //Debug.LogWarning("Name:" + destroyedCube.gameObject.name);
        //Debug.LogWarning("Owner:" + destroyedCube.Owner);
    }

    private void GenerateCube(int power, Vector3 spawnPosition)
    {
        CubeEntity generatedCube = _cubeGenerateManager.GenerateCube(_cubePrefab, power, Owner.Enemy, spawnPosition);
        generatedCube.ShootAction += OnCubeEntityShooted;
        generatedCube.EnterAreaAction += OnCubeEntityEnteredArea;
        generatedCube.StartMergingAction += OnMergingAction;
        generatedCube.MergingActionFinished += OnMergingActionFinished;
        generatedCube.DestroyAction += OnCubeDestroyed;
    }

    private void OnDestroy()
    {
        foreach (var item in _enemyCubeEntities)
        {
            item.ShootAction -= OnCubeEntityShooted;
            item.EnterAreaAction -= OnCubeEntityEnteredArea;
            item.StartMergingAction -= OnMergingAction;
            item.MergingActionFinished -= OnMergingActionFinished;
            item.DestroyAction -= OnCubeDestroyed;
        }
    }
}
