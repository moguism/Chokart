using Unity.Netcode;

public class BasicPlayer : NetworkBehaviour
{
    public float health = 0;
    public static readonly float damageMultiplier = 1; // Al ser estático no se muestra en el editor :(

    // TODO: Creo que aquí se puede poner la lógica de replicación
}
