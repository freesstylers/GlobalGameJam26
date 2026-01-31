using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class FlowManager : MonoBehaviour
{
    public enum enemyType { dolphin, skibido }


    public enum State { Cooldown, Spawn, Round, Improvement, EndGame };
    public int[] timers = { 10, 5, 60, -1, -1 };
    public static FlowManager instance;

    public EnemyPoolManager spawnerManager;

    public Material[] enemyFilters_;
    //Mask
    public List<Mask> masks_;
    private Mask currentMask_;
    private int currentMaskId_ = 0;
    private Action<Mask> onMaskChange;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = State.Cooldown;

        spawnerManager = GameObject.FindAnyObjectByType<EnemyPoolManager>();
    }

    private void onStateChanged()
    {
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
        Debug.LogError("Changing State: " + currentState);

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
        if(masks_.Count <= maskId)
        {
            currentMaskId_ = maskId;
            onMaskChange.Invoke(GetCurrentMask());
        }
    }

    public void NextMask()
    {
        currentMaskId_++;
        currentMaskId_ = currentMaskId_ % masks_.Count;
        onMaskChange.Invoke(GetCurrentMask());
    }

    public void PrevMask()
    {
        currentMaskId_--;
        currentMaskId_ = currentMaskId_ < 0 ? 0 : currentMaskId_;
        onMaskChange.Invoke(GetCurrentMask());
    }
    #endregion
}
