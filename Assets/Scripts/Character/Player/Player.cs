using UnityEngine;

public class Player : CharacterBase
{
    //Player status
    [Header("Elements")]
    [SerializeField] private TypeElement type = TypeElement.None;
    [SerializeField] private Status status = Status.None;
    public PlayerState state;

    //Animator
    private Animator animator;

    //System hp, stamina
    private GameObject healthSystem, hpBox, staminaSystem, staminaBox;

    private int originalDefense;
    private Color originalColor;// Lưu màu gốc của thanh HP

    private void Start()
    {
        
        animator = GetComponent<Animator>();//Animator
        InitializeElementStats();//Init player
        originalDefense = Defense; //Store original defense

        //Health system and health box
        healthSystem = transform.Find("System/Health System").gameObject;   
        hpBox = healthSystem.transform.Find("Hp box").gameObject;
        
        //Stamina system and stamina box
        staminaSystem = transform.Find("System/Stamina system").gameObject;
        staminaBox = staminaSystem.transform.Find("Stamina box").gameObject;

        SpriteRenderer hpBoxRenderer = hpBox.GetComponent<SpriteRenderer>();// Lưu lại màu gốc của thanh HP khi bắt đầu
        if (hpBoxRenderer != null)
        {
            originalColor = hpBoxRenderer.color; // Màu gốc
        }
    }

    //Update
    private void Update()
    {
        UpdateStaminaBar();
        UpdateHpBar();
    }

    // Constructor hoặc Init riêng cho Player
    public void InitPlayer(int maxHealth, int stamina, int damage, int defense, int criticalDmg, int criticalRate, int level)
    {
        // Gọi hàm Init từ lớp cha
        Init(maxHealth, stamina , damage, defense, criticalDmg, criticalRate, level);

    }

    //Initialize element stats
    public void InitializeElementStats()
    {
        Level = PlayerPrefs.GetInt("Hero_Level", 1);
        if (Level == 1)
        {
            //Khởi tạo level 1
            if (type == TypeElement.Water)
            {
                InitPlayer(200, 200, 50, 50, 0, 0, 1);
            }
            else if (type == TypeElement.Ground)
            {
                InitPlayer(230, 100, 40, 80, 0, 0, 1);
            }
            else if (type == TypeElement.Fire)
            {
                InitPlayer(180, 200, 100, 0, 15, 5, 1);
            }
            else if (type == TypeElement.Leaf)
            {
                InitPlayer(200, 200, 65, 35, 0, 0, 1);
            }
            else if (type == TypeElement.Wind)
            {
                InitPlayer(200, 200, 70, 10, 10, 10, 1);
            }
        }
        else
        {
            int maxHealth = PlayerPrefs.GetInt("Hero_HP", 200);
            int maxStamina = PlayerPrefs.GetInt("Hero_Stamina", 150);
            int defense = PlayerPrefs.GetInt("Hero_Defense", 50);
            int damage = PlayerPrefs.GetInt("Hero_Damage", 100);
            int criticalDmg = PlayerPrefs.GetInt("Hero_CriticalDmg", 1);
            int criticalRate = PlayerPrefs.GetInt("Hero_CriticalRate", 5);
            InitPlayer(maxHealth, maxStamina, damage, defense, criticalDmg, criticalRate, Level);
        }
    }

    
    //Defense
    public void PlayerDefense()
    {
        if(state == PlayerState.Defense)
        {
            Defense = (int)(originalDefense * 1.5f);
            if(Defense > 0)
            {
                animator.Play("defense");
            }
            else
            {
                animator.Play("takehit");
            }
        }

    }   

    //Take dmg
    public override void TakeDmg(int dmg, int criticalDmg, int criticalRate)
    {
        base.TakeDmg(dmg, criticalDmg, criticalRate);
        if (isDead())
        {
            state = PlayerState.Death;
            animator.Play("death");
            CurrentStamina = 0;
        }
        else if (state == PlayerState.Defense)
        {
            PlayerDefense();
        }
        else
        {
            animator.Play("takehit");
        }
    }



    //Use special skill
    public void UseSkill(int amount)
    {
        if (state == PlayerState.Skill1 && CurrentStamina >= amount)
        {
            CurrentStamina -= amount;
            
            if(type == TypeElement.Water)
            {
                if(CurrentHealth <= (int)(MaxHealth * 0.15))
                {
                    CurrentHealth += (int)(MaxHealth * 0.4f); //Ability: hp <= 15% restore + 10%
                }
                else
                {
                    CurrentHealth += (int)(MaxHealth * 0.3f);
                }
                if(CurrentHealth > MaxHealth)
                {
                    CurrentHealth = MaxHealth;
                }
            }
            animator.Play("skill1");
        }
        else if (state == PlayerState.Skill2 && CurrentStamina >= amount)
        {
            CurrentStamina -= amount;
            animator.Play("skill2");
        }
        else if (state == PlayerState.Skill3 && CurrentStamina >= amount)
        {
            CurrentStamina -= amount;
            animator.Play("skill3");
        }
        else if (state == PlayerState.SpecialSkill && CurrentStamina >= amount)
        {
            CurrentStamina -= amount;
            animator.Play("special_skill");
        }
        UpdateStaminaBar();
    }

    //Check condition use skill
    public bool CheckConditionStamina(int amount)
    {
        return CurrentStamina >= amount;
    }

    //Base attack
    public void BaseAttack()
    {
        RestoreStamina((int)(MaxStamina * 0.25f));
        UpdateStaminaBar();
        animator.Play("basic_attack");
    }

    public void PlayerAttack()
    {
        FindAnyObjectByType<GameManager>()?.PlayerAttack();
    }

    //Attack and move
    public void AttackAndMove()
    {
        FindAnyObjectByType<GameManager>()?.AttackAndMove();
    }
    public void EndSkill()
    {
        var gm = FindAnyObjectByType<GameManager>();
        gm.EndPlayerSkill();
    }

    //Stamina bar
    public void UpdateStaminaBar()
    {
        float staminaPercentage = (float)CurrentStamina / (float)MaxStamina;
        staminaBox.transform.localScale = new Vector3(staminaPercentage, 1f);
    }
    //Health bar
    public void UpdateHpBar()
    {
        float healthPercentage = (float)CurrentHealth / (float)MaxHealth;
        hpBox.transform.localScale = new Vector3(healthPercentage, 1f);

        // Kiểm tra nếu tỷ lệ HP dưới 30%
        if (healthPercentage < 0.3f)
        {
            // Tạo hiệu ứng Ping Pong màu
            float pingPongValue = Mathf.PingPong(Time.time * 2f, 1f); // Thay đổi tốc độ nhấp nháy bằng cách thay đổi hệ số nhân với Time.time
            Color barColor = new Color(0.0f, 0.6f, 0.2f); // RGB(0, 153, 51)
            Color pingPongColor = Color.Lerp(barColor, Color.blue, pingPongValue);
            SetColor(pingPongColor); // Áp dụng màu cho thanh HP
        }
        else
        {
            // Nếu HP trên 30%, gắn lại màu gốc
            SetColor(originalColor);
        }
    }
    //Set color
    public void SetColor(Color color)
    {
        // Đặt màu cho SpriteRenderer của thanh HP
        SpriteRenderer hpBoxRenderer = hpBox.GetComponent<SpriteRenderer>();
        if (hpBoxRenderer != null)
        {
            hpBoxRenderer.color = color;
        }
    }

    //Getter, Setter
    public TypeElement ElementType { get => type; set => type = value; }
}
