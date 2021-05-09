using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    [Header("References")]
    [SerializeField]
    private CubeEntity _cubePrefab;
    [SerializeField]
    private Transform _spawnTransform01;
    [SerializeField]
    private Transform _spawnTransform02;

    [Header("Shoot Settings")]
    [SerializeField]
    private bool _isDebug = false;
    [SerializeField]
    private Difficulty _difficulty = Difficulty.Normal;

    private List<CubeEntity> _enemyCubeEntities = new List<CubeEntity>();
    private MergeContainer _activeMergeContainer = new MergeContainer();

    private Utils _utils = new Utils();
    private CubeGenerateManager _cubeGenerateManager = new CubeGenerateManager();

    private CubeEntity _generatedCube01;
    private CubeEntity _generatedCube02;

    private float _shootRate = 1f;
    private float _nextShootTime;

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
        SetDifficultyVariables();

        GenerateCube(1, _spawnTransform01.position);
        GenerateCube(1, _spawnTransform02.position);
    }

    private void Update()
    {
        if (Time.time > _nextShootTime && IsReadyToShoot())
        {
            _nextShootTime = Time.time + _shootRate;

            Shoot();
        }
    }

    private bool IsReadyToShoot()
    {
        return _generatedCube01 != null && _generatedCube02 != null;
    }

    private void SetDifficultyVariables()
    {
        switch (_difficulty)
        {
            case Difficulty.Noob:
                _shootRate = 5f;
                break;
            case Difficulty.Normal:
                _shootRate = 3f;
                break;
            case Difficulty.Pro:
                _shootRate = 1f;
                break;
        }
    }

    private void Shoot()
    {
        CubeEntity selectedCube = null;
        int selectedRandomCube = 0;

        if (!_isDebug)
            selectedRandomCube = Random.Range(0, 2);

        if (selectedRandomCube == 0)
            selectedCube = _generatedCube01;
        else if (selectedRandomCube == 1)
            selectedCube = _generatedCube02;

        selectedCube.ShootManager.ShootByManual(true, _isDebug);

        _generatedCube01 = null;
        _generatedCube02 = null;
    }

    private void OnMergingAction(CubeEntity mergingCube)
    {
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

        int maxNumberInArea = _utils.GetMaxNumberInArea(_enemyCubeEntities);
        int[] randomPowers = _utils.GenerateRandomPowers(maxNumberInArea);

        DOVirtual.DelayedCall(0.3f, () =>
        {
            GenerateCube(randomPowers[0], _spawnTransform01.position);
            GenerateCube(randomPowers[1], _spawnTransform02.position);
        });
    }

    private void OnCubeEntityEnterWrongAreaAction(CubeEntity enteredCube)
    {
        if (enteredCube.Owner == Owner.Enemy && _enemyCubeEntities.Contains(enteredCube))
            _enemyCubeEntities.Remove(enteredCube);

        GenerateCubes();
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
    }
    private void OnCollideWithEnemyCubeAction(CubeEntity myCube, CubeEntity enemyCube)
    {
        GenerateCubes();
    }

    private void GenerateCubes()
    {
        int maxNumberInArea = _utils.GetMaxNumberInArea(_enemyCubeEntities);
        int[] randomPowers = _utils.GenerateRandomPowers(maxNumberInArea);

        DOVirtual.DelayedCall(0.3f, () =>
        {
            GenerateCube(randomPowers[0], _spawnTransform01.position);
            GenerateCube(randomPowers[1], _spawnTransform02.position);
        });
    }

    private void GenerateCube(int power, Vector3 spawnPosition)
    {
        if (_generatedCube01 != null && _generatedCube02 != null)
            return;

        CubeEntity generatedCube = _cubeGenerateManager.GenerateCube(_cubePrefab, power, Owner.Enemy, spawnPosition);
        generatedCube.ShootAction += OnCubeEntityShooted;
        generatedCube.EnterAreaAction += OnCubeEntityEnteredArea;
        generatedCube.EnterWrongAreaAction += OnCubeEntityEnterWrongAreaAction;
        generatedCube.StartMergingAction += OnMergingAction;
        generatedCube.MergingActionFinished += OnMergingActionFinished;
        generatedCube.DestroyAction += OnCubeDestroyed;
        generatedCube.CollideWithEnemyCubeAction += OnCollideWithEnemyCubeAction;

        if (_generatedCube01 == null)
            _generatedCube01 = generatedCube;
        else if (_generatedCube02 == null)
            _generatedCube02 = generatedCube;
    }

    private void OnDestroy()
    {
        foreach (var item in _enemyCubeEntities)
        {
            item.ShootAction -= OnCubeEntityShooted;
            item.EnterAreaAction -= OnCubeEntityEnteredArea;
            item.EnterWrongAreaAction -= OnCubeEntityEnterWrongAreaAction;
            item.StartMergingAction -= OnMergingAction;
            item.MergingActionFinished -= OnMergingActionFinished;
            item.DestroyAction -= OnCubeDestroyed;
            item.CollideWithEnemyCubeAction -= OnCollideWithEnemyCubeAction;
        }
    }
}
