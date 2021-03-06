using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField]
    private BoxCollider _reversedCollider;

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
    private bool _hasSelected;
    private bool _hasEnteredBefore;
    private bool _isLeftOne;

    public string Layer { get => _layer; }
    public Owner Owner { get => _owner; }
    public Owner PlacedAreaOwner { get => _placedAreaOwner; }
    public CubeMerge CubeMerge { get => _cubeMerge; }
    public bool HasSelected { get => _hasSelected; }
    public bool HasEnteredBefore { get => _hasEnteredBefore; }
    public bool IsLeftOne { get => _isLeftOne; }
    public CubeShootManager ShootManager { get => _cubeShootManager; }

    public event UnityAction<CubeEntity> ShootAction;
    public event UnityAction<CubeEntity> StartMergingAction;
    public event UnityAction<CubeEntity, CubeEntity> MergingActionFinished;
    public event UnityAction<CubeEntity> EnterAreaAction;
    public event UnityAction<CubeEntity> EnterWrongAreaAction;
    public event UnityAction<CubeEntity> DestroyAction;
    public event UnityAction<CubeEntity, CubeEntity> CollideWithEnemyCubeAction;

    private void Awake()
    {
        _cubeShootManager = GetComponent<CubeShootManager>();
        _cubeMerge = GetComponent<CubeMerge>();

        _cubeShootManager.ShootAction += OnShooted;
        _cubeMerge.StartMergingAction += OnStartMergingAction;
        _cubeMerge.MergingActionFinished += OnMergingActionFinished;

        _status = StatusType.Idle;

        DisableReversedCollider();
    }

    private void Start()
    {
        PlayerManager.instance.CubeSelection += OnCubeSelection;
    }

    private void OnCubeSelection(CubeEntity selectedCube)
    {
        _hasSelected = this == selectedCube;
    }

    private void OnMergingActionFinished(CubeEntity c1, CubeEntity c2)
    {
        if (c1 == this || c2 == this)
            _status = StatusType.Idle;

        MergingActionFinished?.Invoke(c1, c2);
    }

    private void OnDestroy()
    {
        _cubeShootManager.ShootAction -= OnShooted;
        _cubeMerge.StartMergingAction -= OnStartMergingAction;
        _cubeMerge.MergingActionFinished -= OnMergingActionFinished;

        PlayerManager.instance.CubeSelection -= OnCubeSelection;
    }

    private void OnShooted()
    {
        _status = StatusType.Flying;

        ShootAction?.Invoke(this);

        PlayerManager.instance.CubeSelection -= OnCubeSelection;
    }

    private void OnStartMergingAction()
    {
        _status = StatusType.Merging;

        StartMergingAction?.Invoke(this);
    }

    private void CloseAllGraphics()
    {
        foreach (var item in _graphics)
            item.Close();
    }

    private void SetCollider()
    {
        _collider.size = _selectedGraphic.Scale;
        _reversedCollider.size = _selectedGraphic.Scale;
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

    public void EnableReversedCollider()
    {
        _reversedCollider.enabled = true;
    }

    public void EnableRigidbody()
    {
        _rb.isKinematic = false;
    }

    public void DisableCollider()
    {
        _collider.enabled = false;
    }
    public void DisableReversedCollider()
    {
        _reversedCollider.enabled = false;
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

        _isLeftOne = transform.position.x < 0 ? true : false;
    }

    public void SetLayer(string layerName)
    {
        _layer = layerName;

        gameObject.layer = LayerMask.NameToLayer(layerName);

        if (_layer == Strings.PLAYER_CUBE_LAYER)
            _owner = Owner.Player;
        else if (_layer == Strings.ENEMY_CUBE_LAYER)
            _owner = Owner.Enemy;

        SetName();
    }

    public void Destroy()
    {
        _status = StatusType.Destroying;

        DestroyAction?.Invoke(this);

        GameObject.Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        var otherEntity = other.gameObject.GetComponent<CubeEntity>();
        if (otherEntity == null)
            return;

        if (Owner != otherEntity.Owner && otherEntity.Status == StatusType.Flying)
        {
            CollideWithEnemyCubeAction?.Invoke(this, otherEntity);
            Destroy();
        }
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

            if (_hasEnteredBefore)
                return;

            _placedAreaOwner = playArea.Owner;
            _status = StatusType.InArea;

            _hasEnteredBefore = true;

            DOVirtual.DelayedCall(0.2f, () =>
            {
                EnableReversedCollider();
            });

            EnterAreaAction?.Invoke(this);
        }
        else if (other.CompareTag(Strings.DESTROYER_AREA))
        {
            PlayArea destroyerArea = other.GetComponent<PlayArea>();
            if (destroyerArea.Owner == Owner.Player)
            {
                if (Layer == Strings.PLAYER_CUBE_LAYER)
                {
                    if (_hasEnteredBefore)
                    {
                        EnterWrongAreaAction?.Invoke(this);
                        Destroy();
                    }
                    return;
                }
            }
            else if (destroyerArea.Owner == Owner.Enemy)
            {
                if (Layer == Strings.ENEMY_CUBE_LAYER)
                {
                    if (_hasEnteredBefore)
                    {
                        EnterWrongAreaAction?.Invoke(this);
                        Destroy();
                    }
                    return;
                }
            }
        }
    }
    
                    
}
