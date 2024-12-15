using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState : MonoBehaviour
{
    //what does the state do when it enters
    public abstract GameState SwitchToThisState();

    //which triggers does the state react to and how
    public abstract GameState RegisterTrigger(GameStateManager.GameEventTriggers trigger);
}
