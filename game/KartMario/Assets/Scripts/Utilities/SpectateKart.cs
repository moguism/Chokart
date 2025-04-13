using Cinemachine;
using UnityEngine;

public class SpectateKart : MonoBehaviour
{
    private KartController[] _karts;

    public CinemachineVirtualCamera kartCamera;

    private int index = 0;

    public void Next()
    {
        _karts = FindObjectsByType<KartController>(FindObjectsSortMode.None);
        index++;

        if (index >= _karts.Length)
        {
            index = 0;
        }

        AssignCamera();
    }

    public void Previous()
    {
        _karts = FindObjectsByType<KartController>(FindObjectsSortMode.None);
        index--;

        if (index < 0)
        {
            index = _karts.Length - 1;
        }

        AssignCamera();
    }

    private void AssignCamera()
    {
        GameObject gameObject = _karts[index].gameObject;
        kartCamera.LookAt = gameObject.transform;
        kartCamera.Follow = gameObject.transform;
    }
}
