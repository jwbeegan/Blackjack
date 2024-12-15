using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GSSplit : GameState
{
    [SerializeField] private GameStateManager stateManager;
    private int cardSplitSeq;

    public override GameState SwitchToThisState()
    {
        #if UNITY_EDITOR
        Debug.Log("switching to split state");
        
        #endif
        stateManager.buttonManager.ToggleButtonFullOff(ButtonManager.ButtonType.All);
        stateManager.textManager.DisableText(TextManager.TextEnum.All);
        stateManager.textManager.SetupDefaultText(TextManager.Defaults.JustMoney);
        stateManager.textManager.EnableText(TextManager.TextEnum.DealerScore);
        stateManager.activeState = this;

        cardSplitSeq = stateManager.CheckSplitProgression();

        #if UNITY_EDITOR
        Debug.Log("Split seq = " + cardSplitSeq);
        #endif

        //first time it enters this state each round, make the deck manager split the decks, and the anim manager change the positions
        if (cardSplitSeq == 0)
        {
            stateManager.deckManager.SplitDeck();
            stateManager.graphicUpdater.SplitCards();
            stateManager.IncrementSplitProgression();
            stateManager.RegisterSplit();
            stateManager.betManager.Split();
            stateManager.graphicUpdater.OrganizeHand(DeckManager.CardDecks.splitPlayer);
            stateManager.textManager.UpdateText(TextManager.TextEnum.SplitScore1, "" + stateManager.scoreManager.GetPlayerScore()/2);
            stateManager.textManager.UpdateText(TextManager.TextEnum.SplitScore2, "" + stateManager.scoreManager.GetPlayerScore() / 2);
            stateManager.textManager.UpdateText(TextManager.TextEnum.PlayerMoney, "" + stateManager.betManager.GetPlayerMoney());
            return stateManager.gsAnim.SwitchToThisState();

        }
        else if (cardSplitSeq == 1)
        {
            stateManager.graphicUpdater.OrganizeHand(DeckManager.CardDecks.player);
            stateManager.IncrementSplitProgression();
            updateSplitScores();
            return stateManager.gsAnim.SwitchToThisState();

        }
        else if (cardSplitSeq == 2)
        {
            #if UNITY_EDITOR
            Debug.Log("Adding to player deck");
            #endif
            
            stateManager.IncrementSplitProgression();
            stateManager.graphicUpdater.UpdateHand(DeckManager.CardDecks.player, stateManager.deckManager.AddRandomCardToPlayerDeck(false), false);
            updateSplitScores();
            return stateManager.gsAnim.SwitchToThisState();

        }
        else if (cardSplitSeq == 3)
        {
            #if UNITY_EDITOR
            Debug.Log("Adding to split deck");
            #endif

            stateManager.IncrementSplitProgression();
            stateManager.graphicUpdater.UpdateHand(DeckManager.CardDecks.splitPlayer, stateManager.deckManager.AddRandomCardToSplitDeck(false), false);
            updateSplitScores();
            return stateManager.gsAnim.SwitchToThisState();

        }
        else
        {
            stateManager.buttonManager.ToggleButtonFullOn(ButtonManager.ButtonType.Hit);
            stateManager.buttonManager.ToggleButtonFullOn(ButtonManager.ButtonType.Stand);
            stateManager.textManager.EnableText(TextManager.TextEnum.SplitScore1);
            stateManager.textManager.EnableText(TextManager.TextEnum.SplitScore2);

            updateSplitScores();
        }

        if (cardSplitSeq == 4) //increment here to signal that the anim is done and can not move on to highlight. show score etc
        {
            stateManager.IncrementSplitProgression();
            cardSplitSeq = stateManager.CheckSplitProgression();
        }


        if (cardSplitSeq == 5)
        {
            //not yet above limit, highlight cards, check if double down is ok
            if (stateManager.scoreManager.GetPlayerScore() < 21)
            {
                foreach (GameObject card in stateManager.graphicUpdater.GetSplitCards(0))
                    stateManager.layerChanger.ChangeToOutline(card);

                if (stateManager.deckManager.CheckForDoubleDown(0) && stateManager.betManager.CheckForSecondBetAllowed())
                {
                    stateManager.buttonManager.ToggleButtonFullOn(ButtonManager.ButtonType.DoubleDown);
                }
            }
            //if greater than 21, transition
            else
            {

                foreach (GameObject card in stateManager.graphicUpdater.GetSplitCards(0))
                    stateManager.layerChanger.ChangeToDefault(card);
                foreach (GameObject card in stateManager.graphicUpdater.GetSplitCards(1))
                    stateManager.layerChanger.ChangeToOutline(card);

                stateManager.IncrementSplitProgression();
                return this.SwitchToThisState();
            }
        }
        else if (cardSplitSeq == 6)
        {
            //if both above 21 just end
            if ((stateManager.scoreManager.GetPlayerScore() > 21) && (stateManager.scoreManager.GetSplitScore() > 21))
            {
                foreach (GameObject card in stateManager.graphicUpdater.GetSplitCards(1))
                    stateManager.layerChanger.ChangeToDefault(card);

                //return player loss
                #if UNITY_EDITOR
                Debug.Log("both score over 21, going to resolve scores");
                #endif

                return stateManager.gsSplitResolve.SwitchToThisState();


            }
            //if just the split is above 21 but the players is not, dealer move necessary
            else if (stateManager.scoreManager.GetSplitScore() >= 21)
            {
                #if UNITY_EDITOR
                Debug.Log("just the split score over 21, going to dealer move");
                #endif
                
                foreach (GameObject card in stateManager.graphicUpdater.GetSplitCards(1))
                    stateManager.layerChanger.ChangeToDefault(card);

                //go to dealer turn
                return stateManager.gsDealerMove.SwitchToThisState();
            }
            else
            {
                foreach (GameObject card in stateManager.graphicUpdater.GetSplitCards(0))
                    stateManager.layerChanger.ChangeToDefault(card);

                foreach (GameObject card in stateManager.graphicUpdater.GetSplitCards(1))
                    stateManager.layerChanger.ChangeToOutline(card);

                #if UNITY_EDITOR
                Debug.Log("double down allowed = " + stateManager.deckManager.CheckForDoubleDown(1) + " and splitbetallowed = " + stateManager.betManager.CheckForSecondSplitBetAllowed());
                #endif
                
                if (stateManager.deckManager.CheckForDoubleDown(1) && stateManager.betManager.CheckForSecondSplitBetAllowed())
                {
                    stateManager.buttonManager.ToggleButtonFullOn(ButtonManager.ButtonType.DoubleDown);
                }

            }
        }
        return this;
    }
    public override GameState RegisterTrigger(GameStateManager.GameEventTriggers trigger)
    {
        switch (trigger)
        {
            case GameStateManager.GameEventTriggers.AddCardClicked:
                //if not moved onto the second deck yet
                if (stateManager.CheckSplitProgression() == 5)
                {
                    #if UNITY_EDITOR
                    Debug.Log("Adding to player deck");
                    #endif
                    
                    stateManager.graphicUpdater.UpdateHand(DeckManager.CardDecks.player, stateManager.deckManager.AddRandomCardToPlayerDeck(false), false);
                    return stateManager.gsAnim.SwitchToThisState();
                }
                else
                {
                    #if UNITY_EDITOR
                    Debug.Log("Adding to split deck");
                    #endif

                    stateManager.graphicUpdater.UpdateHand(DeckManager.CardDecks.splitPlayer, stateManager.deckManager.AddRandomCardToSplitDeck(false), false);
                    return stateManager.gsAnim.SwitchToThisState();

                }
            case GameStateManager.GameEventTriggers.StandClicked:
                //if not moved onto the second deck yet
                if (stateManager.CheckSplitProgression() == 5)
                {
                    #if UNITY_EDITOR
                    Debug.Log("moving to second hand");
                    #endif
                    
                    stateManager.IncrementSplitProgression();
                    return this.SwitchToThisState();
                }
                else
                {
                    #if UNITY_EDITOR
                    Debug.Log("held on second hand, going to dealer move");
                    #endif

                    foreach (GameObject card in stateManager.graphicUpdater.GetSplitCards(1))
                        stateManager.layerChanger.ChangeToDefault(card);
                    return stateManager.gsDealerMove.SwitchToThisState();
                }
            case GameStateManager.GameEventTriggers.DoubleDownClicked:
                {
                    #if UNITY_EDITOR
                    Debug.Log("changing to double down split");
                    #endif

                    return stateManager.gsDoubleDownSplit.SwitchToThisState();
                }
        }
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
