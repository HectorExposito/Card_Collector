using UnityEngine;
using TMPro;
public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private ErrorMessage errorMesage;
    private void Awake()
    {
        if (PlayerPrefs.HasKey("Name"))
        {
            nameText.text = PlayerPrefs.GetString("Name");
            this.gameObject.SetActive(false);
        }
    }
    
    public void SubmitName()
    {
        string name = nameInputField.text;
        name = name.Trim();
        if (name.Length > 10 || name.Length < 3)
        {
            nameInputField.text = "";
            errorMesage.ShowErrorMessage("NOMBRE NO VÁLIDO.\nEl nombre debe tener más de 3 caracteres y menos de 10.");
            return;
        }
        PlayerPrefs.SetString("Name",name);
        nameText.text = PlayerPrefs.GetString("Name");
        this.gameObject.SetActive(false);
    }
    
}
