using System;
using UnityEngine;
using UnityEngine.Events;
using static RoundManager;

[RequireComponent(typeof(CubeShootManager), typeof(CubeMerge))]
public class CubeEntity : MonoBehaviour
{
    public enum StatusType
    {
        Idle,
        Placed,
        Flying,
        InArea,
        Merging,
        Destroying
    }

    [SerializeField]
    private CubeEntityGraphic[] _graphics;
    [SerializeField]
    private Rigidbody _rb;
    [SerializeField]
    private BoxCollider _collider;

    [Header("Debug")]
    [SerializeField]
    private StatusType _status;
    [SerializeField]
    private int _number;
    
    public int Number { get => _number; }
    public StatusType Status { get => _status; }

    private CubeEntityGraphic _selectedGraphic;

    private Vector3 _yOffset;
    private string _layer;
    private CubeShootManager _cubeShootManager;
    private CubeMerge _cubeMerge;
    private Owner _owner;
    private Owner _placedAreaOwner;
    private int _power;

    public string Layer { get => _layer; }
    public Owner Owner { get => _owner; }
    public Owner PlacedAreaOwner { get => _placedAreaOwner; }
    public CubeMerge CubeMerge { get => _cubeMerge; }

    public event UnityAction<CubeEntity> ShootAction;
    public event UnityAction<CubeEntity> MergingAction;
    public event UnityAction<CubeEntity> EnterAreaAction;
    public event UnityAction<CubeEntity> DestroyAction;

    private void Awake()
    {
        _cubeShootManager = GetComponent<CubeShootManager>();
        _cubeMerge = GetComponent<CubeMerge>();

        _cubeShootManager.ShootAction += OnShooted;
        _cubeMerge.MergingAction += OnMerging;

        _status = StatusType.Idle;
    }

    private void OnDestroy()
    {
        _cubeShootManager.ShootAction -= OnShooted;
        _cubeMerge.MergingAction -= OnMerging;
    }

    private void OnShooted()
    {
        _status = StatusType.Flying;

        ShootAction?.Invoke(this);
    }

    private void OnMerging()
    {
        _status = StatusType.Merging;

        MergingAction?.Invoke(this);
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

    private void SetName()
    {
        gameObject.name = Owner + "_Cube_(" + _number + ")";
    }

    public void SetNumber(int power)
    {
        _power = power;
        _number = (int)Mathf.Pow(2, power);

        SetName();
    }

    public void IncreaseNumber()
    {
        _power++;
        _number = (int)Mathf.Pow(2, _power);

        SetName();
    }

    public void SetGraphic()
    {
        CloseAllGraphics();

        _selectedGraphic = _graphics[_power - 1];
        _selectedGraphic.Open();

        SetPositionBySelectedGraphic();
        SetCollider();
    }

    public void EnableCollider()
    {
        _collider.enabled = true;
    }

    public void EnableRigidbody()
    {
        _rb.isKinematic = false;
    }

    public void DisableCollider()
    {
        _collider.enabled = false;
    }

    public void DisableRigidbody()
    {
        _rb.isKinematic = true;
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
        Debug.LogWarning("Destroying");

        _status = StatusType.Destroying;

        DestroyAction?.Invoke(this);
    }

    private void OnCollisionEnter(Collision other)
    {
        var otherEntity = other.gameObject.GetComponent<CubeEntity>();
        if (otherEntity == null)
            return;

        if (Owner != otherEntity.Owner)
            Destroy();
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

            _placedAreaOwner = playArea.Owner;
            _status = StatusType.InArea;

            EnterAreaAction?.Invoke(this);
        }
    }

}
