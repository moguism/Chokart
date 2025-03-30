using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarSelection : MonoBehaviour
{
    public static KartModel[] cars;

    [SerializeField]
    private KartModel[] _cars;

    [SerializeField]
    private TMP_Text speedText;

    [SerializeField]
    private GameObject kartBase;

    private int index;

    [SerializeField]
    private bool testing = false;

    private void Start()
    {
        cars = _cars.ToArray(); // Para que haga una copia
        index = PlayerPrefs.GetInt("carIndex");
        ManageCarVisibility();
    }

    private void FixedUpdate()
    {
        kartBase.transform.Rotate(0, 1f, 0);
    }

    public void Next()
    {
        index++;
        if(index >= _cars.Length)
        {
            index = 0;
        }
        ManageCarVisibilityAndSave();
    }

    public void Prev()
    {
        index--;
        if(index < 0)
        {
            index = _cars.Length - 1;
        }
        ManageCarVisibilityAndSave();
    }

    public void GoToGame()
    {
        WebsocketSingleton.kartModelIndex = index;

        // TODO: BORRAR DESPUÉS Y PONER ÚNICAMENTE LAS LOBBIES (1)
        int sceneNumber = testing ? 2 : 1;
        SceneManager.LoadScene(sceneNumber);
    }

    private void ManageCarVisibilityAndSave()
    {
        ManageCarVisibility();
        SaveIndex();
    }

    private void ManageCarVisibility()
    {
        for(int i = 0; i < _cars.Length; i++)
        {
            _cars[i].car.SetActive(false);
        }

        _cars[index].car.SetActive(true);
        speedText.text = "Speed: " + _cars[index].speed;
    }

    private void SaveIndex()
    {
        PlayerPrefs.SetInt("carIndex", index);
        PlayerPrefs.Save();
    }
}

[System.Serializable]
public class KartModel
{
    public GameObject car; // Por ahora esto es una referencia al modelo (digo por ahora porque probablemente se puede hacer mejor)
    public float speed;
}