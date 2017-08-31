using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShotBullet : MonoBehaviour {

    public string ShotHit = "HitBullet";
    public float HitLifeTime = 1f;

    private float damage;
    private List<string> targets;
    private Rigidbody2D myrig;
    private SpriteRenderer look;

    private GameObject ShotHit_object;
    private Destructible clank;
    private Rigidbody2D rig;

    void Awake()
    {
        look = GetComponent<SpriteRenderer>();
        myrig = GetComponent<Rigidbody2D>();
    }

    public void Init(List<string> targets_new, float damage_new, Vector2 speed, Color color1, Color color2)
    {
        look.color = new Color(Random.Range(color1.r, color2.r), Random.Range(color1.g, color2.g), Random.Range(color1.b, color2.b));
        targets = targets_new;
        damage = damage_new;
        myrig.velocity = speed;
        if (myrig.velocity.x < 0)
            myrig.rotation = Vector2.Angle(Vector2.up, myrig.velocity);
        else
            myrig.rotation = 360 - Vector2.Angle(Vector2.up, myrig.velocity);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (targets.Contains(other.gameObject.tag))
        {
            ShotHit_object = PoolManager.GetObject(ShotHit, transform.position, transform.rotation);
            if (ShotHit_object)
                ShotHit_object.GetComponent<PoolObject>().ReturnToPool(HitLifeTime);

            clank = other.gameObject.GetComponent<Destructible>();
            if (clank)
                clank.TakeDamage(damage);

            rig = other.GetComponent<Rigidbody2D>();
            if (rig)
                rig.AddForceAtPosition(damage*myrig.velocity, transform.position, ForceMode2D.Impulse);

            gameObject.GetComponent<PoolObject>().ReturnToPool();
        }
    }
}
