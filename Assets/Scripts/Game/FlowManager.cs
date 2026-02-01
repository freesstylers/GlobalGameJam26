using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class FlowManager : MonoBehaviour
{
    [Serializable]
    public enum enemyType { dolphin, skibido, illumiboy, concha }


    public enum State { Cooldown, Spawn, Round, Improvement, EndGame };
    public int[] timers = { 10, 5, 60, -1, -1 };
    public static FlowManager instance;

    public EnemyPoolManager spawnerManager;

    public Material[] enemyFilters_;
    private int fadeOutMaterialIndex = -1;
    private int fadeInMaterialIndex = -1;

    //Mask
    public List<Mask> masks_;
    private Mask currentMask_;
    private int currentMaskId_ = 0;
    private Action<Mask> onMaskChange;

    [SerializeField]
    Camera redCamera;

    [SerializeField]
    Camera blueCamera;

    [SerializeField]
    Camera yellowCamera;

    private FMOD.Studio.EventInstance musicInstance_;
    private FMOD.Studio.EventInstance giantMaskInstance_;
    [SerializeField]
    Material screenMaterial;

    public State currentState
    { 
        get { 
            return currentState_;
        }
        set {
            currentState_ = value;

            onStateChanged();
        }
    }
    [SerializeField]
    private State currentState_;

    float timerValue = 0.0f;
    public int currentRound;

    public static event Action<State> onStateChange;

    [HideInInspector]
    public PlayerMovement currentPlayer;

    public GameObject giantMask;

    public int pointsInterface
    {
        get
        {
            return currentPoints_;
        }
        set
        {
            currentPoints_ = value;

            onPointsChanged();
        }
    }

    private int currentPoints_ = 0;

    public int currentAliveEnemies = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            ResetMaterialVariables();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPoints_ = 0;
        currentAliveEnemies = 0;
        currentState = State.Cooldown;

        spawnerManager = GameObject.FindAnyObjectByType<EnemyPoolManager>();

        foreach(Mask m in masks_)
        {
            m.stats_.ResetStats();
            m.stats_.UpdateStats();
        }

        musicInstance_ = FMODUnity.RuntimeManager.CreateInstance("event:/GameMusic");
        giantMaskInstance_ = FMODUnity.RuntimeManager.CreateInstance("event:/GiantMask");
        musicInstance_.start();

        currentMask_ = masks_[0];
    }

    private void onStateChanged()
    {
    }

    public TextMeshProUGUI points;

    private void onPointsChanged()
    {
        points.text = currentPoints_.ToString();
    }

    public Slider HP;
    public TextMeshProUGUI ammo;

    public void UpdateHP(float current, float max)
    {
        HP.value = current / max;
    }

    public void UpdateAmmo(int amt)
    {
        ammo.text = amt.ToString();
    }

    public float slowTimeScale = 0.1f;

    public void SlowDown(bool state)
    {
        Time.timeScale = state ? slowTimeScale : 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHP(GetCurrentMask().stats_.playerHP_, GetCurrentMask().stats_.maxPlayerHP_);

        if (GetCurrentMask().stats_.playerHP_ <= 0)
        {
            setState(State.EndGame);
            EndGameUI.SetActive(true);
            Time.timeScale = 0.0f;
            //Do stuff
        }

        if (currentState == State.Cooldown)
        {
            if (timerValue < timers[(int)currentState])
            {
                timerValue += Time.deltaTime;
            }
            else
            {
                advanceState();
                timerValue = 0.0f;
            }
        }
#if UNITY_EDITOR
        if (currentState == State.Improvement)
            advanceState();
#endif
    }

    public void setState (State state)
    {
        currentState = state;
    }

    public State GetState()
    {
        return currentState; 
    }

    public void GoBackToMainMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("mainMenu");
    }

    public void advanceState()
    {
        switch (currentState)
        {
            case State.Cooldown:
                setState(State.Spawn);
                FlowManager.instance.currentPlayer.SetPlayerLook(true);
                FlowManager.instance.currentPlayer.SetPlayerCanInteract(true);
                Cursor.lockState = CursorLockMode.Locked;

                spawnerManager.onRoundChange(currentRound);

                break;
            case State.Spawn:
                setState(State.Round);
                break;
            case State.Round:
                musicInstance_.setParameterByName("GameState", 0.0f);
                if (currentAliveEnemies == 0)
                {
                    currentRound += 1;
                    setState(State.Improvement);
                }
                break;
            case State.Improvement:
                SHopUI.SetActive(true);
                giantMask.GetComponent<Animator>().SetTrigger("NewRound");
                giantMaskInstance_.start();
                FlowManager.instance.currentPlayer.SetPlayerLook(false);
                FlowManager.instance.currentPlayer.SetPlayerCanInteract(false);
                Cursor.lockState = CursorLockMode.None;
                musicInstance_.setParameterByName("GameState", 1.0f);
                break;
        }
    }

    public int greenEnemies;
    public int redEnemies;
    public int blueEnemies;

    public TextMeshProUGUI redEnemies_;
    public TextMeshProUGUI greenEnemies_;
    public TextMeshProUGUI blueEnemies_;

    public void UpdateEnemyCount()
    {
        redEnemies_.text = redEnemies.ToString();
        greenEnemies_.text = greenEnemies.ToString();
        blueEnemies_.text = blueEnemies.ToString();
    }

    public void NextRound()
    {
        setState(State.Cooldown);
        SHopUI.SetActive(false);
        musicInstance_.setParameterByName("GameState", 0.0f);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("mainMenu");
    }

    public GameObject PauseMenu;
    public RadialMenuHandler RadialMenuHandler;

    public float mouseSpeedMod = 1.0f;

    public void Pause(bool state)
    {
        if (RadialMenuHandler == null || RadialMenuHandler.radialOn)
            return;

        Time.timeScale = state ? 0.0f : 1.0f;
        PauseMenu.SetActive(state);
    }

    public void SetMouseSpeedMod(float mod)
        { mouseSpeedMod = mod; }

    public List<Animator> Masks;

    public void SetMusicToTension()
    {
        musicInstance_.setParameterByName("LowHealth", 1.0f);
    }

    public void UpdateMasks()
    {
        foreach (var m in masks_)
        {
            m.stats_.UpdateStats();
        }
    }

    public Material blendMaterial;
    public bool blending = false;

    public IEnumerator setBlend()
    {
        blending = true;
        float f = 0.15f;

        while (f > 0.02f)
        {
            f -= (Time.deltaTime * 0.5f);
            blendMaterial.SetFloat("_Blend", f);
            yield return null;
        }

        f = 0.0f;
        blendMaterial.SetFloat("_Blend", 0.0f);
        blending = false;
    }

    #region MASK
    public Mask GetCurrentMask()
    {
        return masks_[currentMaskId_];
    }

    public void SuscribeMaskChange(Action<Mask> action)
    {
        onMaskChange += action;
    }

    public void SetMask(int maskId)
    {
        if (maskId < masks_.Count)
        {
            fadeOutMaterialIndex = currentMaskId_;
            fadeInMaterialIndex = maskId;
            //enemyFilters_[fadeOutMaterialIndex].SetFloat("_opacity", 0);
            //enemyFilters_[fadeInMaterialIndex].SetFloat("_opacity", 1);
            
            String[] offsetNames = { "_mask1YOffset", "_mask2YOffset", "_mask3YOffset" }; //_mask1VisualObstructionStremgth
            String[] obstructionNames = { "_mask1VisualObstructionStrength", "_mask2VisualObstructionStrength", "_mask3VisualObstructionStrength" }; //_mask1VisualObstructionStremgth
            float overallSPeed = .25f;
            //Te quitas la mascara y se vuelve todo negro
            StartCoroutine(LerpFloat(value => screenMaterial.SetFloat(offsetNames[fadeOutMaterialIndex], value), screenMaterial.GetFloat(offsetNames[fadeOutMaterialIndex]), 1, overallSPeed, 0));
            StartCoroutine(LerpFloat(value => screenMaterial.SetFloat(offsetNames[fadeInMaterialIndex], value), screenMaterial.GetFloat(offsetNames[fadeInMaterialIndex]), 1, overallSPeed, 0));
            StartCoroutine(LerpFloat(value => screenMaterial.SetFloat(obstructionNames[fadeInMaterialIndex], value), screenMaterial.GetFloat(obstructionNames[fadeInMaterialIndex]), 1.0f, overallSPeed, 0));
            //Cambia la mascara "logicamente"
            StartCoroutine(ExecuteAfterDelay(() =>
            {
                currentMaskId_ = maskId;
                musicInstance_.setParameterByName("Color", currentMaskId_);

                if (onMaskChange != null)
                {
                    onMaskChange.Invoke(GetCurrentMask());
                }
                switch (currentMaskId_)
                {
                    case 0:
                        redCamera.gameObject.SetActive(true);
                        blueCamera.gameObject.SetActive(false);
                        yellowCamera.gameObject.SetActive(false);
                        break;
                    case 1:
                        redCamera.gameObject.SetActive(false);
                        blueCamera.gameObject.SetActive(false);
                        yellowCamera.gameObject.SetActive(true);
                        break;
                    case 2:
                        redCamera.gameObject.SetActive(false);
                        blueCamera.gameObject.SetActive(true);
                        yellowCamera.gameObject.SetActive(false);
                        break;
                    case -1:
                        redCamera.gameObject.SetActive(false);
                        blueCamera.gameObject.SetActive(false);
                        yellowCamera.gameObject.SetActive(false);
                        break;
                }
            }, overallSPeed));
            //Ponte la mascara que toca y baja la obstruccion 0
            StartCoroutine(LerpFloat(value => screenMaterial.SetFloat(obstructionNames[fadeOutMaterialIndex], value), screenMaterial.GetFloat(obstructionNames[fadeOutMaterialIndex]), 0, overallSPeed, overallSPeed));
            StartCoroutine(LerpFloat(value => screenMaterial.SetFloat(offsetNames[fadeInMaterialIndex], value), screenMaterial.GetFloat(offsetNames[fadeInMaterialIndex]), 0, overallSPeed, overallSPeed));
            StartCoroutine(LerpFloat(value => screenMaterial.SetFloat(obstructionNames[fadeInMaterialIndex], value), screenMaterial.GetFloat(obstructionNames[fadeInMaterialIndex]), 0.0f, overallSPeed, overallSPeed*2));

        }
    }

    public void NextMask()
    {
        SetMask((currentMaskId_ + 1) % masks_.Count);
    }

    public void PrevMask()
    {
        if(currentMaskId_ == 0)
            SetMask(masks_.Count - 1);
        else 
            SetMask(currentMaskId_ - 1);
    }

    private IEnumerator LerpFloat(System.Action<float> onValueChanged, float startValue, float targetValue, float duration, float delay = 0f)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            onValueChanged?.Invoke(Mathf.Lerp(startValue, targetValue, t));
            yield return null;
        }

        onValueChanged?.Invoke(targetValue);
    }

    private IEnumerator ExecuteAfterDelay(System.Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    public void ResetMaterialVariables()
    {
        Debug.Log("Reset Material Variables");
        String[] offsetNames = { "_mask1YOffset", "_mask2YOffset", "_mask3YOffset" };
        String[] obstructionNames = { "_mask1VisualObstructionStrength", "_mask2VisualObstructionStrength", "_mask3VisualObstructionStrength" };
        foreach (string offsetName in offsetNames)
            screenMaterial.SetFloat(offsetName, 0f);
        foreach (string obstructionName in obstructionNames)
            screenMaterial.SetFloat(obstructionName, 0f);
    }

    public GameObject SHopUI;
    public GameObject EndGameUI;

    #endregion
}
