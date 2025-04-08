using UnityEngine;

public class HealthPotion : BasicObject
{
    [SerializeField]
    private float healthChange;
    public HealthChanger healthChanger;

    public new void UseObject()
    {
        print("Usando curaci�n");

        if (IsOwner)
        {
            // El "healthChange" est� en negativo para que sume y no reste
            healthChanger.NotifyServerAboutChangeServerRpc(owner, -healthChange);
            DespawnOnTimeServerRpc();
        }
    }
}
