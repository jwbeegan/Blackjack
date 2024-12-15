using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSPlayerLoss : GameState
{
    [SerializeField] private GameStateManager stateManager;
    public override GameState SwitchToThisState()
    {
        //enable buttons and text
        stateManager.buttonManager.ToggleButtonFullOff(ButtonManager.ButtonType.All);
        stateManager.textManager.DisableText(TextManager.TextEnum.All);
        stateManager.textManager.SetupDefaultText(TextManager.Defaults.MoneyScore);
        stateManager.textManager.EnableText(TextManager.TextEnum.Loser);
        stateManager.activeState = this;
        
        #if UNITY_EDITOR
        Debug.Log("loss");
        #endif

        stateManager.betManager.LoseBet();

        stateManager.soundManager.PlaySound(SoundManager.SoundType.Returnchips);

        if (stateManager.betManager.GetPlayerMoney() <= 0)
        {
            //turn on the end game button if you run out of money at the end of a round
            stateManager.buttonManager.ToggleButtonFullOn(ButtonManager.ButtonType.EndGame);
        }
        else
        {
            //otherwise turn on the next round button
            stateManager.buttonManager.ToggleButtonFullOn(ButtonManager.ButtonType.NextRound);

        }

        return this;
    }
    public override GameState RegisterTrigger(GameStateManager.GameEventTriggers trigger)
    {
        switch (trigger)
        {
            case GameStateManager.GameEventTriggers.NextRoundClicked:
                return stateManager.gsBet.SwitchToThisState();

            case GameStateManager.GameEventTriggers.EndGameClicked:
                return stateManager.gsMenu.SwitchToThisState();
            default: return this;
        }
    }
}
