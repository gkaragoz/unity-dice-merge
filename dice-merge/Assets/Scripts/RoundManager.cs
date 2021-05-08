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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CubeEntity generatedCube = CubeGenerateManager.instance.GenerateCube();
            generatedCube.ShootAction += OnCubeEntityShooted;
            generatedCube.PlacedInAreaAction += OnCubeEntityPlacedInArea;

            if (generatedCube.Owner == Owner.Player)
                _playerCubeEntities.Add(generatedCube);
            else if (generatedCube.Owner == Owner.Enemy)
                _enemyCubeEntities.Add(generatedCube);
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

    private void OnCubeEntityPlacedInArea(CubeEntity placedCube)
    {
        Debug.LogWarning("OnCubeEntityPlacedInArea: ");
        Debug.LogWarning("Name:" + placedCube.gameObject.name);
        Debug.LogWarning("Owner:" + placedCube.Owner);
        Debug.LogWarning("Placed Owner:" + placedCube.PlacedAreaOwner);
        Debug.LogWarning("Total number at area: " + GetTotalNumberInArea(placedCube.PlacedAreaOwner));
    }

    private void OnCubeEntityShooted(CubeEntity shootedCube)
    {
        Debug.LogWarning("OnCubeEntityShooted: ");
        Debug.LogWarning("Name:" + shootedCube.gameObject.name);
        Debug.LogWarning("Owner:" + shootedCube.Owner);
        Debug.LogWarning("HasPlacedInArea:" + shootedCube.HasPlacedInArea);
    }

    private void OnDestroy()
    {
        foreach (var item in _playerCubeEntities)
        {
            item.ShootAction -= OnCubeEntityShooted;
            item.PlacedInAreaAction -= OnCubeEntityPlacedInArea;
        }
    }
}
