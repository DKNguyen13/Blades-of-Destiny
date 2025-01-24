using UnityEngine;
using TMPro;

public class FadeOutText : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public float fadeSpeed = 1.5f;
    public float displayTime = 1f;

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        if(messageText != null)
        {
            messageText.canvasRenderer.SetAlpha(0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (messageText != null && timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else if (messageText != null && timer <= 0)
        {
            // Bắt đầu làm mờ dần
            float alpha = Mathf.Lerp(messageText.canvasRenderer.GetAlpha(), 0, Time.deltaTime * fadeSpeed);
            messageText.canvasRenderer.SetAlpha(alpha);
        }
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;
        messageText.canvasRenderer.SetAlpha(1f);
        timer = displayTime;
    }
}
