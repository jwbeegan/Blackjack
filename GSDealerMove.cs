using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GSDealerMove : GameState
{
    [SerializeField] private GameStateManager stateManager;
    private int playerScore;
    private int dealerScore;
    private bool cardFlipped;
    public override GameState SwitchToThisState()
    {

        #if UNITY_EDITOR
        Debug.Log("switching to dealer move state");
        #endif

        //enable buttons and text
        stateManager.buttonManager.ToggleButtonFullOff(ButtonManager.ButtonType.All);
        stateManager.textManager.DisableText(TextManager.TextEnum.All);
        stateManager.textManager.SetupDefaultText(TextManager.Defaults.MoneyScore);

        stateManager.scoreManager.UpdateScores(DeckManager.CardDecks.dealer);


        playerScore = stateManager.scoreManager.GetPlayerScore();
        dealerScore = stateManager.scoreManager.GetDealerScore();

        stateManager.textManager.UpdateText(TextManager.TextEnum.PlayerScore, "" + playerScore);
        stateManager.textManager.UpdateText(TextManager.TextEnum.DealerScore, "" + dealerScore);
        stateManager.activeState = this;

        if (stateManager.IsSplitHand())
        {
            stateManager.textManager.EnableText(TextManager.TextEnum.SplitBet);
            stateManager.textManager.EnableText(TextManager.TextEnum.SplitBetText);
            stateManager.textManager.EnableText(TextManager.TextEnum.SplitScore1);
            stateManager.textManager.EnableText(TextManager.TextEnum.SplitScore2);

        }

        if (!cardFlipped)
        {
            #if UNITY_EDITOR
            Debug.Log("flipping card from gs dealer move");
            #endif

            stateManager.graphicUpdater.FlipCard();
            dealerScore = stateManager.scoreManager.RevealDealerScore();
            stateManager.textManager.UpdateText(TextManager.TextEnum.DealerScore, "" + dealerScore);
            cardFlipped = true;

            #if UNITY_EDITOR
            Debug.Log("triggered flip card, changing to anim state");
            #endif

            return stateManager.gsAnim.SwitchToThisState();

        }
        else return DetermineNextMove();

    }
    public override GameState RegisterTrigger(GameStateManager.GameEventTriggers trigger)
    {
        return this;
    }


    private GameState DetermineNextMove()
    {

        if (dealerScore < 18) //hit
        {
            stateManager.graphicUpdater.UpdateHand(DeckManager.CardDecks.dealer, stateManager.deckManager.AddRandomCardToDealerDeck(false), false);
            return stateManager.gsAnim.SwitchToThisState();
        }
        else if (!stateManager.IsSplitHand()) //call
        {
            if ((playerScore > dealerScore) || (dealerScore > 21)) //player wins
            {
                cardFlipped = false;
                return stateManager.gsPlayerWin.SwitchToThisState();
            }
            else if (playerScore == dealerScore) //tie
            {
                cardFlipped = false;
                return stateManager.gsTie.SwitchToThisState();
            }
            else
            {
                cardFlipped = false;
                return stateManager.gsPlayerLoss.SwitchToThisState();
            }
        }
        else
        {
            cardFlipped = false;
            return stateManager.gsSplitResolve.SwitchToThisState();
        }

    }
}
