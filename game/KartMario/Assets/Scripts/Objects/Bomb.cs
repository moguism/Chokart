using UnityEngine;

public class Bomb : GreenShell
{
    [SerializeField]
    private float radiusTimer;

    [SerializeField]
    private float radiusIncrease;

    [SerializeField]
    private SphereCollider bombCollider;

    private bool canMove = true;
    public bool exploded = false;

    new void Update()
    {
        if (direction != null)
        {
            targetTime -= Time.deltaTime;

            // Cuenta el tiempo normal hasta que se acabe
            if (targetTime <= 0.0f)
            {
                canMove = false;
                exploded = true;

                print("Ha explotado");

                // Si se agota el tiempo normal, explota
                radiusTimer -= Time.deltaTime;

                bombCollider.radius += radiusIncrease; // Incrementa el radio de la explosión

                if (radiusTimer <= 0.0f)
                {
                    if (IsOwner)
                    {
                        DespawnOnTimeServerRpc();
                    }
                }

            }
        }
    }

    public new void FixedUpdate()
    {
        if(canMove)
        {
            base.FixedUpdate();
        }
    }

    public new void UseObject()
    {
        print("Usada bomba");
    }
}
