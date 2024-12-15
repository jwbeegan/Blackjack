using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int playerScore;
    [SerializeField] private int playerSplitScore;
    [SerializeField] private int visibleDealerScore;
    [SerializeField] private DeckManager deckManager;
    public delegate void DealerScoreUpdated(int score);
    public static event DealerScoreUpdated OnDealerScoreUpdated;
    public delegate void PlayerScoreUpdated(int score);
    public static event PlayerScoreUpdated OnPlayerScoreUpdated;

    public void UpdateScores(DeckManager.CardDecks handToUpdate)
    {
        switch(handToUpdate)
        {
            case DeckManager.CardDecks.player:
                playerScore = AddScore(deckManager.GetCurrentDeck(handToUpdate));
                OnPlayerScoreUpdated(playerScore);
                break;

            case DeckManager.CardDecks.splitPlayer:
                playerSplitScore = AddScore(deckManager.GetCurrentDeck(handToUpdate));
                break;
            case DeckManager.CardDecks.dealer:
                visibleDealerScore = AddScore(deckManager.GetCurrentDeck(handToUpdate));
                OnDealerScoreUpdated(visibleDealerScore);
                break;
            default:
                Debug.LogError("invalid hand in update scores");
                return;
        }


    }

    private int AddScore(List<sPlayingCard> cardsToAdd)
    {
        int outputScore = 0;
        int acesFound = 0;
        foreach (sPlayingCard card in cardsToAdd)
        {
            if (card.Rank != 1)
                outputScore += DeckManager.CardValue(card);
            else
                acesFound += 1;
        }

        //find how many 11's go into the remainder
        int numElevens = (21 - outputScore) / 11;

        for (int i = 0; i < acesFound; i++)
        {
            if (numElevens > 0)
            {
                outputScore += 11;
                numElevens -= 1;
            }
            else
                outputScore += 1;
        }

        return outputScore;
    }

    public int GetPlayerScore()
    {
        return playerScore;
    }

    public int GetDealerScore()
    {
        return visibleDealerScore;
    }

    public int RevealDealerScore()
    {
        OnDealerScoreUpdated(visibleDealerScore);
        return visibleDealerScore;

    }

    public int GetShownDealerScore()
    {
        //return score of first card
        return DeckManager.CardValue(deckManager.GetCurrentDeck(DeckManager.CardDecks.dealer)[0]);
    }

    public int GetSplitScore()
    {
        return playerSplitScore;
    }

}
