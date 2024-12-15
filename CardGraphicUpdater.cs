using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGraphicUpdater : MonoBehaviour
{
    [Header("Card object prefab")]
    [SerializeField] private GameObject cardPrefab;
    [Space(10)]

    [Header("Card offsets and home positions")]
    [SerializeField] private Vector3 playerHandHome;
    [SerializeField] private Vector3 cardOffset;
    [SerializeField] private Vector3 splitOffset;
    [SerializeField] private Vector3 dealerHandHome;
    [SerializeField] private Vector3 cardStartPosition;
    [SerializeField] private Vector3 split1Home;
    [SerializeField] private Vector3 split2Home;
    private Vector3 activeplayerHome;
    private Vector3 activePlayerOffset;
    [Space(10)]

    [Header("Card movement speed")]
    [SerializeField] private float moveSpeed;
    [Space(10)]

    [Header("Reference to state manager")]
    [SerializeField] private GameStateManager stateManager;

    //lists of card objects
    private List<GameObject> playerCardObjects = new List<GameObject>();
    private List<GameObject> dealerCardObjects = new List<GameObject>();
    private List<GameObject> playerSplitObjects = new List<GameObject>();

    private Vector3[] animTargetPositions = new Vector3[20];
    private Vector3[] animStartingPositions = new Vector3[20];

    //list of cards which are actively animating
    private List<GameObject> animList;

    //active card animator, animation substate and refreence animator to flip later
    private Animator cardAnimator;
    private enum AnimSubState { CardsToPos, CardFlip, Finish, None };
    [Header("Current animation substate")]
    [SerializeField] private AnimSubState currentSubState;
    private Animator animToFlipLater;

    //event for when animation has finished
    public delegate void CardAnimationFinished();
    public static event CardAnimationFinished OnCardAnimationFinished;

    private void Awake()
    {
        activeplayerHome = playerHandHome;
        activePlayerOffset = cardOffset;
        currentSubState = AnimSubState.None;

    }

    public void CleanAllCards()
    {
        for (int i = playerCardObjects.Count - 1; i >= 0; i--)
        {

            #if UNITY_EDITOR
            Debug.Log("removing player card, count = " + playerCardObjects.Count + " i = " + i);
            #endif
            
            GameObject tempObject = playerCardObjects[i];
            playerCardObjects.Remove(tempObject);
            Destroy(tempObject); 
        }
        for (int i = dealerCardObjects.Count - 1; i >= 0; i--)
        {
            #if UNITY_EDITOR
            Debug.Log("removing dealer card, count = " + dealerCardObjects.Count + " i = " + i);
            #endif

            GameObject tempObject = dealerCardObjects[i];
            dealerCardObjects.Remove(tempObject);
            Destroy(tempObject); 
        }
        for (int i = playerSplitObjects.Count - 1; i >= 0; i--)
        { 
            #if UNITY_EDITOR
            Debug.Log("removing dealer card, count = " + dealerCardObjects.Count + " i = " + i);
            #endif
            
            GameObject tempObject = playerSplitObjects[i];
            playerSplitObjects.Remove(tempObject);
            Destroy(tempObject); 
        }
    }

    private void Update()
    {
        if (currentSubState != AnimSubState.None)
            CheckForAnimationFinish();

    }

    private void CheckForAnimationFinish()
    {

        switch (currentSubState)
        {

            //if cards are moving to position, then run card animations.
            case AnimSubState.CardsToPos:
                //if the card animation returns false, it is complete, move onto the next sub state
                if (!RunHandAnimations())
                {
                    //if card is turned but not flipped, if it needs to flip immediately, flip it and move to next state
                    //otherwise return to static state and signal animation complete
                    if (cardAnimator.GetCurrentAnimatorStateInfo(0).IsName("CardTurned"))
                    {
                        //if this is the card to flip later, dont flip, just mark done
                        if ((animToFlipLater != null) && (animToFlipLater == cardAnimator))
                        {
                            currentSubState = AnimSubState.None;
                            OnCardAnimationFinished();
                        }
                        else
                        {
                            #if UNITY_EDITOR
                            Debug.Log("trigger flip card");
                            #endif

                            cardAnimator.SetTrigger("FlipCard");
                            stateManager.soundManager.PlaySound(SoundManager.SoundType.Cardflip);
                            currentSubState = AnimSubState.CardFlip;
                        }
                    }
                    else
                        Debug.LogError("CARD NOT READY TO FLIP ONCE MOVEMENT DONE IN CARD GRAPHIC UPDATER");
                }

                break;
            case AnimSubState.CardFlip:
                //check if the card flip is finished, if so trigger animator to move to default state and change state machine to finish state
                if ((cardAnimator.GetCurrentAnimatorStateInfo(0).IsName("CardFlip")) && (cardAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime>=1))
                {
                    #if UNITY_EDITOR
                    Debug.Log("trigger finish");
                    #endif

                    cardAnimator.SetTrigger("Finish");
                    currentSubState = AnimSubState.Finish;

                }
                break;
            case AnimSubState.Finish:
                //if the card animator is now into its default state, return state machine to static state nad signal animation complete
                if (cardAnimator.GetCurrentAnimatorStateInfo(0).IsName("Default"))
                {
                    #if UNITY_EDITOR
                    Debug.Log("Trigger anim done");
                    #endif

                    currentSubState = AnimSubState.None;
                    OnCardAnimationFinished();
                }
                break;
            default:
                Debug.LogError("no active substate specified");
                break;
        }


    }
    public void UpdateHand(DeckManager.CardDecks handToUpdate, sPlayingCard newCard, bool flipLater)
    {
        //add card and assign it to the proper list, then start card movement via organize hand
        //Debug.Log("updating hand for list " + handToUpdate.ToString());
        switch (handToUpdate)
        {
            case DeckManager.CardDecks.player:
                playerCardObjects.Add(CreateCardGraphic(newCard, cardStartPosition, flipLater));
                break;
            case DeckManager.CardDecks.dealer:
                dealerCardObjects.Add(CreateCardGraphic(newCard, cardStartPosition, flipLater));
                break;
            case DeckManager.CardDecks.splitPlayer:
                playerSplitObjects.Add(CreateCardGraphic(newCard, cardStartPosition, flipLater));
                break;
            default:
                Debug.LogError("Invalid hand in UpdateHand");
                return;
        }

        OrganizeHand(handToUpdate);

    }

    private GameObject CreateCardGraphic(sPlayingCard cardToCreate, Vector3 createPosition, bool flipLater)
    {
        //create the required game object, set the values, turn it via the animator, then check if its needs to flip 
        //when it finishes or if it flips via discreet trigger later.

        #if UNITY_EDITOR
        Debug.Log("Creating card object");
        #endif

        GameObject cardInst = Instantiate(cardPrefab, createPosition, Quaternion.identity);
        cardInst.GetComponent<CardArtCreator>().AssignCardArt(cardToCreate.Rank, cardToCreate.Suit);
        cardInst.GetComponent<CardStatContainer>().SetValues(cardToCreate.Rank, cardToCreate.Suit);
        cardAnimator = cardInst.GetComponent<Animator>();

        //flip card over immediately when its created so its facing down
        cardAnimator.SetTrigger("TurnCard");
        if (flipLater)
        {
            if (animToFlipLater == null)
                animToFlipLater = cardAnimator;
            else
                Debug.LogError("AnimToFlipLater not cleared");
        }
        return cardInst;
    }

    public void OrganizeHand(DeckManager.CardDecks deck)
    {
        //set the home,offset and list information based on the deck, then populate the starting and target positions. 
        //Then turn on the state machine which will run the hand animations
        
        Vector3 handHome;
        Vector3 handOffset;
        List<GameObject> cardList;
        switch (deck)
        {
            case DeckManager.CardDecks.player:
                handHome = activeplayerHome;
                handOffset = activePlayerOffset;
                cardList = playerCardObjects;
                break;
            case DeckManager.CardDecks.dealer:
                handHome = dealerHandHome;
                handOffset = cardOffset;
                cardList = dealerCardObjects;
                break;
            case DeckManager.CardDecks.splitPlayer:
                handHome = split2Home;
                handOffset = splitOffset;
                cardList = playerSplitObjects;
                break;
            default:
                Debug.LogError("invalid hand supplied in organize hand");
                return;
        }

        for (int i = 0; i < cardList.Count; i++)
        {
            #if UNITY_EDITOR
            Debug.Log("moving card #" + i + ", in list " + deck.ToString());
            #endif

            animTargetPositions[i] = handHome + (handOffset * (i));
            animStartingPositions[i] = cardList[i].transform.position;
            animList = cardList;
        }

        //transition to start moving cards into position
        currentSubState = AnimSubState.CardsToPos;
    }

    private bool RunHandAnimations()
    {
        //for all the cards that need moved, find if their distance to target is greater than the acceptable margin. 
        //if yes, move the ones that need moved closer and return true. Otherwise return false.

        float distPerTick = moveSpeed * Time.deltaTime;
        bool moveDoneThisTick = false;
        for (int i = 0; i < animList.Count; i++)
        {
            float currDist = Mathf.Abs(Vector3.Distance(animList[i].transform.position, animTargetPositions[i]));
            //if the distance needed to move is greater than the margin, move them. 
            //but if the distance needed to travel is less than distance done in one frame, just set it to the required position.
            if (currDist > 0.001)
            {
                if (currDist - distPerTick < 0)
                    animList[i].transform.position = animTargetPositions[i];
                else
                    animList[i].transform.position += (animTargetPositions[i] - animList[i].transform.position).normalized * distPerTick;

                moveDoneThisTick = true;
            }
        }

        if (!moveDoneThisTick)
        {
            //Debug.Log("no cards found that need to move");
            return false;
        }
        else
        {
            return true;
        }
    }

    public void FlipCard()
    {
        //if card needs to be flipped later, this method will be called, at which
        //point the saved card will be triggered to flip, and the state machine will run then finish with an
        //animation finish event

        if (animToFlipLater != null)
        {
            #if UNITY_EDITOR
            Debug.Log("flipping card");
            #endif

            animToFlipLater.SetTrigger("FlipCard");
            cardAnimator = animToFlipLater;
            stateManager.soundManager.PlaySound(SoundManager.SoundType.Cardflip);
            currentSubState = AnimSubState.CardFlip;
        }
        else
        {
            Debug.LogError("NO CARD TO FLIP LATER");
        }
    }

    public void SplitCards()
    {
        //set up offset and home information for running the cards in a split formation

        activeplayerHome = split1Home;
        activePlayerOffset = splitOffset;

        GameObject temp = playerCardObjects[1];
        playerCardObjects.Remove(temp);
        playerSplitObjects.Add(temp);
    }

    public void EndSplit()
    {
        //reset offset and home information
        activePlayerOffset = cardOffset;
        activeplayerHome = playerHandHome;
        
    }

    public List<GameObject> GetSplitCards(int side)
    {
        //return the list of active cards (mostly for use in outlining cards)

        if (side == 0)
            return playerCardObjects;
        else return playerSplitObjects;
    }
}
