using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{

    public Image fillBarLife;
    private KartController kart;

    private float maxHealth;
    
    void Start()
    {
        kart = GameObject.Find("Kart").GetComponent<KartController>();

        // maxHealth = kart.maxHealth;     
        maxHealth = 150f;
    }

    void Update()
    {
        Debug.Log("LA VIDA DEL COCHE ES: " + kart.health + " y la max " + maxHealth + " y la imagen " + fillBarLife);
        fillBarLife.fillAmount = kart.health / maxHealth;
    }
}
