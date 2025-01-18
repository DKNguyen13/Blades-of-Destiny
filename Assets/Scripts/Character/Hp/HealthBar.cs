using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Transform bar;
    private float currentHealthPercent; // Lưu giá trị phần trăm HP hiện tại
    [SerializeField] private float lerpSpeed = 5f; // Tốc độ trượt
    private float colorChangeSpeed = 1f; // Tốc độ thay đổi màu (có thể điều chỉnh)

    // Start is called before the first frame update
    void Start()
    {
        bar = transform.Find("HealthBar/Bar");
        currentHealthPercent = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        // Trượt từ currentHealthPercent đến healthPercent mỗi frame
        bar.localScale = new Vector3(Mathf.Lerp(bar.localScale.x, currentHealthPercent, Time.deltaTime * lerpSpeed), 1f, 1f);
        if(currentHealthPercent < .2f)
        {
            // Sử dụng PingPong để tạo hiệu ứng thay đổi màu từ trắng đến đỏ và ngược lại
            float lerpFactor = Mathf.PingPong(Time.time * colorChangeSpeed, 1f);
            Color lerpedColor = Color.Lerp(Color.white, Color.red, lerpFactor);
            SetColor(lerpedColor);
        }
    }

    public void SetHealth(float healthPercent)
    {
        //bar.localScale = new Vector3(healthPercent,1f,1f);
        // Cập nhật phần trăm HP mục tiêu
        currentHealthPercent = healthPercent;
    }
    
    public void SetColor(Color color)
    {
        bar.Find("BarSprite").GetComponent<SpriteRenderer>().color = color;
    }
}
