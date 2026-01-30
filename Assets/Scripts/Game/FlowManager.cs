using UnityEngine;
using UnityEngine.SceneManagement;

public class FlowManager : MonoBehaviour
{
    public enum State { Cooldown, Spawn, Round, Improvement, EndGame };
    public int[] timers = { 10, 5, 60, -1, -1 };
    public static FlowManager instance;

    public State currentState;

    float timerValue = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
