using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleVisuals : MonoBehaviour
{

    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI levelText;

    private int currHealth;
    private int maxHealth;
    private int level;


    private Animator anim;

    private const string LEVEL_ABB = "lvl: ";

    private const string IS_ATTACK_PARAM = "IsAttack";
    private const string IS_HIT_PARAM = "IsHit";
    private const string IS_DEAD_PARAM = "IsDead";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    public void SetStartingValues(int _currHealth, int _maxHealth, int _level)
    {
        this.currHealth = _currHealth;
        this.maxHealth = _maxHealth;
        this.level = _level;
        levelText.text = LEVEL_ABB + this.level.ToString();
        UpdateHealthBar();
    }

    public void ChangeHealth(int _currHealth)
    {
        this.currHealth = _currHealth;

        if(currHealth <= 0)
        {
            PlayDeathAnimation();
            Destroy(gameObject, 1f);
        }

        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = currHealth;
    }


    public void PlayAttackAnimation()
    {
        anim.SetTrigger(IS_ATTACK_PARAM);
    }

    public void PlayHitAnimation()
    {
        if (anim != null && gameObject != null)
        {
            anim.SetTrigger(IS_HIT_PARAM);
        }
    }

    public void PlayDeathAnimation()
    {
        anim.SetTrigger(IS_DEAD_PARAM);
    }
}
