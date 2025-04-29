using UnityEngine;

public class DistorsionEffect : MonoBehaviour
{
    [SerializeField]
    private float timer;
    public bool active;

    public CustomVideoPlayer glitchEffect;

    // Update is called once per frame
    void Update()
    {
        if(active)
        {
            glitchEffect.PlayVideo();
            timer -= Time.deltaTime;
            if(timer <= 0.0f)
            {
                glitchEffect.videoPlayer.enabled = false;
                Destroy(gameObject);
            }
        }
    }
}
