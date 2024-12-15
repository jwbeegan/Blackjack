using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GsDoubleDownSingle : GameState
{
    [SerializeField] private GameStateManager stateManager;
    private bool cardDealt;
    public override GameState SwitchToThisState()
    {
        #if UNITY_EDITOR
        Debug.Log("switching to double down state");
        #endif

        //enable buttons and text
        stateManager.buttonManager.ToggleButtonFullOff(ButtonManager.ButtonType.All);
        stateManager.textManager.DisableText(TextManager.TextEnum.All);
        stateManager.textManager.SetupDefaultText(TextManager.Defaults.MoneyScore);
        stateManager.activeState = this;

        stateManager.scoreManager.UpdateScores(DeckManager.CardDecks.player);
        stateManager.textManager.UpdateText(TextManager.TextEnum.PlayerScore, "" + stateManager.scoreManager.GetPlayerScore());
        stateManager.textManager.UpdateText(TextManager.TextEnum.DealerScore, "" + stateManager.scoreManager.GetShownDealerScore());

        //deal a card
        if (!cardDealt)
        {
            stateManager.betManager.DoubleDown();
            stateManager.graphicUpdater.UpdateHand(DeckManager.CardDecks.player, stateManager.deckManager.AddRandomCardToPlayerDeck(false), false);
            cardDealt = true;
            return stateManager.gsAnim.SwitchToThisState();
        }
        //otherwise, eval score then move to dealer move
        else
        {
            if (stateManager.scoreManager.GetPlayerScore() > 21)
            {
                cardDealt = false;
                return stateManager.gsPlayerLoss.SwitchToThisState();
            }
            else if (stateManager.scoreManager.GetPlayerScore() <= 21)
            {
                cardDealt = false;
                return stateManager.gsDealerMove.SwitchToThisState();
            }

        }


        return this;
    }
    public override GameState RegisterTrigger(GameStateManager.GameEventTriggers trigger)
    {
        return this;
    }

}
