using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LanguageSelector : MonoBehaviour
{
    public Dropdown dropdown;
    private bool active = false;

    private void Start()
    {
        dropdown.value = LocalizationSettings.SelectedLocale.SortOrder;
    }

    public void ChangeLocale(int localeID)
    {
        if (active) return;

        StartCoroutine(SetLocale(localeID));
    }

    private IEnumerator SetLocale(int localeID)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
        active = false;
    }
}
