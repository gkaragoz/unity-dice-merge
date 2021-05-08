using UnityEngine;

[RequireComponent(typeof(DrawTrajectory))]
public class CubeShootManager : MonoBehaviour
{
    [Header("Shooting Clamps")]
    [SerializeField] float _minX = -200f;
    [SerializeField] float _maxX = 200f;
    [SerializeField] float _minY = 300f;
    [SerializeField] float _maxY = 640f;

    [Header("Other Settings")]
    [SerializeField] private float _inputMultiplier = 2f;

    private Vector3 _mousePressDownPos;
    private Vector3 _forceVector;

    private bool _isShooting = false;

    private DrawTrajectory _drawTrajectory;
    private CubeEntity _selectedCubeEntity;

    private void Start()
    {
        _drawTrajectory = GetComponent<DrawTrajectory>();
    }

    private void Update()
    {
        if (!CubeGenerateManager.instance.HasCube())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            _mousePressDownPos = Input.mousePosition;

            _selectedCubeEntity = CubeGenerateManager.instance.GetGeneratedCube();

            _isShooting = true;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 input = _mousePressDownPos - Input.mousePosition;

            if (input.y < 0)
                input.x *= -1f;

            input.y = Mathf.Abs(input.y);

            float initialHeight = 2688f;
            input *= initialHeight / (float)Screen.height;

            input.x = Mathf.Clamp(input.x, _minX, _maxX);
            input.y = Mathf.Clamp(input.y, _minY, _maxY);

            _forceVector = Vector3.Lerp(_forceVector, new Vector3(input.x, input.y, input.y) * _inputMultiplier, 0.5f);

            if (_isShooting && _forceVector != Vector3.zero)
            {
                _drawTrajectory.UpdateTrajectory(_forceVector, _selectedCubeEntity.Rb, _selectedCubeEntity.GetPosition());
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _drawTrajectory.HideLine();

            Shoot(_forceVector);

            _forceVector = Vector3.zero;

            _isShooting = false;
        }
    }

    private void Shoot(Vector3 Force)
    {
        _selectedCubeEntity.Rb.isKinematic = false;
        _selectedCubeEntity.Rb.useGravity = true;
        _selectedCubeEntity.Rb.AddForce(Force);
        _selectedCubeEntity.Rb.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 30f);
    }
}
