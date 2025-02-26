using System.Collections.Generic;
using UnityEngine;

public class AI03 : MonoBehaviour
{

    public enum motion_pattern { patrol, random, homing, lead };
    public enum gunfire_pattern { none, anywhere, focus };

    public motion_pattern pattern1 = motion_pattern.patrol;
    public gunfire_pattern pattern2 = gunfire_pattern.none;

    public List<string> targets;
    public float damage = 1f;
    public float speed = 4f;

    public float rollout;
    public float VelocityRate = 0.2f;
    public float speed_rotary = 150f;

    public float FireDelay = 3f;
    public float lead = 3f;

    private float FireStart;
    private LevelManager LM;
    private float width;
    private float offset;
    private float height;

    private List<Vector2> track = new();
    private Vector2 track_single;
    private Vector2 goal_track;
    private float AngularDamp;
    private Vector2 goal_velocity;
    private float goal_rotation;
    private int node = 1;

    private Destructible clank;
    private Rigidbody2D body;
    private Rigidbody2D rig;

    private List<Gun> Guns = new();
    private Vector2 target;

    private GameObject enemy;

    private void Awake()
    {
        body = gameObject.GetComponent<Rigidbody2D>();
        LM = FindFirstObjectByType<LevelManager>();
        AngularDamp = speed_rotary * Time.fixedDeltaTime;

        if (pattern2 != gunfire_pattern.none)
        {
            Guns.Clear();
            Guns.AddRange(GetComponentsInChildren<Gun>());

            for (var i = 0; i < Guns.Count; i++)
                Guns[i].Init(targets, 0);
        }
    }

    private void OnEnable()
    {
        height = LM.height;
        width = LM.width;
        offset = LM.offset;

        rollout = offset * offset;
        track.Clear();

        switch (pattern1)
        {
            case motion_pattern.patrol:
                goal_velocity.Set(offset - width / 2, transform.position.y);
                track.Add(goal_velocity);
                goal_velocity.Set(-offset + width / 2, transform.position.y);
                track.Add(goal_velocity);
                node = 1;
                break;
            case motion_pattern.random:
                track_single.Set(Random.Range(-width / 2 + offset, width / 2 - offset), Random.Range(-height / 2 + offset, height / 2 - offset));
                break;
        }

        if (pattern2 != gunfire_pattern.none)
            FireStart = Time.time + FireDelay;

        enemy = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (damage > 0)
            if (targets.Contains(other.gameObject.tag))
            {
                clank = other.gameObject.GetComponent<Destructible>();
                if (clank)
                    clank.TakeDamage(damage);

                rig = other.GetComponent<Rigidbody2D>();
                if (rig && body)
                    rig.AddForceAtPosition(damage * body.linearVelocity, transform.position, ForceMode2D.Impulse);

                gameObject.GetComponent<Destructible>().Destruct();
            }
    }

    private void Update()
    {
        switch (pattern1)
        {
            case motion_pattern.patrol:
                goal_track = track[node] - (Vector2)transform.position;
                if (goal_track.sqrMagnitude < rollout)
                    node = (node + 1) % 2;
                break;

            case motion_pattern.random:
                goal_track = track_single - (Vector2)transform.position;
                if (goal_track.sqrMagnitude < rollout)
                    track_single.Set(Random.Range(-width / 2 + offset, width / 2 - offset), Random.Range(-height / 2 + offset, height / 2 - offset));
                break;

            case motion_pattern.homing:
                if (enemy)
                    goal_track = (Vector2)enemy.transform.position - (Vector2)transform.position;
                break;

            case motion_pattern.lead:
                if (enemy)
                {
                    goal_track = (Vector2)enemy.transform.position - (Vector2)transform.position;
                    goal_track.Normalize();
                    goal_track = (Vector2)enemy.transform.position - (Vector2)transform.position - lead * goal_track;
                }
                break;
        }

        goal_velocity.Set(-Mathf.Sin(Mathf.Deg2Rad * body.rotation), Mathf.Cos(Mathf.Deg2Rad * body.rotation));
        ;
        goal_velocity *= speed;

        if (goal_track.x < 0)
            goal_rotation = Vector2.Angle(Vector2.up, goal_track);
        else
            goal_rotation = 360 - Vector2.Angle(Vector2.up, goal_track);

        switch (pattern2)
        {
            case gunfire_pattern.none:
                break;

            case gunfire_pattern.anywhere:
                if (body.linearVelocity.x > 0)
                    target = (Vector2)transform.position + Vector2.right;
                else
                    target = (Vector2)transform.position + Vector2.left;

                for (var i = 0; i < Guns.Count; i++)
                {
                    Guns[i].SetFire(enemy && Time.time > FireStart);
                    Guns[i].Aim(target);
                }
                break;

            case gunfire_pattern.focus:
                if (enemy)
                    target = enemy.transform.position;

                for (var i = 0; i < Guns.Count; i++)
                {
                    Guns[i].SetFire(enemy && Time.time > FireStart);
                    Guns[i].Aim(target);
                }
                break;
        }
    }

    private void FixedUpdate()
    {
        body.linearVelocity += (goal_velocity - body.linearVelocity) * VelocityRate;
        body.angularVelocity = speed_rotary * Mathf.Clamp(Mathf.DeltaAngle(body.rotation, goal_rotation) / AngularDamp, -1, 1);
    }
}
