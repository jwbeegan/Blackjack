using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [Header("Deck reference")]
    [SerializeField] private sDeck startingDeck;
    [Space(10)]

    [Header("Deck Monitoring")]
    [SerializeField] private List<sPlayingCard> drawDeck;
    [SerializeField] private List<sPlayingCard> playerDeck;
    [SerializeField] private List<sPlayingCard> dealerDeck;
    [SerializeField] private List<sPlayingCard> playerSplitDeck;
    [Space(10)]

    [Header("script references")]
    [SerializeField] private GameStateManager gameManager;

    //enums
    public enum CardDecks { draw = 1, player = 2, dealer = 3, splitPlayer = 4};


    private void Awake()
    {
        drawDeck = new List<sPlayingCard>();
        playerDeck = new List<sPlayingCard>();
        dealerDeck = new List<sPlayingCard>();
    }

    private void OnEnable()
    {
        CreateInitialDeck();
    }

    public void ResetDecks()
    {
        drawDeck.Clear();
        playerDeck.Clear();
        dealerDeck.Clear();
        playerSplitDeck.Clear();
        CreateInitialDeck();
    }
    private void CreateInitialDeck()
    {
        foreach (sPlayingCard card in startingDeck.CardArray)
        {
            drawDeck.Add(card);
        }
    }

    private sPlayingCard GetRandomCardFromDeck(CardDecks deck)
    {
        List<sPlayingCard> deckToDrawFrom = GetCurrentDeck(deck);

        if (deckToDrawFrom.Count > 0)
        {
            return deckToDrawFrom[Random.Range(0, deckToDrawFrom.Count - 1)];
        }
        else
        {
            Debug.LogError("Deck count 0 when attempting to get card from " + deck);
            return null;
        }
    }

    private sPlayingCard MoveCard(sPlayingCard targetCard, CardDecks inputStartingDeck, CardDecks inputTargetDeck, bool flipLater)
    {
        List<sPlayingCard> startingDeck = GetCurrentDeck(inputStartingDeck);
        List<sPlayingCard> targetDeck = GetCurrentDeck(inputTargetDeck);

        if (startingDeck.Contains(targetCard))
        {
            startingDeck.Remove(targetCard);
            targetDeck.Add(targetCard);

            //OnCardAddedToDeck(inputTargetDeck, targetCard, targetDeck, flipLater);
            return targetCard;
        }
        else
        {
            Debug.LogError("starting deck does not contain target card");
            return null;
        }
    }

    public sPlayingCard AddRandomCardToPlayerDeck(bool flipLater)
    {
        return MoveCard(GetRandomCardFromDeck(CardDecks.draw), CardDecks.draw, CardDecks.player, flipLater);
    }

    public sPlayingCard AddRandomCardToDealerDeck(bool flipLater)
    {
        return MoveCard(GetRandomCardFromDeck(CardDecks.draw), CardDecks.draw, CardDecks.dealer, flipLater);
    }
    public sPlayingCard AddRandomCardToSplitDeck(bool flipLater)
    {
        return MoveCard(GetRandomCardFromDeck(CardDecks.draw), CardDecks.draw, CardDecks.splitPlayer, flipLater);
    }


    public List<sPlayingCard> GetCurrentDeck(CardDecks deckNum)
    {
        switch(deckNum)
        {
            case CardDecks.draw:
                return drawDeck;
            case CardDecks.player:
                return playerDeck;
            case CardDecks.dealer:
                return dealerDeck;
            case CardDecks.splitPlayer:
                return playerSplitDeck;
            default:
                Debug.LogError("improper deck inquiry in get current deck");
                return null;
        }
    }

    public static int CardValue(sPlayingCard card)
    {
        if (card.Rank < 10)
        {
            return card.Rank;
        }
        else
            return 10;
    }

    public bool CheckForSplit()
    {
        //if the rank is the same, allow split for just one turn
        if ((playerDeck[0].Rank == playerDeck[1].Rank) && (playerDeck.Count < 3))
        {

            #if UNITY_EDITOR
            Debug.Log("SPLIT DETECTED");
            #endif
            return true;
        }

        return false;

    }


    public bool CheckForDoubleDown(int hand)
    {
        //double down allowed when there are only two cards in either the split, or in the hand
        //is split, check which hand
        if (gameManager.IsSplitHand())
        {
            switch (hand)
            {
                case 0:
                    if (playerDeck.Count == 2)
                        return true;
                    else
                        return false;
                case 1:
                    if (playerSplitDeck.Count == 2)
                        return true;
                    else
                        return false;

                default:
                    Debug.LogError("INCORRECT HAND IN CHECKFORDOUBLEDOWN");
                    return false;
            }

        }
        else if (playerDeck.Count == 2)
            return true;
        else
            return false;
    }

    public void SplitDeck() //should only be calling this if we already have a split
    {
        if (!((playerDeck[0].Rank == playerDeck[1].Rank) && (playerDeck.Count < 3)))
        {
            Debug.LogError("Invalid conditions when going to split decks");
            return;
        }

        MoveCard(playerDeck[1], CardDecks.player, CardDecks.splitPlayer, false);

    }



}
