using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuScript : MonoBehaviour
{
    [SerializeField]
    GameObject quit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
#if UNITY_STANDALONE
        quit.SetActive(true);
#else
        quit.SetActive(false);
#endif
    }

    public void Play()
    {
        SceneManager.LoadScene("gameScreen");
    }

    [SerializeField]
    GameObject settings;

    public void ToggleSettings()
    {
        settings.SetActive(!settings.activeSelf);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public IEnumerator maskAnimation()
    {
        yield return null;
    }
}
