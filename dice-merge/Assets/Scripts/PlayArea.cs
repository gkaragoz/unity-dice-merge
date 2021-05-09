using UnityEngine;

public class PlayArea : MonoBehaviour
{
    [SerializeField]
    private Owner _owner;

    public Owner Owner { get => _owner; }
}
