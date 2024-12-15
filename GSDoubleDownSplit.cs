using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSDoubleDownSplit : GameState
{
    [SerializeField] private GameStateManager stateManager;
    private int cardSplitSeq;
    private bool firstHandDone;
    private bool secondHandDone;
    public override GameState SwitchToThisState()
    {
        #if UNITY_EDITOR
        Debug.Log("switching to split state");
        #endif

        stateManager.buttonManager.ToggleButtonFullOff(ButtonManager.ButtonType.All);
        stateManager.textManager.DisableText(TextManager.TextEnum.All);
        stateManager.textManager.SetupDefaultText(TextManager.Defaults.SplitMoneyScore);
        stateManager.activeState = this;

        cardSplitSeq = stateManager.CheckSplitProgression();

        updateSplitScores();

        if (cardSplitSeq == 5)
        {
            if (!firstHandDone)
            {
                //deal card, then move onto the next hand
                #if UNITY_EDITOR
                Debug.Log("Adding to player deck");
                #endif
                
                stateManager.betManager.DoubleDown();
                stateManager.graphicUpdater.UpdateHand(DeckManager.CardDecks.player, stateManager.deckManager.AddRandomCardToPlayerDeck(false), false);
                firstHandDone = true;
                return stateManager.gsAnim.SwitchToThisState();
            }
            else
            {
                stateManager.IncrementSplitProgression();
                firstHandDone = false;
                return stateManager.gsSplit.SwitchToThisState();
            }
        }
        //card split sequence is 5
        else
        {
            //deal card the move onto next hand

            if (!secondHandDone)
            {
                //deal card, then move onto the next hand
                
                #if UNITY_EDITOR
                Debug.Log("Adding to split deck");
                #endif
                
                stateManager.betManager.DoubleDownSplitBet();
                stateManager.graphicUpdater.UpdateHand(DeckManager.CardDecks.splitPlayer, stateManager.deckManager.AddRandomCardToSplitDeck(false), false);
                secondHandDone = true;
                return stateManager.gsAnim.SwitchToThisState();
            }
            else
            {
                stateManager.IncrementSplitProgression();
                secondHandDone = false;

                //check score, if both above 21 can move straight to resolve, otherwise do dealer move
                if ((stateManager.scoreManager.GetPlayerScore() > 21) && (stateManager.scoreManager.GetSplitScore() > 21))
                {
                    //return player loss
                    #if UNITY_EDITOR
                    Debug.Log("both score over 21, going to resolve scores");
                    #endif
                    
                    foreach (GameObject card in stateManager.graphicUpdater.GetSplitCards(1))
                        stateManager.layerChanger.ChangeToDefault(card);
                    return stateManager.gsSplitResolve.SwitchToThisState();
                }
                else
                {
                    foreach (GameObject card in stateManager.graphicUpdater.GetSplitCards(1))
                        stateManager.layerChanger.ChangeToDefault(card);
                    return stateManager.gsDealerMove.SwitchToThisState();
                }
            }
        }

    }
    public override GameState RegisterTrigger(GameStateManager.GameEventTriggers trigger)
    {
        return this;
    }
    private void updateSplitScores()
    {
        stateManager.scoreManager.UpdateScores(DeckManager.CardDecks.player);
        stateManager.scoreManager.UpdateScores(DeckManager.CardDecks.splitPlayer);
        stateManager.textManager.UpdateText(TextManager.TextEnum.SplitScore1, "" + stateManager.scoreManager.GetPlayerScore());
        stateManager.textManager.UpdateText(TextManager.TextEnum.SplitScore2, "" + stateManager.scoreManager.GetSplitScore());

    }


}
