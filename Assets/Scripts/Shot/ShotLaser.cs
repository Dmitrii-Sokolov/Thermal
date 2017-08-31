using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShotLaser : MonoBehaviour {

    public string ShotHit = "HitLaser";
    public float HitLifeTime = 1f;

    public float Disappearing_speed = 1f;

    private float alpha;
    private LineRenderer line;
    private GameObject ShotHit_object;
    private Destructible clank;

    private Color color1;
    private Color color2;

    private RaycastHit2D[] hit;


    void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    public void Init(List<string> targets, float damage, Vector2 trace, float range, Color color1_new, Color color2_new)
    {
        color1 = color1_new;
        color2 = color2_new;

        alpha = 1f;
        hit = Physics2D.RaycastAll(transform.position, trace, range);

        if (line)
        {
            line.SetColors(color1, color2);
            line.SetPosition(0, transform.position);
            line.SetPosition(1, transform.position + (Vector3) trace * range);
        }

        for (int i = 0; i < hit.Length; i++)
        {
            if (targets.Contains(hit[i].transform.tag))
            {
                ShotHit_object = PoolManager.GetObject(ShotHit, hit[i].point, transform.rotation);
                if (ShotHit_object)
                    ShotHit_object.GetComponent<PoolObject>().ReturnToPool(HitLifeTime);

                clank = hit[i].transform.GetComponent<Destructible>();
                if (clank)
                    clank.TakeDamage(damage);
            }
        }
    }

    void Update()
    {
        alpha -= Time.deltaTime * Disappearing_speed;

        if (alpha<0)
            gameObject.GetComponent<PoolObject>().ReturnToPool();
        else
            line.SetColors(new Color(color1.r, color1.g, color1.b, alpha), new Color(color2.r, color2.g, color2.b, alpha));

    }

}
