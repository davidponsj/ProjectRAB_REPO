using UnityEngine;
using TMPro;

public class AbilityUIController : MonoBehaviour
{
    public TMP_Text abilityText;

    public void SetAbilityText(string t)
    {
        if (abilityText != null)
            abilityText.text = "Habilidad: " + t;
    }
}

