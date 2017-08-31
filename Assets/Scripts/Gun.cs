using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gun : MonoBehaviour {

    public float lifetime = 10f;
    public int item = 0;
    public List<WeaponSet> Options = new List<WeaponSet>();

    private float nextFire = 0f;
    private bool isFire = false;

    private Vector2 target;
    private Vector2 target_local;
    private Vector2 front;
    private float angle_firing;
    private GameObject shot;

    private Rigidbody2D myrig;
    private List<string> targets;

    private AudioSource audio_out;
    private SpriteRenderer look_out;

    private float level_rate = 1f;
    private float level_damage = 1f;
    private float level_accuracy = 1f;
    private float level_shot = 1f;

    [System.Serializable]
    public struct WeaponSet
    {
        public enum weapon { shell, missile, laser, explosive, none };
        public weapon type;

        public string shot_name;

        public float fireDelay;
        public float damage;
        public float RandomRate;
        public float ShotSpeed;
        public bool isLeadAngle;

        public AudioClip beep;
        public float picthUp;
        public float picthDown;

        public float allowance;

        public Sprite look;
        public Color color;
        public Color color_shot1;
        public Color color_shot2;
    }

    public void Init(List<string> targets_new, int item_new)
    {
        audio_out = GetComponent<AudioSource>();
        look_out = GetComponent<SpriteRenderer>();

        myrig = GetComponentInParent<Rigidbody2D>();

        targets = targets_new;
        item = item_new;
        audio_out.clip = Options[item].beep;
        look_out.sprite = Options[item].look;
        look_out.color = Options[item].color;
    }

    public void SetFire(bool isFire_new)
    {
        isFire = isFire_new;
    }

	void FixedUpdate()
    {
        if ((Time.time > nextFire) && isFire)
            Fire();
    }

    public void Aim(Vector2 target_new)
    {
        target = target_new;
        target_local = target - (Vector2)transform.position;

        //if (Options[item].isLeadAngle)
          //  target_local -= myrig.velocity * target_local.SqrMagnitude() /
            //        (Mathf.Sqrt(Options[item].ShotSpeed * Options[item].ShotSpeed * level_shot * level_shot * target_local.SqrMagnitude() -
              //      (target_local.x * myrig.velocity.y - target_local.y * myrig.velocity.x) *
                //    (target_local.x * myrig.velocity.y - target_local.y * myrig.velocity.x))
                  //  + target_local.x * myrig.velocity.x + target_local.y * myrig.velocity.y);

        if (target_local.x < 0)
            transform.rotation = Quaternion.Euler(0, 0, Vector2.Angle(Vector2.up, target_local));
        else
            transform.rotation = Quaternion.Euler(0, 0, 360 - Vector2.Angle(Vector2.up, target_local));
    }

    void Fire()
    {
        angle_firing = -Mathf.Deg2Rad * (transform.eulerAngles.z + (Random.value - 0.5f) * Options[item].RandomRate * level_accuracy);
        front.Set(Mathf.Sin(angle_firing), Mathf.Cos(angle_firing));

        switch (Options[item].type)
        {
            case WeaponSet.weapon.shell:
                shot = PoolManager.GetObject(Options[item].shot_name, (Vector3)(Options[item].allowance * front) + transform.position, Quaternion.identity);
                if (shot)
                    shot.GetComponent<ShotBullet>().Init(targets, Options[item].damage * level_damage, Options[item].ShotSpeed * front * level_shot, Options[item].color_shot1, Options[item].color_shot2);
                //shot.GetComponent<ShotBullet>().Init(targets, Options[item].damage * level_damage, myrig.velocity + Options[item].ShotSpeed * front * level_shot, Options[item].color_shot1, Options[item].color_shot2);
                break;

            case WeaponSet.weapon.missile:
                shot = PoolManager.GetObject(Options[item].shot_name, (Vector3)(Options[item].allowance * front) + transform.position, Quaternion.identity);
                if (shot)
                    shot.GetComponent<ShotMissile>().Init(targets, Options[item].damage * level_damage, Options[item].ShotSpeed * front * level_shot, Options[item].ShotSpeed * level_shot, target, Options[item].color_shot1, Options[item].color_shot2);
                break;

            case WeaponSet.weapon.laser:
                shot = PoolManager.GetObject(Options[item].shot_name, transform.position, transform.rotation);
                if (shot)
                    shot.GetComponent<ShotLaser>().Init(targets, Options[item].damage * level_damage, front, Options[item].ShotSpeed * level_shot, Options[item].color_shot1, Options[item].color_shot2);
                break;

            case WeaponSet.weapon.explosive:
                shot = PoolManager.GetObject(Options[item].shot_name, (Vector3)(Options[item].allowance * front) + transform.position, Quaternion.identity);
                if (shot)
                    shot.GetComponent<ShotExplosive>().Init(targets, Options[item].damage * level_damage, target, Options[item].ShotSpeed * front, level_shot, Options[item].color_shot1, Options[item].color_shot2);
                break;
        }

        if (shot)
            shot.GetComponent<PoolObject>().ReturnToPool(lifetime);
        audio_out.pitch = 1 + Random.Range(-Options[item].picthDown, Options[item].picthUp);
        audio_out.Play();
        nextFire = Time.time + Options[item].fireDelay * level_rate;

    }

    public void LevelUp(bool firerate, bool damage, bool accuracy, bool shotspecial)
    {
        if (firerate)
            level_rate = level_rate * 0.8f + 0.06f;

        if (damage)
            level_damage += 0.1f;

        if (accuracy)
            level_accuracy = level_accuracy * 0.8f + 0.06f;

        if (shotspecial)
            level_shot = level_shot * 0.9f + 0.3f;
    }

}
