using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSPlayerMove : GameState
{
    [SerializeField] private GameStateManager stateManager;
    public override GameState SwitchToThisState()
    {

        #if UNITY_EDITOR
        Debug.Log("switching to player move state");
        #endif

        //enable buttons and text
        stateManager.buttonManager.ToggleButtonFullOff(ButtonManager.ButtonType.All);
        stateManager.textManager.DisableText(TextManager.TextEnum.All);
        stateManager.textManager.SetupDefaultText(TextManager.Defaults.MoneyScore);
        stateManager.buttonManager.ToggleButtonFullOn(ButtonManager.ButtonType.Hit);
        stateManager.buttonManager.ToggleButtonFullOn(ButtonManager.ButtonType.Stand);

        stateManager.scoreManager.UpdateScores(DeckManager.CardDecks.player);

        stateManager.textManager.UpdateText(TextManager.TextEnum.PlayerScore, "" + stateManager.scoreManager.GetPlayerScore());
        stateManager.textManager.UpdateText(TextManager.TextEnum.DealerScore, "" + stateManager.scoreManager.GetShownDealerScore());

        Highlight();

        stateManager.activeState = this;

        //check for split
        if (stateManager.deckManager.CheckForSplit() && stateManager.betManager.CheckForSecondBetAllowed())
        {
            stateManager.buttonManager.ToggleButtonFullOn(ButtonManager.ButtonType.Split);
        }

        //check for double down
        if (stateManager.deckManager.CheckForDoubleDown(1) && stateManager.betManager.CheckForSecondBetAllowed())
        {
            stateManager.buttonManager.ToggleButtonFullOn(ButtonManager.ButtonType.DoubleDown);
        }

        //check score to make sure last hit was not over
        if (stateManager.scoreManager.GetPlayerScore() > 21)
        {
            DeHighlight();
            return stateManager.gsPlayerLoss.SwitchToThisState();
        }
        else if (stateManager.scoreManager.GetPlayerScore() == 21)
        {
            DeHighlight();
            return stateManager.gsDealerMove.SwitchToThisState();
        }

        return this;
    }

    public override GameState RegisterTrigger(GameStateManager.GameEventTriggers trigger)
    {
        switch(trigger)
        {
            case GameStateManager.GameEventTriggers.AddCardClicked:
                stateManager.graphicUpdater.UpdateHand(DeckManager.CardDecks.player, stateManager.deckManager.AddRandomCardToPlayerDeck(false), false);
                return stateManager.gsAnim.SwitchToThisState();
            case GameStateManager.GameEventTriggers.StandClicked:
                DeHighlight();
                return stateManager.gsDealerMove.SwitchToThisState();
            case GameStateManager.GameEventTriggers.SplitClicked:
                DeHighlight();
                return stateManager.gsSplit.SwitchToThisState();
            case GameStateManager.GameEventTriggers.DoubleDownClicked:
                DeHighlight();
                return stateManager.gsDoubleDownSingle.SwitchToThisState();
            default: break;
        }
        
        return this;
    }

    private void Highlight()
    {

        #if UNITY_EDITOR
        Debug.Log("highlighting cards");
        #endif

        foreach (GameObject card in stateManager.graphicUpdater.GetSplitCards(0))
            stateManager.layerChanger.ChangeToOutline(card);

    }

    private void DeHighlight()
    {

        #if UNITY_EDITOR
        Debug.Log("Un-highlighting cards");
        #endif

        foreach (GameObject card in stateManager.graphicUpdater.GetSplitCards(0))
            stateManager.layerChanger.ChangeToDefault(card);

    }
}
