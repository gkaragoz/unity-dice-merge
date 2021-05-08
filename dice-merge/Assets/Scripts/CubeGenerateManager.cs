using UnityEngine;

public class CubeGenerateManager : MonoBehaviour
{
    public static CubeGenerateManager instance;

    [SerializeField]
    private CubeEntity _cubePrefab;
    [SerializeField]
    private string _playerCubeLayer;
    [SerializeField]
    private string _enemyCubeLayer;

    [SerializeField]
    private Transform _playerStartTransform;
    [SerializeField]
    private Transform _enemyStartTransform;

    private CubeEntity _generatedCube;

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

    public CubeEntity GenerateCube()
    {
        _generatedCube = Instantiate(_cubePrefab, transform);
        _generatedCube.SetNumber(2);
        _generatedCube.SetLayer(_playerCubeLayer);

        _generatedCube.SetPosition(_playerStartTransform.position);

        return _generatedCube;
    }

    public bool HasCube()
    {
        return _generatedCube != null;
    }

    public CubeEntity GetGeneratedCube()
    {
        return _generatedCube;
    }

}
