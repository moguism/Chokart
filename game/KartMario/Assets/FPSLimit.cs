using UnityEngine;

public class FPSLimit : MonoBehaviour
{
    public int frameRate = 60;

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frameRate;
    }
}
