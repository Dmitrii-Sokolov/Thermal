using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


public class MainMenu : MonoBehaviour {

    public GameObject credits;
    public GameObject options;
    public GameObject game;
    public AudioMixer Mixer;

    void Start()
    {
        credits.SetActive(false);
        options.SetActive(false);
        game.SetActive(false);

        Mixer.SetFloat("Master", PlayerPrefs.GetFloat("VolumeLevel") * PlayerPrefs.GetInt("VolumeOn", 1) + 80* (PlayerPrefs.GetInt("VolumeOn", 1)-1));
        Mixer.SetFloat("Sound", PlayerPrefs.GetFloat("SoundLevel") * PlayerPrefs.GetInt("SoundOn", 1) + 80 * (PlayerPrefs.GetInt("SoundOn", 1) - 1));
        Mixer.SetFloat("Music", PlayerPrefs.GetFloat("MusicLevel") * PlayerPrefs.GetInt("MusicOn", 1) + 80 * (PlayerPrefs.GetInt("MusicOn", 1) - 1));
    }

    public void click()
    {
        GetComponent<AudioSource>().Play();
    }

    public void main_newgame_button()
    {
        credits.SetActive(false);
        options.SetActive(false);
        game.SetActive(!game.activeSelf);
    }

    public void main_newgame_difficult_button(int level)
    {
        PlayerPrefs.SetInt("Difficulty", level);
        SceneManager.LoadScene("First", LoadSceneMode.Single);
    }

    public void main_options_button()
    {
        game.SetActive(false);
        credits.SetActive(false);
        options.SetActive(!options.activeSelf);
    }

    public void main_credits_button()
    {
        game.SetActive(false);
        options.SetActive(false);
        credits.SetActive(!credits.activeSelf);
    }

    public void main_exit_button()
    {
        Application.Quit();
    }
}
