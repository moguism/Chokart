using TMPro;
using UnityEngine;

public class StartCounter : MonoBehaviour
{
    [SerializeField]
    private TMP_Text startText;

    [SerializeField]
    private float timer = 3.0f;

    private KartController[] _karts;

    private bool start = false;

    void Update()
    {
        if(start)
        {
            timer -= Time.deltaTime;
            if(timer <= 3.0f && timer >= 2.0f)
            {
                startText.text = "3";
            }
            else if(timer < 2.0f && timer >= 1.0f)
            {
                startText.text = "2";
            }
            else if(timer < 1.0f && timer >= 0.0f)
            {
                startText.text = "1";
            }
            else
            {
                EnableOrDisableKarts(true);
                Destroy(gameObject);
            }
        }
    }

    public void StartBegginingCounter(KartController[] karts)
    {
        _karts = karts;
        EnableOrDisableKarts(false);
        gameObject.SetActive(true);
        start = true;
    }

    private void EnableOrDisableKarts(bool canMove)
    {
        for(int i = 0; i < _karts.Length; i++)
        {
            _karts[i].canMove = canMove;
        }
    }
}
