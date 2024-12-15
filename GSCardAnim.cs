using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GSCardAnim : GameState
{
    [SerializeField] private GameStateManager stateManager;
    private GameState lastState;
    [SerializeField] private float animTime;
    private float animTimer;
    [SerializeField] bool animationFinished;
    [SerializeField] private bool timerFinished;

    private void Update()
    {
        //run timer
        if (stateManager.activeState == this)
            if (!timerFinished)
            {
                if (animTimer > 0)
                {
                    animTimer -= Time.deltaTime;
                }

                if (animTimer <= 0)
                {
                    if (animationFinished)
                    {
                        stateManager.activeState = lastState.SwitchToThisState();
                    }
                    else
                    {
                        timerFinished = true;
                    }
                }
            }


    }
    public override GameState SwitchToThisState()
    {
        #if UNITY_EDITOR
        Debug.Log("switching to animation state");
        #endif
        stateManager.buttonManager.ToggleButtonFullOff(ButtonManager.ButtonType.All);
        stateManager.textManager.DisableText(TextManager.TextEnum.All);
        stateManager.textManager.SetupDefaultText(TextManager.Defaults.JustMoney);
        stateManager.textManager.EnableText(TextManager.TextEnum.DealerScore);

        stateManager.buttonManager.ShowButton(ButtonManager.ButtonType.Hit);
        stateManager.buttonManager.DisableButton(ButtonManager.ButtonType.Hit);
        stateManager.buttonManager.ShowButton(ButtonManager.ButtonType.Stand);
        stateManager.buttonManager.DisableButton(ButtonManager.ButtonType.Stand);

        lastState = stateManager.activeState;

        if (stateManager.IsSplitHand())
        {
            if (stateManager.CheckSplitProgression() >= 5)
            {
                stateManager.textManager.EnableText(TextManager.TextEnum.SplitScore1);
                stateManager.textManager.EnableText(TextManager.TextEnum.SplitScore2);
            }

        }
        else
        {
            stateManager.textManager.EnableText(TextManager.TextEnum.PlayerScore);
        }

        stateManager.activeState = this;
        animTimer = animTime;
        timerFinished = false;
        animationFinished = false;
        return this;
    }
    public override GameState RegisterTrigger(GameStateManager.GameEventTriggers trigger)
    {
        switch (trigger)
        {
            case GameStateManager.GameEventTriggers.AnimationFinish:

                #if UNITY_EDITOR
                Debug.Log("registering anim finish in card anim, last state = " + lastState.ToString());
                #endif

                if (timerFinished)
                {
                    return lastState.SwitchToThisState();
                }
                else
                {
                    animationFinished = true;
                    return this;
                }
            default: return this;
        }
    }
}
