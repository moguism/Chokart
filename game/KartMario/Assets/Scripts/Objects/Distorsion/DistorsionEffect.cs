using UnityEngine;

public class DistorsionEffect : MonoBehaviour
{
    [SerializeField]
    private float timer;
    public bool active;

    // Update is called once per frame
    void Update()
    {
        if(active)
        {
            //timer -= Time.deltaTime;
            if(timer <= 0.0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
