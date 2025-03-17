using UnityEngine;
using TMPro;

public class DetectCollision : MonoBehaviour
{
    public KartController kart;

    //para mostrar la salud en pantalla
    public TextMeshProUGUI healthText;


    /*private void Start()
    {
        kart = transform.parent.GetComponentInChildren<KartController>();
        print(kart);
    }*/

    private void Start()
    {
        // texto de salud en pantalla TODO: Esto seria mejor hacerlo desde un gameManaher que tenga variables globales
        healthText = GameObject.Find("HealthText").GetComponent<TextMeshProUGUI>();
        if (healthText == null)
        {
            Debug.LogError("No se encontró el objeto HealthText en la escena.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var parent = collision.gameObject.transform.parent;
        if (parent && parent.CompareTag("Enemy"))
        {
            BasicPlayer basicPlayer = parent.GetComponent<BasicPlayer>(); // TODO: Habría que cambiar "BasicPlayer" por "KartController" (y quizás la forma a la que se accede al componente)
            print("ENTER " + parent.name + ". Salud: " + basicPlayer.health);


            float kartSpeed = kart.sphere.linearVelocity.magnitude;
            print("VELOCIDAD PROPIA: " + kartSpeed);

            /* TODO: DESCOMENTAR ESTO Y BORRAR LO QUE VIENE DESPUÉS CUANDO ESTÉ EL ONLINE
        
            float otherKartSpeed = otherKart.sphere.linearVelocity.magnitude;
            print("VELOCIDAD DEL OPONENTE: " + otherKartSpeed);

            if (kartSpeed > otherKartSpeed)
            {
                CheckAndRemoveObject(otherKart, kartSpeed);
            }
            else if (otherKartSpeed > kartSpeed)
            {
                CheckAndRemoveObject(kart, otherKartSpeed);
            }
            else
            {
                // Quito vida a los dos por igual
                CheckAndRemoveObject(kart, otherKartSpeed);
                CheckAndRemoveObject(otherKart, kartSpeed);
            }
            */

            basicPlayer.health -= kartSpeed * BasicPlayer.damageMultiplier;
            print("SALUD RESTANTE: " + basicPlayer.health);

            // TEXTO DE SALUD (QUITAR DE ESTE ARCHIVO)
            if (healthText != null)
            {
                healthText.text = "Salud: " + kart.health.ToString("F1");

                // color salud
                if (kart.health <= 60)
                {
                    healthText.color = Color.yellow;
                }
                else if (kart.health <= 30)
                {
                    healthText.color = Color.red;
                }
                else
                {
                    healthText.color = Color.green;
                }
            }


            if (basicPlayer.health <= 0)
            {
                Destroy(parent.gameObject);
            }
        }
    }

    private static void CheckAndRemoveObject(KartController kartController, float speed)
    {
        kartController.health -= speed * BasicPlayer.damageMultiplier;
        if (kartController.health <= 0)
        {
            Destroy(kartController.gameObject);
        }
    }

    /*private void OnCollisionStay(Collision collision)
    {
        var parent = collision.gameObject.transform.parent;
        if (parent && parent.CompareTag("Enemy"))
        {
            BasicPlayer basicPlayer = parent.GetComponent<BasicPlayer>();
            print("STAY " + parent.name + ". Salud: " + basicPlayer.health);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        var parent = collision.gameObject.transform.parent;
        if (parent && parent.CompareTag("Enemy"))
        {
            BasicPlayer basicPlayer = parent.GetComponent<BasicPlayer>();
            print("EXIT " + parent.name + ". Salud: " + basicPlayer.health);
        }
    }*/
}
