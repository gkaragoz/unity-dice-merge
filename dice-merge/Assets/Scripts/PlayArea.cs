using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayArea : MonoBehaviour
{
    public enum PlayerType
    {
        Player,
        Enemy
    }

    [SerializeField]
    private PlayerType _type;

    [Header("Debug")]
    [SerializeField]
    private List<CubeEntity> _cubeEntities = new List<CubeEntity>();

    public PlayerType Type { get => _type; }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Cube"))
            return;

        var cubeEntity = other.gameObject.GetComponent<CubeEntity>();
        if (cubeEntity == null)
            return;

        if (Type == PlayerType.Player)
        {
            if (cubeEntity.Layer == Strings.PLAYER_CUBE_LAYER)
                return;
        }
        else if (Type == PlayerType.Enemy)
        {
            if (cubeEntity.Layer == Strings.ENEMY_CUBE_LAYER)
                return;
        }

        foreach (var item in _cubeEntities)
            if (item == cubeEntity)
                return;

        _cubeEntities.Add(cubeEntity);
        cubeEntity.DestroyAction += OnCubeEntityDestroyed;
    }

    private void OnCubeEntityDestroyed(CubeEntity entity)
    {
        _cubeEntities.Remove(entity);
    }
}
