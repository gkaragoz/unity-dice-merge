using UnityEngine;
using UnityEngine.Events;

public class CubeEntity : MonoBehaviour
{
    [SerializeField]
    private int _number;
    [SerializeField]
    private CubeEntityGraphic[] _graphics;
    [SerializeField]
    private Rigidbody _rb;
    [SerializeField]
    private BoxCollider _collider;

    public int Number { get => _number; }
    public Rigidbody Rb { get => _rb; }

    private CubeEntityGraphic _selectedGraphic;
    private Vector3 _yOffset;
    private string _layer;

    public event UnityAction<CubeEntity> DestroyAction;
    public string Layer { get => _layer; }

    private void CloseAllGraphics()
    {
        foreach (var item in _graphics)
            item.Close();
    }

    private void SetCollider()
    {
        _collider.size = _selectedGraphic.Scale;
    }

    private void SetPositionBySelectedGraphic()
    {
        float scaleY = _selectedGraphic.Scale.y;

        Vector3 pos = transform.position;
        pos.y = scaleY / 2;

        _yOffset = pos;
    }

    public void SetNumber(int power)
    {
        _number = (int)Mathf.Pow(2, power);

        CloseAllGraphics();

        _selectedGraphic = _graphics[power - 1];
        _selectedGraphic.Open();

        SetCollider();
        SetPositionBySelectedGraphic();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position + _yOffset;
    }

    public void SetLayer(string layerName)
    {
        _layer = layerName;

        gameObject.layer = LayerMask.NameToLayer(layerName);
    }

    public void Destroy()
    {
        DestroyAction?.Invoke(this);
    }
}
