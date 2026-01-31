using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private State currentState_;

    float timerValue = 0.0f;
    public int currentRound;

    public static event Action<State> onStateChange;

    [HideInInspector]
    public PlayerMovement currentPlayer;

    public int currentPoints = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
        currentPoints = 0;
        currentState = State.Cooldown;

        spawnerManager = GameObject.FindAnyObjectByType<EnemyPoolManager>();
    }

    private void onStateChanged()
    {
    }

    public float slowTimeScale = 0.1f;

    public void SlowDown(bool state)
    {
        Time.timeScale = state ? slowTimeScale : 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == State.Cooldown || currentState == State.Spawn || currentState == State.Round)
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

    public void advanceState()
    {
        switch (currentState)
        {
            case State.Cooldown:
                setState(State.Spawn);

                spawnerManager.onRoundChange(currentRound);

                break;
            case State.Spawn:
                setState(State.Round);
                break;
            case State.Round:
                if (true)
                {
                    setState(State.Improvement);
                }
                else
                {
                    setState(State.EndGame);
                }
                break;
            case State.Improvement:
                setState(State.Cooldown);
                break;
            //case State.EndGame:
            //    GoToMenu();
            //    break;
        }

        Debug.LogError("Changing State: " + currentState);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("mainMenu");
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
        Debug.Log("PUTA " + maskId + " COUNT: " + masks_.Count);
        if (maskId < masks_.Count)
        {
            Debug.Log("COÑO " + maskId);
            fadeOutMaterialIndex = currentMaskId_;
            fadeInMaterialIndex = maskId;
            Debug.Log("Changing Mask from " + currentMaskId_ + " to " + maskId);
            //enemyFilters_[fadeOutMaterialIndex].SetFloat("_opacity", 0);
            //enemyFilters_[fadeInMaterialIndex].SetFloat("_opacity", 1);
            StartCoroutine(LerpFloat(value => enemyFilters_[fadeOutMaterialIndex].SetFloat("_opacity", value), 1, 0, .5f));
            StartCoroutine(LerpFloat(value => enemyFilters_[fadeInMaterialIndex].SetFloat("_opacity", value), 0, 1, .5f));

            currentMaskId_ = maskId;

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
        }
    }

    public void NextMask()
    {
        Debug.Log("Next Mask");
        SetMask((currentMaskId_ + 1) % masks_.Count);
    }

    public void PrevMask()
    {
        if(currentMaskId_ == 0)
            SetMask(masks_.Count - 1);
        else 
            SetMask(currentMaskId_ - 1);
    }

    private IEnumerator LerpFloat(System.Action<float> onValueChanged,float startValue, float targetValue, float duration)
    {
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

    #endregion
}
