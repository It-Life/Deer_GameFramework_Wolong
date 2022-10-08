using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class HealthBarEventHandler : MonoBehaviour 
{
    [Header("Event calls when health decrease.")]
    public UnityEvent OnHealthDecrease;
    [Header("Event calls when health is 0.")]
    public UnityEvent OnDeath;
    [Header("Event calls when health increase.")]
    public UnityEvent OnHealthIncrease;
    [Header("Event calls when health changing.")]
    public UnityEvent OnHalthChange;
    [Header("Event calls when health become full.")]
    public UnityEvent OnHealthRestored;

    private HealthBar healthBar;
    private float curHealth;
    private float lastHealth;
    private bool isDead;
    private int hitsCount;

	// Use this for initialization
	void Start () {
        healthBar = GetComponent<HealthBar>();
        lastHealth = healthBar.GetDefaultHealth();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    curHealth = healthBar.GetCurrentHealth();

        if(curHealth <= 0 && !isDead)
        {
            OnDeath.Invoke();
            isDead = true;
        }
        else if (lastHealth < curHealth && curHealth != 0)
        {
            OnHealthIncrease.Invoke();
            OnHalthChange.Invoke();
            lastHealth = curHealth;
            isDead = false;
        }
        else if (lastHealth > curHealth)
        {
            OnHealthDecrease.Invoke();
            OnHalthChange.Invoke();
            hitsCount++;
            lastHealth = curHealth;
        }
        else if (curHealth == healthBar.GetDefaultHealth() && hitsCount > 0)
        {
            OnHealthRestored.Invoke();
            ResetHitsCount();
        }
	}


    void ResetHitsCount()
    {
        hitsCount = 0;
    }
}
