using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;
using System.Collections.Generic;

// ESTE SCRIPT TIENE QUE ESTAR PUESTO EN TODAS LAS ESCENAS PARA QUE FUNCIONE

public class LocalizationManager : MonoBehaviour
{
    private const string PLAYER_PREFS_KEY = "LanguageCode";

    public static string languageCode;

    [SerializeField]
    private TMP_Dropdown languageDropdown;

    [SerializeField]
    private List<StringTable> tables = new();

    private List<TranslationText> textsToTranslate = new();

    IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;

        LocalizationSettings.SelectedLocaleChanged += LocalizationSettings_SelectedLocaleChanged;
        languageCode = PlayerPrefs.GetString(PLAYER_PREFS_KEY);

        ChangeLanguage(true);
    }

    public void ChangeLanguage(bool isStart)
    {
        if (string.IsNullOrEmpty(languageCode))
        {
            languageCode = System.Globalization.CultureInfo.CurrentCulture.Name;
            if(languageCode.Contains("es"))
            {
                languageCode = "es-ES";
            }
            else
            {
                languageCode = "en-US";
            }
        }

        var locale = LocalizationSettings.AvailableLocales.Locales.Find(locale => locale.Identifier.Code == languageCode);

        if(locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
            LocalizationSettings.Instance.SetSelectedLocale(locale);

            if (isStart && languageDropdown != null)
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

            Translate();
        }
    }

    private void LocalizationSettings_SelectedLocaleChanged(UnityEngine.Localization.Locale obj)
    {
        Debug.Log(LocalizationSettings.SelectedLocale);
    }

    private void Translate()
    {
        if(textsToTranslate.Count == 0)
        {
            textsToTranslate = FindObjectsByType<TranslationText>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
        }

        var table = tables.FirstOrDefault(t => t.name.Equals("Locales_" + languageCode));

        for (int i = 0; i < textsToTranslate.Count; i++)
        {
            var textToTranslate = textsToTranslate.ElementAt(i);

            try
            {
                if (textToTranslate.dropdown != null)
                {
                    for (int j = 0; j < textToTranslate.dropdown.options.Count; j++)
                    {
                        GetTextToTranslate(textToTranslate.dropdown.options[j], textToTranslate.dropdownCodes[j], table);
                    }

                    int value = textToTranslate.dropdown.value;
                    GetTextToTranslate(textToTranslate.dropdownSelectedText, textToTranslate.dropdownCodes[value], table);
                }
                else
                {
                    if (textToTranslate.textRaw == null)
                    {
                        GetTextToTranslate(textToTranslate.textElement, textToTranslate.code, table);
                    }
                    else
                    {
                        GetTextToTranslate(textToTranslate.textRaw, textToTranslate.code, table);
                    }
                }
            } catch
            {
                Debug.LogWarning("Error en la tradcucción de: " + textToTranslate.code);
            }
        }
    }

    private void GetTextToTranslate(TMP_Text textElement, string code, StringTable table)
    {
        var entry = table.GetEntry(code).LocalizedValue;
        textElement.text = entry;
    }

    private void GetTextToTranslate(Text textElement, string code, StringTable table)
    {
        var entry = table.GetEntry(code).LocalizedValue;
        textElement.text = entry;
    }

    private void GetTextToTranslate(TMP_Dropdown.OptionData textElement, string code, StringTable table)
    {
        var entry = table.GetEntry(code).LocalizedValue;
        textElement.text = entry;
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