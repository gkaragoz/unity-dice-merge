using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CubeGenerateManager.instance.GenerateCube();
        }
    }
}
