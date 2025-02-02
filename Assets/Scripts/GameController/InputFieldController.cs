using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldController : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button button;
    // Start is called before the first frame update
    void Start()
    {
        inputField.onValueChanged.AddListener(OnInputFieldChanged);
    }

    public void OnInputFieldChanged(string text)
    {
        if (string.IsNullOrEmpty(text.Trim()) || char.IsDigit(text[0]))
        {
            button.gameObject.SetActive(false);
        }
        else button.gameObject.SetActive(true);
    }
}
