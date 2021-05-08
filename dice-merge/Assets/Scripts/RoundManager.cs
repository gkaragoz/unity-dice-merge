using UnityEngine;

public class RoundManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CubeGenerateManager.instance.GenerateCube();
        }
    }
}
