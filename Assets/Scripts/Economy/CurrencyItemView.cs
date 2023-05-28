using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyItemView : MonoBehaviour
{
    public string definitionId;

    public TextMeshProUGUI balanceField;

    public void SetBalance(long balance)
    {
        balanceField.text = balance.ToString();
    }
}
