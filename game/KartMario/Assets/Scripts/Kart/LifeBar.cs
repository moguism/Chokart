using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    public Image fillBarLife;
    public KartController kart;

    private float maxHealth;
    
    void Start()
    {
        //maxHealth = kart.maxHealth;     
        maxHealth = 300f;
    }

    void Update()
    {
        if(kart == null)
        {
            return;
        }

        Debug.Log("LA VIDA DEL COCHE ES: " + kart.health + " y la max " + maxHealth + " y la imagen " + fillBarLife);
        fillBarLife.fillAmount = kart.health / maxHealth;
    }
}
