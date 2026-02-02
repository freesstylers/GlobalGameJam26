using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenuScript : MonoBehaviour
{
    [SerializeField]
    GameObject quit;
    private FMOD.Studio.EventInstance musicInstance_;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Time.timeScale = 1.0f;
//#if UNITY_STANDALONE
//        quit.SetActive(true);
//#else
//        quit.SetActive(false);
//#endif
    }

    private void Start()
    {
        musicInstance_ = FMODUnity.RuntimeManager.CreateInstance("event:/GameMusic");
        musicInstance_.setParameterByName("GameState", 2);
        musicInstance_.setParameterByName("LowHealth", 1f);
        musicInstance_.start();
    }

    private void OnDestroy()
    {
        musicInstance_.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicInstance_.release();
    }

    public void Play()
    {
        gameObject.SetActive(false);    
        SceneManager.LoadScene("gameScreen");
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
