using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    public float speed_rotary = 300f; // 100 ... 500
    public float speed_forward = 7f; // 3.0 ... 10.0
    public float VelocityRate = 0.1f; // 0.01 ... 1.0

    public List<string> targets;
    public List<Vector3> Gun_points = new List<Vector3>();
    public GameObject Gun_prefab;
    private GameObject Gun_new;
    private List<Gun> Guns = new List<Gun>();
    private Vector2 target;

    private float move_vertical = 0f;
    private float move_horizontal = 0f;
    private float speed_boost = 1f;

    private float AngularDamp;
    private Vector2 goal_velocity;
    private float goal_rotation;
    private Rigidbody2D body;

    void Awake () {
        body = gameObject.GetComponent<Rigidbody2D>();
        AngularDamp = speed_rotary*Time.fixedDeltaTime;

        for (int i = 0; i < Gun_points.Count; i++)
        {
            Gun_new = (GameObject) Instantiate(Gun_prefab,transform.position + Gun_points[i], Quaternion.identity);
            Gun_new.transform.SetParent(transform);
            Guns.Add(Gun_new.GetComponent<Gun>());
            Guns[i].Init(targets, PlayerPrefs.GetInt("Gun", 0));
        }
    }

    void FixedUpdate() {
        move_vertical = Input.GetAxis("Vertical");
        move_horizontal = Input.GetAxis("Horizontal"); 
        target = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        goal_velocity.Set(move_horizontal, move_vertical);
        if ((move_vertical != 0) || (move_horizontal != 0))
        {
            goal_velocity *= speed_forward* speed_boost / Mathf.Sqrt(move_vertical * move_vertical + move_horizontal * move_horizontal);
            if (goal_velocity.x < 0)
                goal_rotation = Vector2.Angle(Vector2.up, goal_velocity);
            else
                goal_rotation = 360 - Vector2.Angle(Vector2.up, goal_velocity);
            body.angularVelocity = speed_rotary * Mathf.Clamp(Mathf.DeltaAngle(body.rotation, goal_rotation) / AngularDamp, -1, 1);
        }
        else
            body.angularVelocity = 0f;

        body.linearVelocity += (goal_velocity - body.linearVelocity) * VelocityRate;

        for (int i = 0; i < Guns.Count; i++)
        {
            Guns[i].SetFire(Input.GetButton("Fire0"));
            Guns[i].Aim(target);
        }

    }

    public void LevelUp(bool speed, bool firerate, bool damage, bool accuracy, bool shotspecial)
    {
        if (speed)
            speed_boost = 0.9f * speed_boost + 0.25f;

        for (int i = 0; i < Guns.Count; i++)
        {
            Guns[i].LevelUp(firerate, damage, accuracy, shotspecial);
        }
    }

}
