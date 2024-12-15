using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSInitialDeal : GameState
{
    [SerializeField] private GameStateManager stateManager;
    private int deals;
    //what does the state do when it enters
    public override GameState SwitchToThisState()
    {
        #if UNITY_EDITOR
        Debug.Log("switching to initial deal state");
        #endif

        stateManager.buttonManager.ToggleButtonFullOff(ButtonManager.ButtonType.All);
        stateManager.textManager.DisableText(TextManager.TextEnum.All);
        stateManager.textManager.SetupDefaultText(TextManager.Defaults.JustMoney);

        stateManager.activeState = this;

        RunInitialDeals();

        return this;
    }

    //which triggers does the state react to and how
    public override GameState RegisterTrigger(GameStateManager.GameEventTriggers trigger)
    {
        switch (trigger)
        {
            case GameStateManager.GameEventTriggers.AnimationFinish:
                if (deals < 4)
                    RunInitialDeals();
                else
                {
                    deals = 0;
                    return stateManager.gsPlayerMove.SwitchToThisState();
                }
                break;
        }
        return this;
    }

    private void RunInitialDeals()
    {
        switch(deals)
        {
            case 0:
                stateManager.graphicUpdater.UpdateHand(DeckManager.CardDecks.player, stateManager.deckManager.AddRandomCardToPlayerDeck(false), false);
                break;
            case 1:
                stateManager.graphicUpdater.UpdateHand(DeckManager.CardDecks.dealer, stateManager.deckManager.AddRandomCardToDealerDeck(false), false);
                break;
            case 2:
                stateManager.graphicUpdater.UpdateHand(DeckManager.CardDecks.player, stateManager.deckManager.AddRandomCardToPlayerDeck(false), false);
                break;
            case 3:
                stateManager.graphicUpdater.UpdateHand(DeckManager.CardDecks.dealer, stateManager.deckManager.AddRandomCardToDealerDeck(false), true);
                break;
            default:
                Debug.LogError("INVALID DEAL RUN");
                break;
        }

        deals += 1;
        stateManager.scoreManager.UpdateScores(DeckManager.CardDecks.player);
        stateManager.scoreManager.UpdateScores(DeckManager.CardDecks.dealer);

    }
}
