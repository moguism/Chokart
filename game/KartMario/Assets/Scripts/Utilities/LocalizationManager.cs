using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizationManager : MonoBehaviour
{
    private string languageCode;

    public void ChangeLanguage()
    {
        var locale = LocalizationSettings.AvailableLocales.Locales.Find(locale => locale.Identifier.Code == languageCode);

        if(locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
        }
    }

    public void OnLanguageChanged(int optionId)
    {
        switch(optionId)
        {
            case 0:
                languageCode = "es-ES";
                break;
            case 1:
                languageCode = "en-US";
                break;
        }

        ChangeLanguage();
    }
}
