using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class TranslationText : MonoBehaviour
{
    public TMP_Text textElement;
    public Text textRaw;
    public string code;

    // PARA LOS DROPDOWNS
    [Header("DROPDOWN")]
    public TMP_Dropdown dropdown;
    public List<string> dropdownCodes = new List<string>(); // Esta lista de opciones tiene que estar en el mismo orden que en el dropdown 
    public TMP_Text dropdownSelectedText;
}
