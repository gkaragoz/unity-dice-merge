using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CubeEntity), typeof(DrawTrajectory))]
public class CubeShootManager : MonoBehaviour
{
    [Header("Left Shooting Clamps")]
    [SerializeField] float _leftMinX = -200f;
    [SerializeField] float _leftMaxX = 200f;
    [SerializeField] float _leftMinY = 300f;
    [SerializeField] float _leftMaxY = 640f;

    [Header("Right Shooting Clamps")]
    [SerializeField] float _rightMinX = -200f;
    [SerializeField] float _rightMaxX = 200f;
    [SerializeField] float _rightMinY = 300f;
    [SerializeField] float _rightMaxY = 640f;

    [Header("Other Settings")]
    [SerializeField] private float _inputMultiplier = 2f;
    [SerializeField] private Rigidbody _rb;

    private Vector3 _mousePressDownPos;
    private Vector3 _forceVector;

    private bool _isShooting = false;
    private bool _hasShootedOnce = false;

    private DrawTrajectory _drawTrajectory;
    private CubeEntity _cubeEntity;
    private float _minX;
    private float _maxX;
    private float _minY;
    private float _maxY;

    public event UnityAction ShootAction;

    private void Start()
    {
        _cubeEntity = GetComponent<CubeEntity>();
        _drawTrajectory = GetComponent<DrawTrajectory>();

        CalculateMinMaxClamps();
    }

    private void CalculateMinMaxClamps()
    {
        if (_cubeEntity.IsLeftOne)
        {
            _minX = _leftMinX;
            _maxX = _leftMaxX;

            _minY = _leftMinY;
            _maxY = _leftMaxY;
        }
        else
        {
            _minX = _rightMinX;
            _maxX = _rightMaxX;

            _minY = _rightMinY;
            _maxY = _rightMaxY;
        }
    }

    private void Update()
    {
        if (_hasShootedOnce)
            return;
        if (!_cubeEntity.HasSelected)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            _mousePressDownPos = Input.mousePosition;

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
                _drawTrajectory.UpdateTrajectory(_forceVector, _rb, _rb.transform.position);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _drawTrajectory.HideLine();

            Shoot(_forceVector);

            _forceVector = Vector3.zero;

            _isShooting = false;
            _hasShootedOnce = true;

            ShootAction?.Invoke();
        }
    }

    public void Shoot(Vector3 Force)
    {
        _rb.isKinematic = false;
        _rb.useGravity = true;
        _rb.AddForce(Force);
        _rb.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 30f);
    }

    private Vector3 GetRandomInput(bool isReversedDirection)
    {
        Vector2 input = Vector2.zero;

        input.x = Random.Range(_minX, _maxX);
        input.y = Random.Range(_minY, _maxY);

        input.x = Mathf.Clamp(input.x, _minX, _maxX);
        input.y = Mathf.Clamp(input.y, _minY, _maxY);

        Vector3 randomInput = new Vector3(input.x, input.y, input.y) * _inputMultiplier;

        if (isReversedDirection)
            randomInput.z *= -1;

        return randomInput;
    }

    private Vector3 GetStraightInput(bool isReversedDirection)
    {
        Vector2 input = Vector2.zero;

        input.x = _maxX / 2;
        input.y = _maxY / 2;

        input.x = Mathf.Clamp(input.x, _minX, _maxX);
        input.y = Mathf.Clamp(input.y, _minY, _maxY);

        Vector3 finalInput = new Vector3(input.x, input.y, input.y) * _inputMultiplier;

        if (isReversedDirection)
            finalInput.z *= -1;

        return finalInput;
    }

    public void ShootByManual(bool isReversedDirection, bool debugStraightInput = false)
    {
        if (_hasShootedOnce)
            return;

        if (debugStraightInput)
            _forceVector = GetStraightInput(isReversedDirection);
        else
            _forceVector = GetRandomInput(isReversedDirection);

        _rb.isKinematic = false;
        _rb.useGravity = true;
        _rb.AddForce(_forceVector);
        _rb.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 30f);

        _hasShootedOnce = true;

        ShootAction?.Invoke();
    }
}
