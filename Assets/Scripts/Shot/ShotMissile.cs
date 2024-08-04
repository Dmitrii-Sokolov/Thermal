using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShotMissile : MonoBehaviour {

    public string ShotHit = "HitMissile";
    public float HitLifeTime = 1f;

    private float damage;
    private List<string> targets;
    private Rigidbody2D myrig;
    private SpriteRenderer look;

    private GameObject ShotHit_object;
    private Destructible clank;
    private Rigidbody2D rig;

    private GameObject target;
    private GameObject[] target_try;
    private float target_range_1;
    private float target_range_2;

    private Vector2 target_local;
    private Vector2 goal_velocity;
    private float goal_rotation;
    public float speed_rotary = 300f;
    public float VelocityRate = 0.1f;
    private float speed_max;
    private float AngularDamp;

    void Awake()
    {
        look = GetComponent<SpriteRenderer>();
        myrig = GetComponent<Rigidbody2D>();
        target = null;
        AngularDamp = speed_rotary * Time.fixedDeltaTime;
    }

    public void Init(List<string> targets_new, float damage_new, Vector2 speed, float speed_max_new, Vector2 target_lock, Color color1, Color color2)
    {
        look.color = new Color(Random.Range(color1.r, color2.r), Random.Range(color1.g, color2.g), Random.Range(color1.b, color2.b));
        targets = targets_new;
        damage = damage_new;
        myrig.linearVelocity = speed;
        speed_max = speed_max_new;

        if (myrig.linearVelocity.x < 0)
            myrig.rotation = Vector2.Angle(Vector2.up, myrig.linearVelocity);
        else
            myrig.rotation = 360 - Vector2.Angle(Vector2.up, myrig.linearVelocity);

        target = FindTarget(target_lock);
    }

    void FixedUpdate()
    {
        if (!target)
                target = FindTarget(transform.position);
        else
        {
            if (!target.gameObject.activeSelf)
                target = FindTarget(transform.position);
            else
                Homing(target.transform.position);

        }
    }

    void Homing(Vector2 goal)
    {
        target_local = goal - (Vector2) transform.position;
        target_local.Normalize();

        if (target_local.x < 0)
            goal_rotation = Vector2.Angle(Vector2.up, target_local);
        else
            goal_rotation = 360 - Vector2.Angle(Vector2.up, target_local);
        myrig.angularVelocity = speed_rotary * Mathf.Clamp(Mathf.DeltaAngle(myrig.rotation, goal_rotation) / AngularDamp, -1, 1);

        //goal_velocity = target_local;
        goal_velocity.Set(-Mathf.Sin(Mathf.Deg2Rad * (myrig.rotation)), Mathf.Cos(Mathf.Deg2Rad * (myrig.rotation))); ;
        goal_velocity *= speed_max;
        myrig.linearVelocity += (goal_velocity - myrig.linearVelocity) * VelocityRate;
    }

    GameObject FindTarget(Vector2 target_lock)
    {
        GameObject target_new = null;
        target_range_1 = Mathf.Infinity;

        for (int i = 0; i < targets.Count; i++)
        {
            target_try = GameObject.FindGameObjectsWithTag(targets[i]);
            for (int n = 0; n < target_try.Length; n++)
            {
                target_range_2 = ((Vector2)target_try[n].transform.position - target_lock).SqrMagnitude();
                if (target_range_2 < target_range_1)
                {
                    target_new = target_try[n];
                    target_range_1 = target_range_2;
                }
            }
        }
        return target_new;
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
                rig.AddForceAtPosition(damage * myrig.linearVelocity, transform.position, ForceMode2D.Impulse);

            gameObject.GetComponent<PoolObject>().ReturnToPool();
        }
    }
}
