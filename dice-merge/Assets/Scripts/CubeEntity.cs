using System;
using UnityEngine;

public class CubeEntity : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rb;
    [SerializeField]
    private int _number;

    public int Number { get => _number; }
    public Rigidbody Rb { get => _rb; }

    public void SetNumber(int number)
    {
        _number = number;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetLayer(string layerName)
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);
    }
}
