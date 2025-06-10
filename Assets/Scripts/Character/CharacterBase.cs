using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    [Header("Character's Status")]
    [SerializeField] private string nameCharacter;
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentStamina = 0;
    [SerializeField] private int maxStamina = 100;
    [SerializeField] private int damage;
    [SerializeField] private int criticalDmg;
    [SerializeField] private int criticalRate = 0;
    [SerializeField] private int defense;
    [SerializeField] private int level = 1;
    //[SerializeField] private string passiveAbility;

    //Constructor
    public void Init(int maxHealth, int maxStamina, int damage, int defense, int criticalDmg, int criticalRate, int level)
    {
        this.maxHealth = maxHealth;
        this.maxStamina = maxStamina;
        this.damage = damage;
        this.criticalDmg = criticalDmg;
        this.defense = defense;
        this.level = Mathf.Max(1, level);
        this.criticalRate = Mathf.Max(0, criticalRate);

        // Initialize current health and stamina
        CurrentHealth = maxHealth;
        CurrentStamina = 0;

    }

    //Take damage
    public virtual void TakeDmg(int dmg, int criticalDmg, int criticalRate)
    {
        bool isCriticalHit = Random.Range(0f, 1f) <= (criticalRate / 100f);

        int finalDamage = dmg;
        if (isCriticalHit)
        {
            finalDamage += Mathf.RoundToInt(dmg * (criticalDmg / 100f));
        }

        finalDamage = Mathf.Max(0, finalDamage - defense);

        CurrentHealth = Mathf.Max(0, CurrentHealth - finalDamage);
    }

    //Check dead
    public bool isDead()
    {
        return CurrentHealth <= 0;
    }

    //Restore Health
    public void RestoreHealth(int amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
    }

    //Restore Stamina
    public void RestoreStamina(int amount)
    {
        CurrentStamina += amount;
        if (CurrentStamina > MaxStamina)
        {
            CurrentStamina = MaxStamina;
        }
    }



    public string NameCharacter { get => nameCharacter; set => nameCharacter = value; }
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public int Damage { get => damage; set => damage = value; }
    public int Defense { get => defense; set => defense = value; }
    public int CriticalDmg { get => criticalDmg; set => criticalDmg = value; }
    public int CurrentStamina { get => currentStamina; set => currentStamina = value; }
    public int MaxStamina { get => maxStamina; set => maxStamina = value; }
    public int Level { get => level; set => level = (value >= 1) ? value : 1; }
    public int CriticalRate { get => criticalRate; set => criticalRate = Mathf.Clamp(value,0,100); }
}
