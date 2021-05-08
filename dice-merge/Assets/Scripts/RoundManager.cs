using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public enum Owner
    {
        Player,
        Enemy
    }

    private List<CubeEntity> _playerCubeEntities = new List<CubeEntity>();
    private List<CubeEntity> _enemyCubeEntities = new List<CubeEntity>();
    private MergeContainer _activeMergeContainer = new MergeContainer();

    [SerializeField]
    private Transform[] _eglence;

    private void Start()
    {
        GenerateCube();
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

    private int GetTotalNumberInArea(Owner areaOwner)
    {
        int totalAmount = 0;

        if (areaOwner == Owner.Player)
            totalAmount = _enemyCubeEntities.Sum(a => a.Number);
        else if (areaOwner == Owner.Enemy)
            totalAmount = _playerCubeEntities.Sum(a => a.Number);

        return totalAmount;
    }

    private void OnCubeEntityEnteredArea(CubeEntity placedCube)
    {
        Debug.LogWarning("OnCubeEntityEnteredArea: ");
        Debug.LogWarning("Name:" + placedCube.gameObject.name);
        Debug.LogWarning("Owner:" + placedCube.Owner);
        Debug.LogWarning("Placed Owner:" + placedCube.PlacedAreaOwner);
        Debug.LogWarning("Status:" + placedCube.Status);
        Debug.LogWarning("Total number at area: " + GetTotalNumberInArea(placedCube.PlacedAreaOwner));
    }

    private void OnCubeEntityShooted(CubeEntity shootedCube)
    {
        Debug.LogWarning("OnCubeEntityShooted: ");
        Debug.LogWarning("Name:" + shootedCube.gameObject.name);
        Debug.LogWarning("Owner:" + shootedCube.Owner);
        Debug.LogWarning("Status:" + shootedCube.Status);

        DOVirtual.DelayedCall(0.1f, () =>
        {
            GenerateCube();
        });
    }

    private void GenerateCube()
    {
        CubeEntity generatedCube = CubeGenerateManager.instance.GenerateCube();
        generatedCube.ShootAction += OnCubeEntityShooted;
        generatedCube.EnterAreaAction += OnCubeEntityEnteredArea;
        generatedCube.MergingAction += OnMergingAction;

        if (generatedCube.Owner == Owner.Player)
            _playerCubeEntities.Add(generatedCube);
        else if (generatedCube.Owner == Owner.Enemy)
            _enemyCubeEntities.Add(generatedCube);
    }

    private void OnDestroy()
    {
        foreach (var item in _playerCubeEntities)
        {
            item.ShootAction -= OnCubeEntityShooted;
            item.EnterAreaAction -= OnCubeEntityEnteredArea;
            item.MergingAction -= OnMergingAction;
        }
    }
}
