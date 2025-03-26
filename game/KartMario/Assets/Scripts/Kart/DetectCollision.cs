using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    public KartController kart;

    /*private void Start()
    {
        kart = transform.parent.GetComponentInChildren<KartController>();
        print(kart);
    }*/

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

            if (basicPlayer.health <= 0)
            {
                Destroy(parent.gameObject);
            }
        }
        else if(collision.gameObject.CompareTag("Ground"))
        {
            kart.isGrounded = true;
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

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            kart.isGrounded = false;
        }
    }
}
