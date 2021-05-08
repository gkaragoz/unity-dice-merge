using System;
using UnityEngine;
using UnityEngine.Events;
using static PlayArea;
using static RoundManager;

[RequireComponent(typeof(CubeShootManager))]
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

    private CubeEntityGraphic _selectedGraphic;
    private Vector3 _yOffset;
    private string _layer;
    private CubeShootManager _cubeShootManager;
    private Owner _owner;
    private Owner _placedAreaOwner;

    public event UnityAction<CubeEntity> DestroyAction;
    public string Layer { get => _layer; }
    public bool HasPlacedInArea { get; private set; }
    public Owner Owner { get => _owner; }
    public Owner PlacedAreaOwner { get => _placedAreaOwner; }

    public event UnityAction<CubeEntity> ShootAction;
    public event UnityAction<CubeEntity> PlacedInAreaAction;

    private void Awake()
    {
        _cubeShootManager = GetComponent<CubeShootManager>();

        _cubeShootManager.ShootAction += OnShooted;
    }

    private void OnDestroy()
    {
        _cubeShootManager.ShootAction -= OnShooted;
    }

    private void OnShooted()
    {
        ShootAction?.Invoke(this);
    }

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

        if (_layer == Strings.PLAYER_CUBE_LAYER)
            _owner = Owner.Player;
        else if (_layer == Strings.ENEMY_CUBE_LAYER)
            _owner = Owner.Enemy;
    }

    public void Destroy()
    {
        DestroyAction?.Invoke(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Strings.PLAY_AREA))
        {
            PlayArea playArea = other.GetComponent<PlayArea>();
            if (playArea.Owner == Owner.Player)
            {
                if (Layer == Strings.PLAYER_CUBE_LAYER)
                    return;
            }
            else if (playArea.Owner == Owner.Enemy)
            {
                if (Layer == Strings.ENEMY_CUBE_LAYER)
                    return;
            }

            HasPlacedInArea = true;
            _placedAreaOwner = playArea.Owner;

            PlacedInAreaAction?.Invoke(this);
        }
    }

}
