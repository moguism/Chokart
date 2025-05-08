using TMPro;
using UnityEngine;

public class VictoryScreenItem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text playerNameText;

    [SerializeField]
    private TMP_Text positionText;

    [SerializeField]
    private TMP_Text killsText;

    public FinishKart finishKart;

    public void SetItem()
    {
        playerNameText.text = finishKart.playerName;
        positionText.text = finishKart.position.ToString();
        killsText.text = finishKart.kills.ToString();
    }
}
