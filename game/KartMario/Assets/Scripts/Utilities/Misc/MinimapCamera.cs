using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public GameObject player;

    private void LateUpdate()
    {
        if(player == null)
        {
            return;
        }

        transform.position = new Vector3(player.transform.position.x, 40, player.transform.position.z);
    }
}
