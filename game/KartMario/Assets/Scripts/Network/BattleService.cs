using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class BattleService {
    public IEnumerator CreateBattleCorroutine(List<FinishKart> finishKarts)
    {
        UnityWebRequest unityWebRequest = CreateWebRequest(finishKarts);

        yield return unityWebRequest.SendWebRequest();

        ManageResult(unityWebRequest);
    }

    public async Task CreateBattleAsync(List<FinishKart> finishKarts)
    {
        UnityWebRequest unityWebRequest = CreateWebRequest(finishKarts);
        await unityWebRequest.SendWebRequest();
        ManageResult(unityWebRequest);
    }

    private UnityWebRequest CreateWebRequest(List<FinishKart> finishKarts)
    {
        UnityWebRequest unityWebRequest = new UnityWebRequest(ENVIRONMENT.API_URL + "Battle", "POST");
        string jsonData = JsonConvert.SerializeObject(finishKarts);
        Debug.Log("Datos: " + jsonData);

        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        unityWebRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
        unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
        unityWebRequest.SetRequestHeader("Content-Type", "application/json");
        unityWebRequest.SetRequestHeader("Authorization", "Bearer " + AuthManager.token);

        return unityWebRequest;
    }

    private void ManageResult(UnityWebRequest unityWebRequest)
    {
        if (unityWebRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Se ha creado la batalla :D");
        }
        else
        {
            Debug.LogError("No se ha podido crear la batalla: " + unityWebRequest.error);
        }
    }
}
