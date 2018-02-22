using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Audio;


public class UIController : MonoBehaviour {

    public Text ScoreText;
    public Text WaveText;
    public Text EndGameText;
    public Text Timer;

    //public string LootText = "LootText";
    public GameObject PauseMenu;
    public AudioMixer Mixer;

    public List<AudioClip> music = new List<AudioClip>();
    public List<GameObject> ships = new List<GameObject>();

    //public GameObject LootText_object;
    private AudioSource audio_out;
    private int music_current = 0;

    private float start_time = 0;
    private float score = 0f;
    private GameObject player;
    private bool end = false;

    void OnEnable () {
        score = 0f;
        ScoreUp(0f);
        PauseMenu.SetActive(false);

        audio_out = GetComponent<AudioSource>();
        Time.timeScale = 1;

        start_time = Time.time;
    }

    void Start()
    {
        Mixer.SetFloat("Master", PlayerPrefs.GetFloat("VolumeLevel") * PlayerPrefs.GetInt("VolumeOn", 1) + 80 * (PlayerPrefs.GetInt("VolumeOn", 1) - 1));
        Mixer.SetFloat("Sound", PlayerPrefs.GetFloat("SoundLevel") * PlayerPrefs.GetInt("SoundOn", 1) + 80 * (PlayerPrefs.GetInt("SoundOn", 1) - 1));
        Mixer.SetFloat("Music", PlayerPrefs.GetFloat("MusicLevel") * PlayerPrefs.GetInt("MusicOn", 1) + 80 * (PlayerPrefs.GetInt("MusicOn", 1) - 1));
        Instantiate(ships[PlayerPrefs.GetInt("Ship", 1)]);
    }

    void Update()
    {

        if (!audio_out.isPlaying)
        {
            music_current = Random.Range(0, music.Count);
            audio_out.clip = music[music_current];
            audio_out.Play();
        }

        switch (end)
        {
            case false:
                if (Input.GetButtonDown("Pause"))
                    PauseMenu.SetActive(!PauseMenu.activeSelf);
                if (Timer)
                {
                    Timer.text = "TIME: " + (Time.time - start_time).ToString("##.0");
                }

                break;
            case true:
                if (Input.GetButtonDown("Restart"))
                    SceneManager.LoadScene("First", LoadSceneMode.Single);
                if (Input.GetButtonDown("Pause"))
                    SceneManager.LoadScene("Main", LoadSceneMode.Single);
                break;
        }
    }

    public void EndGame()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player)
            player.GetComponent<Destructible>().Destruct();
        EndGameText.gameObject.SetActive(true);
        end = true;
    }

    public void SetWave(int wave)
    {
        if (WaveText)
            WaveText.text = "WAVE: " + wave.ToString();
    }
	
    public void ScoreUp(float plus)
    {
        score += plus;
        ScoreText.text = "SCORE: " + score.ToString();
    }
}

