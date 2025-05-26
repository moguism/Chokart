using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PositionManager : NetworkBehaviour
{
    public List<KartController> karts;
    public FinishLine finishLine;

    public List<FinishKart> finishKarts = new();

    [SerializeField]
    private StartCounter startCounter;

    public GameObject victoryScreen;

    public SpectateKart spectateKart;
    public GameObject spectateCanvas;
    public GameObject loadingScreen;

    private readonly BattleService battleService = new BattleService();

    void LateUpdate()
    {
        if (!IsHost)
        {
            //print("No soy host");
            return;
        }

        foreach (KartController kart in karts)
        {
            finishLine.CalculateDistanceToNextTrigger(kart);
        }

        // Primero los ordena por las vueltas, luego por el �ltimo trigger que hayan visitado y finalmente por la distancia al siguiente trigger
        karts = karts
            .Where(k => k != null)
            .Distinct()
            .OrderByDescending(k => k.totalLaps)
            .ThenByDescending(k => k.lastTriggerIndex)
            .ThenBy(k => k.distanceToNextTrigger)
            .ToList();

        for (int i = 0; i < karts.Count(); i++)
        {
            //print("i vale " + i);
            KartController kart = karts.ElementAt(i);

            //print("EL COCHE " + kart.NetworkObjectId + " EST� EN " + kart.currentPosition);

            int newPosition = i + 1;
            kart.position = newPosition;

            NetworkObject networkObject = kart.gameObject.transform.parent.GetComponent<NetworkObject>();
            ulong ownerClient = networkObject.OwnerClientId;

            // Si es el 0 es el servidor
            if (ownerClient == 0)
            {
                AssignNewPosition(newPosition, kart);
            }
            else
            {
                var parameters = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { kart.OwnerClientId }
                    }
                };

                NotifyPositionChangedClientRpc(newPosition, parameters);
            }
        }

    }

    [ClientRpc]
    private void NotifyPositionChangedClientRpc(int newPosition, ClientRpcParams clientRpcParams = default)
    {
        print("POSICI�N RECIBIDA");

        var kart = karts.FirstOrDefault();
        if (kart != null)
        {
            print("La nueva posici�n es: " + newPosition);
            AssignNewPosition(newPosition, kart);

        }
    }

    private void AssignNewPosition(int newPosition, KartController kart)
    {
        kart.position = newPosition;

        if (kart.positionText == null)
        {
            kart.positionText = GameObject.Find("PositionValue").GetComponent<TMP_Text>();
        }

        if (!kart.enableAI)
        {
            kart.positionText.text = GetOrdinal(newPosition);
           
        }
    }

    private string GetOrdinal(int number)
    {
        if (LocalizationManager.languageCode == "en-US")
        {

            switch (number)
            {
                case 1: return number + "st";
                case 2: return number + "nd";
                case 3: return number + "rd";
                default: return number + "th";
            }
        }
        else
        {
            return number + "�";
        }
    }




    public void ChangeValuesOfKart(Vector3 newPosition, ulong kartId, int lastTriggerIndex, int position, int totalLaps, int[] triggers, bool reset = false)
    {
        InformClientsAboutChangeClientRpc(newPosition, kartId, lastTriggerIndex, position, totalLaps, triggers, reset);
    }


    [ClientRpc]
    private void InformClientsAboutChangeClientRpc(Vector3 newPosition, ulong kartId, int lastTriggerIndex, int position, int totalLaps, int[] triggers, bool reset)
    {
        var kart = karts.FirstOrDefault(k => k.NetworkObjectId == kartId);
        if (kart != null)
        {
            LobbyManager.gameStarted = true;

            kart.sphere.position = newPosition;
            kart.sphere.transform.position = newPosition;
            kart.transform.position = newPosition;
            kart.position = position;
            kart.lastTriggerIndex = lastTriggerIndex;
            kart.totalLaps = totalLaps;
            kart.triggers = triggers.ToList();

            try
            {
                if (reset)
                {
                    kart.sphere.rotation = Quaternion.identity; // Resetea la rotaci�n
                    kart.sphere.transform.rotation = Quaternion.identity;
                    kart.transform.rotation = Quaternion.identity;

                    kart.currentObject = "";
                    kart.health = kart.maxHealth;
                    kart.passedThroughFinishLine = false;
                    kart.canMove = false;
                    kart.transform.parent.GetComponentInChildren<CarDamage>().Repair();
                }
            }
            catch { }
        }
    }
    public void InformAboutGameStart()
    {
        InformAboutGameStartClientRpc();
    }

    [ClientRpc]
    private void InformAboutGameStartClientRpc()
    {
        startCounter.StartBegginingCounter(karts.ToArray());
    }

    public void CheckVictory(ulong kartId)
    {
        if (karts.Count - 1 <= 1)
        {
            DetectCollision.CreateNewFinishKart(this, karts.FirstOrDefault(k => k != null && k.NetworkObjectId != kartId), karts.Count - 1);
            SetVictoryScreen();
        }
    }

    public void SetVictoryScreen()
    {
        CreateBattleCoroutine(); // Manda la petici�n a la base de datos

        string json = JsonConvert.SerializeObject(finishKarts);
        Debug.LogWarning("JSON: " + json);

        NotifyAboutGameEndClientRpc(json);
    }

    [ClientRpc(RequireOwnership = false)]
    private void NotifyAboutGameEndClientRpc(string json)
    {
        LobbyManager.gameStarted = false;

        Debug.LogWarning("JSON1: " + json);
        victoryScreen.SetActive(true);

        List<FinishKart> finishKarts = JsonConvert.DeserializeObject<List<FinishKart>>(json);

        Debug.LogWarning("Deserializado: " + finishKarts.Count);

        VictoryScreen victory = victoryScreen.GetComponentInChildren<VictoryScreen>();
        victory.finishKarts = finishKarts.OrderBy(k => k.position).ToList();
        victory.SetFinishKarts();
    }

    public void CreateBattleCoroutine()
    {
        StartCoroutine(battleService.CreateBattleCorroutine(finishKarts));
    }

    public async Task CreateBattleAsync()
    {
        await battleService.CreateBattleAsync(finishKarts);
    }

    public async Task ExitGame()
    {
        if (LobbyManager.gameStarted && LobbyManager.isHost)
        {
            foreach (KartController kart in karts)
            {
                DetectCollision.CreateNewFinishKart(this, kart, kart.position);
            }

            await CreateBattleAsync();
        }
    }

    private async void OnApplicationQuit()
    {
        Debug.LogWarning("Cerrando");
        await ExitGame();
    }
}