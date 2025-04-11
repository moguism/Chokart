using UnityEngine;
using UnityEngine.Video;

public class DistorsionEffect : MonoBehaviour
{
    [SerializeField]
    private float timer;
    public bool active;

    public VideoPlayer glitchEffect;

    // Update is called once per frame
    void Update()
    {
        if(active)
        {
            glitchEffect.enabled = true;
            timer -= Time.deltaTime;
            if(timer <= 0.0f)
            {
                glitchEffect.enabled = false;
                Destroy(gameObject);
            }
        }
    }
}
