using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Destructible : MonoBehaviour {

    public float health_max = 2f;
    public int ScoreValue = 2;
    public string Explosion = "Explosion01";
    public float ExplosionTime = 2f;

    public Slider HealthBar;

    public float loot_chance;
    public List<string> loot;

    private float health = 2f;

    private GameObject Explosion_object;
    private PoolObject Try_return;

    private UIController UI;
    private LevelManager LM;

    void Awake()
    {
        LM = FindObjectOfType<LevelManager>();
        UI = FindObjectOfType<UIController>();
    }

    void OnEnable()
    {
        health = health_max;

        if (gameObject.tag != "Player")
            LM.EnemyCount++;
        else
        {
            if (GameObject.Find("PlayerHealth"))
                HealthBar = GameObject.Find("PlayerHealth").GetComponent<Slider>(); ;
        }

        if (HealthBar)
            HealthBar.value = 1000 * health / health_max;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (HealthBar)
            HealthBar.value = 1000 * health / health_max;

        if (health <= 0)
        {
            Destruct();
            Score();
        }
    }

    public void Repair(float plus)
    {
        health = Mathf.Clamp(health + plus,0, health_max);
        if (HealthBar)
            HealthBar.value = 1000 * health / health_max;
    }

    private void Score()
    {
        if (gameObject.tag != "Player")
            UI.ScoreUp(ScoreValue);
        else
            UI.EndGame();
    }

    public void Destruct()
    {
        Explosion_object = PoolManager.GetObject(Explosion, transform.position, transform.rotation);
        if (Explosion_object)
            Explosion_object.GetComponent<PoolObject>().ReturnToPool(ExplosionTime);

        Try_return = gameObject.GetComponent<PoolObject>();
        if (Try_return)
            Try_return.ReturnToPool();
        else
            Destroy(gameObject);

        if (loot.Count > 0)
            if (Random.value < loot_chance)
                PoolManager.GetObject(loot[Random.Range(0, loot.Count)], transform.position, transform.rotation);
    }

    void OnDisable()
    {
        LM.EnemyCount--;
    }
}
