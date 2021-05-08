using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CubeEntity))]
public class CubeMerge : MonoBehaviour
{
    private CubeEntity _entity;

    public event UnityAction MergingAction;

    private void Awake()
    {
        _entity = GetComponent<CubeEntity>();
    }

    private void OnCollisionEnter(Collision other)
    {
        var otherEntity = other.gameObject.GetComponent<CubeEntity>();
        if (otherEntity == null)
            return;

        if (_entity.Owner == otherEntity.Owner && _entity.Number == otherEntity.Number)
            MergingAction?.Invoke();
    }

    public void MergeTo(CubeEntity targetEntity)
    {
        _entity.DisableCollider();
        targetEntity.DisableCollider();

        _entity.DisableRigidbody();
        targetEntity.DisableRigidbody();

        Vector3 midPoint = (_entity.GetPosition() + targetEntity.GetPosition()) * 0.5f;
        _entity.transform.DOMove(midPoint, 0.5f).SetEase(Ease.InOutBack);
        _entity.transform.DOScale(0f, 0.5f).SetEase(Ease.InOutBack);
        targetEntity.transform.DOMove(midPoint, 0.5f).SetEase(Ease.InOutBack);
        targetEntity.transform.DOScale(0f, 0.5f).SetEase(Ease.InOutBack)
            .OnComplete(() =>
            {
                _entity.IncreaseNumber();
                _entity.SetGraphic();
                _entity.transform.DOScale(1f, 0.5f)
                    .SetEase(Ease.InOutBack)
                    .OnComplete(() =>
                    {
                        _entity.EnableCollider();
                        _entity.EnableRigidbody();
                    });
            });
    }
}
