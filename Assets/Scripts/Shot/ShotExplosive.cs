using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShotExplosive : MonoBehaviour {

    public string ShotHit = "HitExplosive";
    public float HitLifeTime = 1f;

    private float damage;
    private float radius;
    private List<string> targets;
    private Rigidbody2D myrig;
    private SpriteRenderer look;

    private GameObject ShotHit_object;
    private Destructible clank;
    private Rigidbody2D rig;

    private Collider2D[] hit;
    private Vector2 Force;

    private float time_end;

    void Awake()
    {
        look = GetComponent<SpriteRenderer>();
        myrig = GetComponent<Rigidbody2D>();
    }

    public void Init(List<string> targets_new, float damage_new, Vector2 point_end, Vector2 speed, float radius_mod, Color color1, Color color2)
    {
        look.color = new Color(Random.Range(color1.r, color2.r), Random.Range(color1.g, color2.g), Random.Range(color1.b, color2.b));
        targets = targets_new;
        damage = damage_new;
        radius = 5f * radius_mod;
        myrig.linearVelocity = speed;
        time_end = Time.time + Mathf.Sqrt((point_end - (Vector2) transform.position).sqrMagnitude / speed.sqrMagnitude);

        if (myrig.linearVelocity.x < 0)
            myrig.rotation = Vector2.Angle(Vector2.up, myrig.linearVelocity);
        else
            myrig.rotation = 360 - Vector2.Angle(Vector2.up, myrig.linearVelocity);
    }

    void FixedUpdate()
    {
        if (Time.time > time_end)
            Explosive();
    }

    void Explosive()
    {
        hit = Physics2D.OverlapCircleAll(transform.position, radius);

        ShotHit_object = PoolManager.GetObject(ShotHit, transform.position, transform.rotation);

        if (ShotHit_object)
            ShotHit_object.GetComponent<PoolObject>().ReturnToPool(HitLifeTime);

        for (int i = 0; i < hit.Length; i++)
        {
            if (targets.Contains(hit[i].transform.tag))
            {
                clank = hit[i].transform.GetComponent<Destructible>();
                if (clank)
                    clank.TakeDamage(damage);

                rig = hit[i].GetComponent<Rigidbody2D>();
                if (rig)
                {
                    Force = hit[i].transform.position - transform.position;
                    Force *= 10 * damage * radius * Mathf.Clamp01(1 / Force.SqrMagnitude());
                    rig.AddForce(Force, ForceMode2D.Impulse);
                }
            }
        }

        gameObject.GetComponent<PoolObject>().ReturnToPool();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (targets.Contains(other.gameObject.tag))
            Explosive();
    }
}
