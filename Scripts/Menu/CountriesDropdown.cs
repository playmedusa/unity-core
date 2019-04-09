using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mgl;


public class CountriesDropdown : MonoBehaviour
{
	Dropdown dropdown;

	// Start is called before the first frame update
	void Awake()
	{
		dropdown = GetComponent<Dropdown>();
		dropdown.ClearOptions();

		List<string> dropOptions = new List<string> {
			I18n.t("Belgium"),
			I18n.t("Cyprus"),
			I18n.t("Denmark"),
			I18n.t("France"),
			I18n.t("Greece"),
			I18n.t("Italy"),
			I18n.t("Malta"),
			I18n.t("Norway"),
			I18n.t("Portugal"),
			I18n.t("Slovenia"),
			I18n.t("Spain"),
			I18n.t("Sweden"),
			I18n.t("United Kingdom"),
			I18n.t("Brazil"),
			I18n.t("Chile"),
			I18n.t("Colombia"),
			I18n.t("Ecuador"),
			I18n.t("El Salvador"),
			I18n.t("Guatemala"),
			I18n.t("Nicaragua"),
			I18n.t("Paraguay"),
			I18n.t("Dominican Republic"),
			I18n.t("Uruguay"),
			I18n.t("Venezuela"),
			I18n.t("United States of America"),
			I18n.t("Canada"),
			I18n.t("Benin"),
			I18n.t("Ivory Coast"),
			I18n.t("Gambia"),
			I18n.t("Ghana"),
			I18n.t("La Réunion"),
			I18n.t("Mauritaria"),
			I18n.t("Senegal"),
			I18n.t("Togo"),
			I18n.t("Australia"),
		};
		dropOptions.Sort();

		dropdown.AddOptions(dropOptions);
	}
}
