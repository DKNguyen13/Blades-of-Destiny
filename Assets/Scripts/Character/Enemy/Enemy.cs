using System.Collections;
using UnityEngine;

public class Enemy : CharacterBase
{
    public enum EnemyType
    {
        Monster, Boss
    }

    [Header("Enemy's status")]
    [SerializeField] private int statModifier = 1;
    [SerializeField] private EnemyType enemyType = EnemyType.Monster;
    [SerializeField] private int startLevel = 1;
    [SerializeField] private int endLevel = 100;

    //Animator
    private Animator animator;

    //Hp bar enemy
    private GameObject healthSystem;
    private GameObject hpBox;

    // Lưu màu gốc của thanh HP
    private Color originalColor;


    private void Start()
    {
        Level = Random.Range(startLevel, endLevel + 1);
        statModifier = enemyType == EnemyType.Monster ? 1 : 10;//1 * statModifier * Level
        Init(100 * statModifier * Level, 100 * statModifier * Level, 10 * statModifier * Level , 1 * Level, 5 * Level, Level, CriticalRate);

        animator = GetComponent<Animator>();
        // Tìm đối tượng Health System và HpBox con
        healthSystem = transform.Find("Health System").gameObject;
        hpBox = healthSystem.transform.Find("Hp box").gameObject;

        // Lưu lại màu gốc của thanh HP khi bắt đầu
        SpriteRenderer hpBoxRenderer = hpBox.GetComponent<SpriteRenderer>();
        if (hpBoxRenderer != null)
        {
            originalColor = hpBoxRenderer.color; // Màu gốc
        }
    }

    public override void TakeDmg(int dmg, int criticalDmg, int criticalRate)
    {
        base.TakeDmg(dmg, criticalDmg, criticalRate);
        UpdateHpBar();
        if (isDead())
        {
            Die();
        }
    }

    public void UpdateHpBar()
    {
        float healthPercentage = (float)CurrentHealth / (float)MaxHealth;
        hpBox.transform.localScale = new Vector3(healthPercentage, 1f);

        // Kiểm tra nếu tỷ lệ HP dưới 30%
        if (healthPercentage < 0.3f)
        {
            // Tạo hiệu ứng Ping Pong màu
            float pingPongValue = Mathf.PingPong(Time.time * 2f, 1f); // Thay đổi tốc độ nhấp nháy bằng cách thay đổi hệ số nhân với Time.time
            Color pingPongColor = Color.Lerp(Color.red, Color.yellow, pingPongValue); // Chuyển từ đỏ sang vàng
            SetColor(pingPongColor); // Áp dụng màu cho thanh HP
        }
        else
        {
            // Nếu HP trên 30%, gắn lại màu gốc
            SetColor(originalColor);
        }
    }

    public void BaseAttack()
    {
        animator.SetTrigger("atk1");
    }

    public void SetColor(Color color)
    {
        // Đặt màu cho SpriteRenderer của thanh HP
        SpriteRenderer hpBoxRenderer = hpBox.GetComponent<SpriteRenderer>();
        if (hpBoxRenderer != null)
        {
            hpBoxRenderer.color = color;
        }
    }


    //Die
    public void Die()
    {
        animator.SetTrigger("death");
        StartCoroutine(DestroyAfterAnimationDeath());
    }

    IEnumerator DestroyAfterAnimationDeath()
    {
        // Đảm bảo animation 'death' đã thực sự bắt đầu
        float deathAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        //Debug.Log(deathAnimationLength + " s");
        yield return new WaitForSeconds(deathAnimationLength);
        Destroy(gameObject);
    }

}
