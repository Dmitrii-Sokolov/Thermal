using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{

    public float width = 80f;
    public float height = 30f;
    public float offset = 5f;
    public Slider WaveBar;

    public List<WaveMix> waves;
    public float[] cycle_mod = new float[3];
    public float[] max_enemy_mod = new float[3];

    public string fieldedge = "Edge";
    public string fieldcenter = "Center";
    public int EnemyCount = 0;

    private List<BoxCollider2D> box = new();

    private int difficulty = 1;
    private int wave_count = 0;
    private int wave = 0;
    private float spawntime = 2f;
    private float frequencySumPerWave;
    private float randomPoint;
    private string enemy_current;
    private Vector2 enemy_pos;
    private Quaternion enemy_rot = Quaternion.identity;
    private GameObject player;
    //private GameObject enemy_spawn;

    [System.Serializable]
    public struct EnemyDanger
    {
        public string name;
        public int frequency;
    }

    [System.Serializable]
    public struct WaveMix
    {
        public int count;
        public float cycle;
        public List<EnemyDanger> mix;
    }

    private void Awake()
    {
        difficulty = PlayerPrefs.GetInt("Difficulty", 1);
    }

    // Use this for initialization
    private void Start()
    {
        box.AddRange(GameObject.FindGameObjectWithTag(fieldedge).GetComponents<BoxCollider2D>());
        height = FindFirstObjectByType<Camera>().orthographicSize * 2;
        width = FindFirstObjectByType<Camera>().aspect * height;

        enemy_pos.Set(0, height / 2 + offset);
        box[0].offset = enemy_pos;
        enemy_pos.Set(width, offset * 2);
        box[0].size = enemy_pos;

        enemy_pos.Set(0, -height / 2 - offset);
        box[1].offset = enemy_pos;
        enemy_pos.Set(width, offset * 2);
        box[1].size = enemy_pos;

        enemy_pos.Set(width / 2 + offset, 0);
        box[2].offset = enemy_pos;
        enemy_pos.Set(offset * 2, height);
        box[2].size = enemy_pos;

        enemy_pos.Set(-width / 2 - offset, 0);
        box[3].offset = enemy_pos;
        enemy_pos.Set(offset * 2, height);
        box[3].size = enemy_pos;

        box.Clear();
        box.AddRange(GameObject.FindGameObjectWithTag(fieldcenter).GetComponents<BoxCollider2D>());
        enemy_pos.Set(0, 0);
        box[0].offset = enemy_pos;
        enemy_pos.Set(width + 4 * offset, height + 4 * offset);
        box[0].size = enemy_pos;

        FindFirstObjectByType<UIController>().SetWave(wave);
    }

    // Update is called once per frame
    private void Update()
    {
        if (WaveBar)
        {
            if (EnemyCount < max_enemy_mod[difficulty])
                WaveBar.value = 1000 * (wave_count + Mathf.Clamp((-spawntime + Time.time) / (waves[wave].cycle * cycle_mod[difficulty]), -1, 0)) / waves[wave].count;
            else
                WaveBar.value = 1000 * (wave_count - 1) / waves[wave].count;
        }

        if (!player)
            player = GameObject.FindGameObjectWithTag("Player");
    }


    private void FixedUpdate()
    {
        if (Time.time > spawntime && EnemyCount < max_enemy_mod[difficulty])
        {
            if (wave_count < waves[wave].count)
            {
                if (player)
                {
                    Spawn(waves[wave].mix);
                    wave_count++;
                    spawntime = Time.time + waves[wave].cycle * cycle_mod[difficulty];
                }
            }
            else
            {
                if (waves.Count - 1 == wave)
                {
                    waves.Add(waves[waves.Count - 1]);
                    cycle_mod[difficulty] = cycle_mod[difficulty] * 0.95f + 0.02f;
                    //cycle_mod[difficulty] = cycle_mod[difficulty] * 0.98f + 0.008f;
                }
                wave++;

                FindFirstObjectByType<UIController>().SetWave(wave);

                wave_count = 0;
            }
        }
    }

    private void Spawn(List<EnemyDanger> mix_current)
    {
        enemy_current = ChooseEnemy(mix_current);

        if (Random.value > 0.5)
        //if (true)

        {
            enemy_pos.Set(-width / 2 - offset, Random.value * (height - 2 * offset) - height / 2 + offset);
            enemy_rot = Quaternion.Euler(0, 0, 270);
        }
        else
        {
            enemy_pos.Set(width / 2 + offset, Random.value * (height - 2 * offset) - height / 2 + offset);
            enemy_rot = Quaternion.Euler(0, 0, 90);
        }
        /*else
            if (Random.value > 0.5)
            {
                enemy_pos.Set(Random.value * (width-2* offset) - width / 2 + offset, -height / 2 - offset);
                enemy_rot = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                enemy_pos.Set(Random.value * (width - 2 * offset) - width / 2 + offset, height / 2 + offset);
                enemy_rot = Quaternion.Euler(0, 0, 180);
            }*/

        PoolManager.GetObject(enemy_current, enemy_pos, enemy_rot);
    }

    private string ChooseEnemy(List<EnemyDanger> mix_choose)
    {
        frequencySumPerWave = 0;

        for (var i = 0; i < mix_choose.Count; i++)
        {
            frequencySumPerWave += mix_choose[i].frequency;
        }

        randomPoint = Random.value * frequencySumPerWave;

        for (var i = 0; i < mix_choose.Count; i++)
        {
            if (randomPoint < mix_choose[i].frequency)
            {
                return mix_choose[i].name;
            }
            else
            {
                randomPoint -= mix_choose[i].frequency;
            }
        }
        return mix_choose[mix_choose.Count].name;

    }

}
