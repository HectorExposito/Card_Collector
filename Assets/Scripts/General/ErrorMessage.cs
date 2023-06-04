using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ErrorMessage : MonoBehaviour
{
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private GameObject errorMessagePanel;
    public void ShowErrorMessage(string message)
    {
        errorText.text = message;
        errorMessagePanel.SetActive(true);
    }
}
