using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMan : MonoBehaviour
{
    public Slider slider;

    private void Awake()
    {
        slider.value = FlowManager.instance.mouseSpeedMod * 10;
    }

    public void UpdatedSlider(float value)
    {
        FlowManager.instance.SetMouseSpeedMod(value / 10);
    }

    public void UnPause()
    {
        FlowManager.instance.Pause(false);
    }

    public void Quit()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("mainMenu");
    }
}
