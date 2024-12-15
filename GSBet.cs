using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSBet : GameState
{
    [SerializeField] private GameStateManager stateManager;

    //what does the state do when it enters
    public override GameState SwitchToThisState()
    {
        #if UNITY_EDITOR
        Debug.Log("switching to bet state");
        #endif

        stateManager.buttonManager.ToggleButtonFullOff(ButtonManager.ButtonType.All);
        stateManager.textManager.DisableText(TextManager.TextEnum.All);
        stateManager.buttonManager.ToggleButtonFullOn(ButtonManager.ButtonType.StartRound);

        stateManager.textManager.EnableText(TextManager.TextEnum.PlayerMoneyText);
        stateManager.textManager.EnableText(TextManager.TextEnum.PlayerMoney);

        stateManager.deckManager.ResetDecks();
        stateManager.graphicUpdater.CleanAllCards();
        stateManager.ResetSplit();
        stateManager.chipSelector.ResetChipUIPosition();
        stateManager.chipSelector.EnableChipSelect();

        stateManager.activeState = this;

        return this;
    }

    public override GameState RegisterTrigger(GameStateManager.GameEventTriggers trigger)
    {
        switch (trigger)
        {
            case GameStateManager.GameEventTriggers.StartRoundClicked:

                stateManager.chipSelector.DisableChipSelect();
                stateManager.betManager.SetBet((int)stateManager.chipSpawner.GetChipTypeFromArrayIndex(stateManager.chipSelector.GetChipSelected()));

                return stateManager.gsInitialDeal.SwitchToThisState();

            default: return this;
        }
    }
}
