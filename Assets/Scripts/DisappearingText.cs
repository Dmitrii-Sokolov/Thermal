using UnityEngine;
using UnityEngine.UI;

public class DisappearingText : MonoBehaviour
{

    public float Delay = 2f;
    public float Disappearing_speed = 2f;

    private float endtime;
    private float endtime2;
    private Text Text_out;

    private void Awake()
    {
        Text_out = GetComponent<Text>();
    }

    private void OnEnable()
    {
        transform.SetParent(FindFirstObjectByType<Canvas>().transform);
        transform.localScale = Vector3.one;

        Text_out.color = new Color(Text_out.color[0], Text_out.color[1], Text_out.color[2], 1);
        endtime = Time.time + Delay;
        endtime2 = Time.time + Delay + 1 / Disappearing_speed;

    }

    private void Update()
    {
        if (Time.time > endtime)
        {
            Text_out.color -= new Color(0f, 0f, 0f, Disappearing_speed) * Time.deltaTime;
        }

        if (Time.time > endtime2)
        {
            if (GetComponent<PoolObject>())
                GetComponent<PoolObject>().ReturnToPool();
            else
                gameObject.SetActive(false);
        }

    }
}
