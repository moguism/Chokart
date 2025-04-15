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
                EnableOrDisableKarts(true, true);
                Destroy(gameObject);
            }
        }
    }

    public void StartBegginingCounter(KartController[] karts)
    {
        _karts = karts;
        EnableOrDisableKarts(false, false);
        gameObject.SetActive(true);
        start = true;
    }

    private void EnableOrDisableKarts(bool canMove, bool canBeHurt)
    {
        for(int i = 0; i < _karts.Length; i++)
        {
            _karts[i].canMove = canMove;
            _karts[i].canBeHurt = canBeHurt;
            if (!_karts[i].canBeHurt)
            {
                _karts[i].health = _karts[i].maxHealth;
            }
        }
    }
}
