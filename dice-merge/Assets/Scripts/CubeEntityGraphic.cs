using UnityEngine;

public class CubeEntityGraphic : MonoBehaviour
{
    public Vector3 Scale { get => transform.localScale; }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

}
