using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Loot : MonoBehaviour {

    public List<AudioClip> beep = new List<AudioClip>();
    public List<Sprite> look = new List<Sprite>();
    public List<Color> colors = new List<Color>();

    public float Disappearing_speed = 1f;
    public float Rotation_speed = 30f;
    public string LootText = "LootText";

    private Text Text_out;
    private AudioSource audio_out;
    private SpriteRenderer look_out;

    private Vector3 rot;
    private bool end;
    private bonus loot;

    public enum bonus { repair, fire_rate, shot_damage, fire_accuracy, shot_speed, shot_mass };

    void Awake()
    {
        audio_out = GetComponent<AudioSource>();
        look_out = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        loot = (bonus)Random.Range(0, 6);
        audio_out.clip = beep[(int)loot];
        look_out.sprite = look[(int)loot];
        look_out.color = colors[(int)loot];
        end = false;
    }

    void Update()
    {
        if (end)
            look_out.color -= new Color (0f,0f,0f, Disappearing_speed) * Time.deltaTime;

        rot.Set(0, 0, Rotation_speed * Time.deltaTime);
        transform.Rotate(rot);

    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.tag=="Player") && (!end))
        {
            Text_out = PoolManager.GetObject(LootText, Camera.main.WorldToScreenPoint(transform.position), Quaternion.identity).GetComponent<Text>();

            switch (loot)
            {
                case bonus.repair:
                    other.GetComponent<Destructible>().Repair(4);
                    Text_out.text = "repair";
                    break;
                case bonus.fire_rate:
                    other.GetComponent<Player>().LevelUp(false, true, false, false, false);
                    Text_out.text = "+ fire rate";
                    break;
                case bonus.shot_damage:
                    other.GetComponent<Player>().LevelUp(false, false, true, false, false);
                    Text_out.text = "+ damage";
                    break;
                case bonus.fire_accuracy:
                    other.GetComponent<Player>().LevelUp(false, false, false, true, false);
                    Text_out.text = "+ accuracy";
                    break;
                case bonus.shot_speed:
                    other.GetComponent<Player>().LevelUp(false, false, false, false, true);
                    Text_out.text = "+ weapon level";
                    break;
                case bonus.shot_mass:
                    other.GetComponent<Player>().LevelUp(true, false, false, false, false);
                    Text_out.text = "+ speed";
                    break;
            }

            end = true;
            audio_out.Play();

            if (GetComponent<PoolObject>())
                GetComponent<PoolObject>().ReturnToPool(1/ Disappearing_speed);
            else
                Destroy(gameObject, 1/ Disappearing_speed);
        }
    }
}
