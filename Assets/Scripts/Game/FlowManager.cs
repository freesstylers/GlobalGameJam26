using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlowManager : MonoBehaviour
{
    public enum State { Cooldown, Spawn, Round, Improvement, EndGame };
    public int[] timers = { 10, 5, 60, -1, -1 };
    public static FlowManager instance;

    [SerializeField]
    GameObject spawnerParent;

    public EnemySpawner[] spawners;

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

    public static event Action<State> onStateChange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = State.Cooldown;

        spawners = spawnerParent.transform.GetComponentsInChildren<EnemySpawner>();
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

                foreach (EnemySpawner eS in spawners)
                {
                   StartCoroutine(eS.Spawn());
                }

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
}
