using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Invisibility : BasicObject
{
    public KartController parent;

    [SerializeField]
    private float timer;

    void Update()
    {
        if (parent != null)
        {
            timer -= Time.deltaTime;

            DisableOnEnableRenders(parent, false);

            if (timer <= 0.0f)
            {
                DisableOnEnableRenders(parent, true);

                InformClientAboutChangeClientRpc(parent.NetworkObjectId, true);

                if (IsOwner)
                {
                    DespawnOnTimeServerRpc();
                }
            }

            InformClientAboutChangeClientRpc(parent.NetworkObjectId, parent.renders[0].enabled);
        }
    }

    public static void DisableOnEnableRenders(KartController kart, bool enabled)
    {
        foreach(Renderer render in kart.renders)
        {
            render.enabled = enabled;
        }
        kart.characters.SetActive(enabled);
    }

    [ClientRpc]
    private void InformClientAboutChangeClientRpc(ulong kartId, bool shouldBeVisibleToOthers)
    {
        KartController kart = FindObjectsByType<KartController>(FindObjectsSortMode.None).FirstOrDefault(k => k.NetworkObjectId == kartId);
        if (kart != null)
        {
            bool isLocalPlayer = kart.IsOwner;
            bool shouldRender = isLocalPlayer || shouldBeVisibleToOthers;
            DisableOnEnableRenders(kart, shouldRender);
        }
    }

    public new void UseObject()
    {
        print("Usada invisibilidad");
    }
}
