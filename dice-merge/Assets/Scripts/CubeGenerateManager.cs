using System;
using UnityEngine;

public class CubeGenerateManager : MonoBehaviour
{
    public static CubeGenerateManager instance;

    [SerializeField]
    private CubeEntity _cubePrefab;

    [SerializeField]
    private Transform _playerStartTransform01;
    [SerializeField]
    private Transform _playerStartTransform02;
    [SerializeField]
    private Transform _enemyStartTransform;

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

    public CubeEntity GenerateCube(int power)
    {
        CubeEntity generatedCube = Instantiate(_cubePrefab, transform);
        generatedCube.SetNumber(power);
        generatedCube.SetGraphic();
        generatedCube.SetLayer(Strings.PLAYER_CUBE_LAYER);

        generatedCube.ShootAction += OnShootAction;

        if (_generatedCube01 == null)
        {
            _generatedCube01 = generatedCube;
            _generatedCube01.SetPosition(_playerStartTransform01.position);

            return _generatedCube01;
        }
        else
        {
            _generatedCube02 = generatedCube;
            _generatedCube02.SetPosition(_playerStartTransform02.position);

            return _generatedCube02;
        }
    }

    private void OnShootAction(CubeEntity shootedCube)
    {
        shootedCube.ShootAction -= OnShootAction;

        if (_generatedCube01 != null && _generatedCube01 != shootedCube)
            _generatedCube01.Destroy();
        else if (_generatedCube02 != null && _generatedCube02 != shootedCube)
            _generatedCube02.Destroy();

        _generatedCube01 = null;
        _generatedCube02 = null;
    }
}
