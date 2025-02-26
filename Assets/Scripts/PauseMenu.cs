using UnityEngine;

public class PauseMenu : MonoBehaviour
{


    public GameObject Panel;
    public GameObject options;
    public GameObject HelpPanel;
    public GameObject Surrender;
    public GameObject Quit;

    private void OnEnable()
    {
        Time.timeScale = 0;
    }

    public void pause_click()
    {
        gameObject.GetComponent<AudioSource>().Play();
    }

    public void pause_howtoplay()
    {
        HelpPanel.SetActive(!HelpPanel.activeSelf);
    }

    public void pause_return_button()
    {
        Panel.SetActive(!Panel.activeSelf);
    }

    public void pause_options_button()
    {
        options.SetActive(!options.activeSelf);
    }

    public void pause_surrender_button()
    {
        FindFirstObjectByType<UIController>().EndGame();
        Panel.SetActive(!Panel.activeSelf);
    }

    public void pause_exit_button()
    {
        FindFirstObjectByType<UIController>().EndGame();
        Panel.SetActive(!Panel.activeSelf);
        Application.Quit();
    }

    public void pause_surrender_panel()
    {
        Quit.SetActive(false);
        Surrender.SetActive(!Surrender.activeSelf);
    }
    public void pause_exit_panel()
    {
        Surrender.SetActive(false);
        Quit.SetActive(!Quit.activeSelf);
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
        Surrender.SetActive(false);
        Quit.SetActive(false);
        HelpPanel.SetActive(false);
        options.SetActive(false);
    }
}
