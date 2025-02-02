using UnityEngine;
using UnityEngine.UI;

public class StretchImage : MonoBehaviour
{
    [SerializeField] private Button stretch_btn;
    [SerializeField] private RectTransform rectT;
    [SerializeField] private float speedStretch = 350f;
    [SerializeField] private float maxWidth = 250f;
    [SerializeField] private Sprite solidArrowL;
    [SerializeField] private Sprite solidArrowR;
    [SerializeField] private GameObject optionPanel;

    private bool isStretching = false;

    private void Start()
    {
        rectT.pivot = new Vector2(1, 0);
        stretch_btn.onClick.AddListener(ToggleStretch);
    }

    private void Update()
    {
        if(isStretching)
        {
            if(rectT.sizeDelta.x < maxWidth)
            {
                rectT.sizeDelta += new Vector2(speedStretch * Time.deltaTime, 0);
                // Di chuyển button theo hình ảnh sao cho luôn sát đầu di chuyển của hình ảnh
                Vector2 newButtonPosition = stretch_btn.GetComponent<RectTransform>().anchoredPosition;
                newButtonPosition.x -= speedStretch * Time.deltaTime;  // Di chuyển button về phía trái sao cho sát đầu hình ảnh
                stretch_btn.GetComponent<RectTransform>().anchoredPosition = newButtonPosition;
            }
            else
            {
                stretch_btn.image.sprite = solidArrowR;
                optionPanel.SetActive(true);
            }
        }
        else
        {
            optionPanel.SetActive(false);
            if(rectT.sizeDelta.x > 0)
            {
                rectT.sizeDelta -= new Vector2(speedStretch * Time.deltaTime, 0);
                Vector2 newButtonPos = stretch_btn.GetComponent<RectTransform>().anchoredPosition;
                newButtonPos.x += speedStretch * Time.deltaTime;
                stretch_btn.GetComponent<RectTransform>().anchoredPosition = newButtonPos;
            }
            else
            {
                stretch_btn.image.sprite = solidArrowL;
            }
        }
    }

    public void ToggleStretch()
    {
        isStretching = !isStretching;
    }
}
