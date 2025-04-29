using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

// ESTE SCRIPT TIENE QUE ESTAR PUESTO EN TODAS LAS ESCENAS PARA QUE FUNCIONE

public class LocalizationManager : MonoBehaviour
{
    private const string PLAYER_PREFS_KEY = "LanguageCode";

    private string languageCode;

    [SerializeField]
    private TMP_Dropdown languageDropdown;

    [SerializeField]
    private System.Collections.Generic.List<TranslationText> textsToTranslate = new();

    [SerializeField]
    private System.Collections.Generic.List<StringTable> tables = new();

    IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;

        LocalizationSettings.SelectedLocaleChanged += LocalizationSettings_SelectedLocaleChanged;
        languageCode = PlayerPrefs.GetString(PLAYER_PREFS_KEY);

        ChangeLanguage(true);
    }

    public void ChangeLanguage(bool isStart)
    {
        if(languageCode == null)
        {
            return;
        }

        var locale = LocalizationSettings.AvailableLocales.Locales.Find(locale => locale.Identifier.Code == languageCode);

        if(locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
            LocalizationSettings.Instance.SetSelectedLocale(locale);

            if (isStart)
            {
                switch (languageCode)
                {
                    case "es-ES":
                        languageDropdown.value = 0;
                        break;
                    case "en-US":
                        languageDropdown.value = 1;
                        break;
                }
            }
            else
            {
                PlayerPrefs.SetString(PLAYER_PREFS_KEY, languageCode);
                PlayerPrefs.Save();
            }

#if UNITY_WEBGL
            Translate();
#endif
        }
    }

    private void LocalizationSettings_SelectedLocaleChanged(UnityEngine.Localization.Locale obj)
    {
        Debug.Log(LocalizationSettings.SelectedLocale);
    }

    private void Translate()
    {
        for(int i = 0; i < textsToTranslate.Count; i++)
        {
            var textToTranslate = textsToTranslate.ElementAt(i);

            var table = tables.FirstOrDefault(t => t.name.Equals("Locales_" + languageCode));

            var entry = table.GetEntry(textToTranslate.code).LocalizedValue;

            textToTranslate.textElement.text = entry;
        }
    }

    public void OnLanguageChanged()
    {
        int optionId = languageDropdown.value;
        switch(optionId)
        {
            case 0:
                languageCode = "es-ES";
                break;
            case 1:
                languageCode = "en-US";
                break;
        }

        ChangeLanguage(false);
    }
}

[System.Serializable]
public class TranslationText
{
    public TMP_Text textElement;
    public string code;
}
