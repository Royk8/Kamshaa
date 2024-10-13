using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEditor;
using UnityEngine.UI;

public class LocalizeDropdown : MonoBehaviour
{
    [SerializeField] private List<LocalizedString> dropdownOptions;
    private Dropdown _tmpDropdown;

    private void Awake()
    {
        if (_tmpDropdown == null)
            _tmpDropdown = GetComponent<Dropdown>();

        LocalizationSettings.SelectedLocaleChanged += ChangedLocale;
    }

    private void ChangedLocale(Locale newLocale)
    {
        List<Dropdown.OptionData> tmpDropdownOptions = new List<Dropdown.OptionData>();
        foreach (var dropdownOption in dropdownOptions)
        {
            tmpDropdownOptions.Add(new Dropdown.OptionData(dropdownOption.GetLocalizedString()));
        }
        _tmpDropdown.options = tmpDropdownOptions;
    }
}

public abstract class AddLocalizeDropdown
{
    [MenuItem("CONTEXT/Dropdown/Localize", false, 1)]
    private static void AddLocalizeComponent()
    {
        // add localize dropdown component to selected gameobject
        GameObject selected = Selection.activeGameObject;
        if (selected != null)
        {
            selected.AddComponent<LocalizeDropdown>();
        }
    }
}