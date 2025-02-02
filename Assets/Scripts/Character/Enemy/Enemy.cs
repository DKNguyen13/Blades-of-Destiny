using UnityEngine;

public class Enemy : CharacterBase
{
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
        InitStat();
    }

    public void InitStat()
    {
        int lvl = PlayerPrefs.GetInt("lvl", 1);
        startLevel = PlayerPrefs.GetInt("Hero_Level", 1);
        statModifier = (enemyType == EnemyType.Monster) ? 100 : 1000;
        endLevel = (lvl <= 2) ? (startLevel + 2) : (startLevel + 5);
        Level = Random.Range(startLevel, endLevel + 1);
        int maxHp = Level * statModifier * 2;
        int maxSta = maxHp;
        int dmg = Level * 20;
        int def = Level * 10;
        int critDmg = Level * 3;
        int critRate = Level * 5;
        Init(maxHp, maxSta, dmg, def, critDmg, critRate, Level);
    }

    private void Update()
    {
        if (isDead())
        {
            Die();
        }
    }

    public override void TakeDmg(int dmg, int criticalDmg, int criticalRate)
    { 
        if (isDead())
        {
            Die();
        }
        else
        {
            base.TakeDmg(dmg, criticalDmg, criticalRate);
            UpdateHpBar();
            animator.Play("take_hit");
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
        animator.Play("base_atk");
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
        animator.Play("death");
        //StartCoroutine(DestroyAfterAnimationDeath());
    }

    /*
    IEnumerator DestroyAfterAnimationDeath()
    {
        yield return null;
        // Đảm bảo animation 'death' đã thực sự bắt đầu
        float deathAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        //Debug.Log(deathAnimationLength + " s");
        yield return new WaitForSeconds(deathAnimationLength + 0.3f);
        Destroy(gameObject);
    }
    */

    public void OnDestroy()
    {
        Destroy(gameObject);
    }
}
