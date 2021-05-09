using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [SerializeField]
    private CubeEntity _cubePrefab;
    [SerializeField]
    private Transform _spawnTransform01;
    [SerializeField]
    private Transform _spawnTransform02;

    public event UnityAction<CubeEntity> CubeSelection;

    private List<CubeEntity> _playerCubeEntities = new List<CubeEntity>();
    private MergeContainer _activeMergeContainer = new MergeContainer();

    private Utils _utils = new Utils();
    private CubeGenerateManager _cubeGenerateManager = new CubeGenerateManager();

    private CubeEntity _generatedCube01;
    private CubeEntity _generatedCube02;

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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                CubeEntity selectedCube = hit.transform.GetComponent<CubeEntity>();
                if (selectedCube != null)
                    CubeSelection?.Invoke(selectedCube);
            }
        }
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

    private void OnCubeEntityEnteredArea(CubeEntity enteredCube)
    {
        if (enteredCube.Owner == Owner.Player)
            _playerCubeEntities.Add(enteredCube);

        //Debug.LogWarning("OnCubeEntityEnteredArea: ");
        //Debug.LogWarning("Name:" + enteredCube.gameObject.name);
        //Debug.LogWarning("Owner:" + enteredCube.Owner);
        //Debug.LogWarning("Placed Owner:" + enteredCube.PlacedAreaOwner);
        //Debug.LogWarning("Status:" + enteredCube.Status);
        //Debug.LogWarning("Total number at area: " + _utils.GetTotalNumberInArea(_playerCubeEntities));

        GenerateCubes();
    }


    private void OnCubeEntityEnterWrongAreaAction(CubeEntity enteredCube)
    {
        if (enteredCube.Owner == Owner.Player && _playerCubeEntities.Contains(enteredCube))
            _playerCubeEntities.Remove(enteredCube);

        GenerateCubes();
    }

    private void OnCubeEntityShooted(CubeEntity shootedCube)
    {
        _generatedCube01 = null;
        _generatedCube02 = null;
    }

    private void OnMergingActionFinished(CubeEntity liveCube, CubeEntity mergedCube)
    {
        Debug.LogWarning("OnMergingActionFinished:");

        if (mergedCube.Owner == Owner.Player)
            _playerCubeEntities.Remove(mergedCube);

        mergedCube.Destroy();

        Debug.LogWarning("Total number at area: " + _utils.GetTotalNumberInArea(_playerCubeEntities));
    }

    private void OnCubeDestroyed(CubeEntity destroyedCube)
    {
        if (destroyedCube.Owner == Owner.Player && _playerCubeEntities.Contains(destroyedCube))
            _playerCubeEntities.Remove(destroyedCube);

        //Debug.LogWarning("OnCubeDestroyed:");
        //Debug.LogWarning("Name:" + destroyedCube.gameObject.name);
        //Debug.LogWarning("Owner:" + destroyedCube.Owner);
    }

    private void OnCollideWithEnemyCubeAction(CubeEntity myCube, CubeEntity enemyCube)
    {
        GenerateCubes();
    }

    private void GenerateCubes()
    {
        int maxNumberInArea = _utils.GetMaxNumberInArea(_playerCubeEntities);
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

        CubeEntity generatedCube = _cubeGenerateManager.GenerateCube(_cubePrefab, power, Owner.Player, spawnPosition);
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
        foreach (var item in _playerCubeEntities)
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
