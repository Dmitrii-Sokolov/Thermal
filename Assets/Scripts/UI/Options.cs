using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;


public class Options : MonoBehaviour {

    public Dropdown resolution_dropdown;
    public Toggle fullscreen_check;
    public GameObject yes_no_panel;
    public Text yes_no_panel_timer;

    public Toggle volume_toggle;
    public Slider volume_slider;
    public Toggle sound_toggle;
    public Slider sound_slider;
    public Toggle music_toggle;
    public Slider music_slider;
    public AudioMixer Mixer;

    private float return_time;
    private int resolution_current = 0;
    private int resolution_last = -1;
    private Resolution[] resolutions;
    private List<string> resolutions_names = new List<string>();

    void Awake () {
        fullscreen_check.isOn = Screen.fullScreen;

        if (resolution_dropdown)
        {
            resolutions = new Resolution[Screen.resolutions.Length];

            for (int i = 0; i < resolutions.Length; i++)
            {
                resolutions[i] = Screen.resolutions[resolutions.Length - i - 1];
                resolutions_names.Add(resolutions[i].width.ToString() + "x" + resolutions[i].height.ToString());
                if ((resolutions[i].width == Screen.width) && (resolutions[i].height == Screen.height))
                    resolution_current = i;
            }

            resolution_dropdown.ClearOptions();
            resolution_dropdown.AddOptions(resolutions_names);
            resolution_dropdown.value = resolution_current;
            resolution_last = 0;
        }

        volume_toggle.isOn = (PlayerPrefs.GetInt("VolumeOn", 1) == 1);
        sound_toggle.isOn = (PlayerPrefs.GetInt("SoundOn", 1) == 1);
        music_toggle.isOn = (PlayerPrefs.GetInt("MusicOn", 1) == 1);
        volume_slider.value = Mathf.Pow(10, (PlayerPrefs.GetFloat("VolumeLevel", 0f) + 80) / 40);
        sound_slider.value = Mathf.Pow(10, (PlayerPrefs.GetFloat("SoundLevel", 0f) + 80) / 40);
        music_slider.value = Mathf.Pow(10, (PlayerPrefs.GetFloat("MusicLevel", 0f) + 80) / 40);
    }

    void Update()
    {
        if (yes_no_panel)
            if (yes_no_panel.activeSelf)
            {
                if (Time.time > return_time)
                    options_resolution_dialog(false);
                yes_no_panel_timer.text = (return_time - Time.time).ToString("##.0");
            }
    }

    public void options_resolution(int value)
    {
        if (resolution_last != -1)
        {
            resolution_last = resolution_current;
            resolution_current = value;
            Screen.SetResolution(resolutions[value].width, resolutions[value].height, Screen.fullScreen);
            return_time = Time.time + 15f;
            yes_no_panel.SetActive(true);
        }
    }

    public void options_fullscreen(bool isOn)
    {
        Screen.fullScreen = isOn;
    }

    public void options_resolution_dialog(bool yes)
    {
        if (!yes)
        {
            resolution_current = resolution_last;
            resolution_dropdown.value = resolution_current;
        }
        yes_no_panel.SetActive(false);
    }

    public void options_volume_toggle(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetInt("VolumeOn", 1);
            Mixer.SetFloat("Master", PlayerPrefs.GetFloat("VolumeLevel"));
        }
        else
        {
            PlayerPrefs.SetInt("VolumeOn", 0);
            Mixer.SetFloat("Master", -80f);
        }
    }

    public void options_volume_slider(float value)
    {
        value = 40 * Mathf.Log10(value) - 80;
        PlayerPrefs.SetFloat("VolumeLevel", value);
        Mixer.SetFloat("Master", value * PlayerPrefs.GetInt("VolumeOn", 1) + 80 * (PlayerPrefs.GetInt("VolumeOn", 1) - 1));
    }

    public void options_sound_toggle(bool isOn)
    {
        if (isOn)
        {
            Mixer.SetFloat("Sound", PlayerPrefs.GetFloat("SoundLevel"));
            PlayerPrefs.SetInt("SoundOn", 1);
        }
        else
        {
            PlayerPrefs.SetInt("SoundOn", 0);
            Mixer.SetFloat("Sound", -80f);
        }
    }

    public void options_sound_slider(float value)
    {
        value = 40 * Mathf.Log10(value) - 80;
        PlayerPrefs.SetFloat("SoundLevel", value);
        Mixer.SetFloat("Sound", value * PlayerPrefs.GetInt("SoundOn", 1) + 80 * (PlayerPrefs.GetInt("SoundOn", 1) - 1));
    }


    public void options_music_toggle(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetInt("MusicOn", 1);
            Mixer.SetFloat("Music", PlayerPrefs.GetFloat("MusicLevel"));
        }
        else
        {
            PlayerPrefs.SetInt("MusicOn", 0);
            Mixer.SetFloat("Music", -80f);
        }
    }

    public void options_music_slider(float value)
    {
        value = 40 * Mathf.Log10(value) - 80;
        PlayerPrefs.SetFloat("MusicLevel", value);
        Mixer.SetFloat("Music", value * PlayerPrefs.GetInt("MusicOn", 1) + 80 * (PlayerPrefs.GetInt("MusicOn", 1) - 1));
    }

}
