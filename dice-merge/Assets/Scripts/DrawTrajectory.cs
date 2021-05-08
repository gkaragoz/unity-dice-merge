using System.Collections.Generic;
using UnityEngine;

public class DrawTrajectory : MonoBehaviour
{
    [SerializeField]
    private int _lineSegmentCount = 20;
    [SerializeField]
    private GameObject _trajectoryPrefab;
    [SerializeField]
    private LayerMask _layerMask;

    private GameObject[] _balls;
    private List<Vector3> _linePoints = new List<Vector3>();

    private void Awake()
    {
        _balls = new GameObject[_lineSegmentCount];
        for (int ii = 0; ii < _lineSegmentCount; ii++)
        {
            _balls[ii] = Instantiate(_trajectoryPrefab, transform);
            _balls[ii].transform.position = new Vector3(100, 0, 0);
        }
    }

    public void UpdateTrajectory(Vector3 forceVector, Rigidbody rigidBody, Vector3 startingPoint)
    {
        Vector3 velocity = (forceVector / rigidBody.mass) * Time.fixedDeltaTime;

        float flightDuration = (2 * velocity.y) / Physics.gravity.y;

        float stepTime = flightDuration / (float)_lineSegmentCount;

        _linePoints.Clear();
        _linePoints.Add(startingPoint);

        for (int i = 1; i < _lineSegmentCount; i++)
        {
            float stepTimePassed = stepTime * i;

            Vector3 MovementVector = new Vector3(
                velocity.x * stepTimePassed,
                velocity.y * stepTimePassed - (0.5f * Physics.gravity.y * stepTimePassed * stepTimePassed),
                velocity.z * stepTimePassed
                );

            MovementVector.z = velocity.z > 0 ? 1 * MovementVector.z : -1 * MovementVector.z;

            Vector3 newPointOnLine = -MovementVector + startingPoint;

            RaycastHit hit;

            if (Physics.Raycast(_linePoints[i - 1], newPointOnLine - _linePoints[i - 1], out hit, (newPointOnLine - _linePoints[i - 1]).magnitude, _layerMask))
            {
                _linePoints.Add(hit.point);
                break;
            }

            _linePoints.Add(newPointOnLine);
        }

        ShowLines();
    }

    private void ShowLines()
    {
        for (int i = 0; i < _linePoints.Count; i++)
        {
            _balls[i].SetActive(true);
            _balls[i].transform.position = _linePoints[i];
        }

        if (_balls.Length > _linePoints.Count)
        {
            for (int i = _linePoints.Count; i < _balls.Length; i++)
            {
                _balls[i].SetActive(false);
            }
        }
    }

    public void HideLine()
    {
        for (int i = 0; i < _balls.Length; i++)
        {
            _balls[i].SetActive(false);
        }
    }

}
