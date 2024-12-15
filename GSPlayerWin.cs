using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSPlayerWin : GameState
{
    [SerializeField] private GameStateManager stateManager;

    public override GameState SwitchToThisState()
    {
        //enable buttons and text
        stateManager.buttonManager.ToggleButtonFullOff(ButtonManager.ButtonType.All);
        stateManager.buttonManager.ToggleButtonFullOn(ButtonManager.ButtonType.NextRound);
        stateManager.textManager.DisableText(TextManager.TextEnum.All);
        stateManager.textManager.SetupDefaultText(TextManager.Defaults.MoneyScore);
        stateManager.textManager.EnableText(TextManager.TextEnum.Winner);

        #if UNITY_EDITOR
        Debug.Log("Winner");
        #endif
        
        //do player money
        stateManager.betManager.WinPool();

        stateManager.soundManager.PlaySound(SoundManager.SoundType.Returnchips);

        stateManager.activeState = this;
        return this;
    }
    public override GameState RegisterTrigger(GameStateManager.GameEventTriggers trigger)
    {
        switch (trigger)
        {
            case GameStateManager.GameEventTriggers.NextRoundClicked:
                return stateManager.gsBet.SwitchToThisState();
            default: return this;
        }
    }
}
