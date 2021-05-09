using UnityEngine;

public class CubeGenerateManager
{
    private CubeEntity _generatedCube01;
    private CubeEntity _generatedCube02;

    public CubeEntity GenerateCube(CubeEntity prefab, int power, Owner owner, Vector3 spawnPosition)
    {
        CubeEntity generatedCube = GameObject.Instantiate(prefab);
        generatedCube.SetNumber(power);
        generatedCube.SetGraphic();

        if (owner == Owner.Player)
            generatedCube.SetLayer(Strings.PLAYER_CUBE_LAYER);
        else if (owner == Owner.Enemy)
            generatedCube.SetLayer(Strings.ENEMY_CUBE_LAYER);

        generatedCube.SetPosition(spawnPosition);

        generatedCube.ShootAction += OnShootAction;

        if (_generatedCube01 == null)
        {
            _generatedCube01 = generatedCube;
            return _generatedCube01;
        }
        else
        {
            _generatedCube02 = generatedCube;
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
