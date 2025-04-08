using UnityEngine;

public class HealthPotion : BasicObject
{
    [SerializeField]
    private float healthChange;
    public HealthChanger healthChanger;

    public new void UseObject()
    {
        print("Usando curación");

        if (IsOwner)
        {
            // El "healthChange" está en negativo para que sume y no reste
            healthChanger.NotifyServerAboutChangeServerRpc(owner, -healthChange);
            DespawnOnTimeServerRpc();
        }
    }
}
