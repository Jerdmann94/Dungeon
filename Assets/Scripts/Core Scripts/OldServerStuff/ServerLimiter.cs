using UnityEngine;

public class ServerLimiter : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log(Application.targetFrameRate);
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Debug.Log(Application.targetFrameRate);
    }
}