using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class FinishLine : MapTrigger
{
    [SerializeField]
    private int totalLapsToWin = 3;

    // el numero de vueltas
    public GameObject lapPanel;              
    public TextMeshProUGUI lapText;

    public List<MapTrigger> triggers = new List<MapTrigger>();

    private void Start()
    {
        // activar u ocultar según el modo de juego
        if (LobbyManager.gamemode == Gamemodes.Race)
        {
            lapPanel.SetActive(true);
        }
        else
        {
            lapPanel.SetActive(false);
        }
    }

    public new void OnTriggerEnter(Collider other)
    {
        if (!LobbyManager.isHost)
        {
            return;
        }

        base.OnTriggerEnter(other);

        var parent = other.gameObject.transform.parent;

        if (parent == null || !parent.CompareTag("Kart")) return;

        var kart = parent.GetComponentInChildren<KartController>();

        if (kart == null) return;

        //ChangeIndexAndCalculatePosition(kart);

        var list = kart.triggers.Where(t => t != -1).Distinct().ToList();
        // Solo cuenta una vuelta si ha activado todos los triggers (en orden, que es lo que hace "SequenceEqual") antes de cruzar la meta
        if (list.Count == triggers.Count || !kart.passedThroughFinishLine)
        {
            NotifyLapCompletedServerRpc(kart.NetworkObjectId);
            ChangeIndexAndCalculatePosition(kart);
        }
        else
        {
            kart.triggers = new List<int>() { 0 };
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void NotifyLapCompletedServerRpc(ulong kartId, ServerRpcParams rpcParams = default)
    {
        var kart = positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId);
        if (kart != null)
        {
            kart.totalLaps++;
            bool disable = false;

            if (LobbyManager.gamemode == Gamemodes.Race && LobbyManager.gameStarted)
            {

                if (kart.totalLaps == totalLapsToWin)
                {
                    disable = true;

                    Debug.LogWarning("Ha hecho las vueltas y su posición final ha sido: " + kart.position);
                    DetectCollision.CreateNewFinishKart(positionManager, kart, kart.position);

                    if (positionManager.karts.Count == positionManager.finishKarts.Count)
                    {
                        positionManager.SetVictoryScreen();
                    }
                }
            }

            NotifyClientsAboutLapClientRpc(kartId, disable);
        }
    }

    [ClientRpc]
    private void NotifyClientsAboutLapClientRpc(ulong kartId, bool disable)
    {
        KartController kart = positionManager.karts.FirstOrDefault(k => k.NetworkObjectId == kartId);
        if (kart != null)
        {
            kart.passedThroughFinishLine = true;
            kart.triggers = new List<int>() { 0 };
            kart.totalLaps++;
            Debug.LogWarning("El coche " + kartId + " ha dado " + kart.totalLaps + " vueltas");

            if (lapPanel.activeSelf && LobbyManager.gameStarted)
            {
                string text = $"{kart.totalLaps - 1}/{totalLapsToWin - 1}";
                if (LobbyManager.isHost && kart.isHost)
                {
                    if(kart.isHost)
                    {
                        lapText.text = text;
                    }
                }
                else
                {
                    lapText.text = text;
                }
            }

            if (disable)
            {
                DetectCollision.DisableKart(positionManager, kart, false, true);
            }
        }
    }
}
