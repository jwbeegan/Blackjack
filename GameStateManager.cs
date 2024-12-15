using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public enum GameEventTriggers {DealerScoreUpdated, PlayerScoreUpdated, AddCardClicked, AnimationFinish, DealerCalled, NextRoundClicked, StartRoundClicked, BetsUpdated, 
        StandClicked, SplitClicked, DoubleDownClicked, StartGameClicked, EndGameClicked}
    public GameState activeState;
    private bool split;
    [SerializeField] private int splitProgression;

    //references for all the other scripts 
    [Header("references to non-state scripts")]
    public ScoreManager scoreManager;
    public BetManager betManager;
    public ButtonManager buttonManager;
    public TextManager textManager;
    public CardGraphicUpdater graphicUpdater;
    public DeckManager deckManager;
    public ChipSpawner chipSpawner;
    public SoundManager soundManager;
    public LayerChanger layerChanger;
    public ChipSelectUI chipSelector;
    [Space(10)]

    [Header("references to game states")]
    public GameState gsAnim;
    public GameState gsBet;
    public GameState gsMenu;
    public GameState gsInitialDeal;
    public GameState gsPlayerMove;
    public GameState gsDealerMove;
    public GameState gsTie;
    public GameState gsSplit;
    public GameState gsSplitResolve;
    public GameState gsPlayerLoss;
    public GameState gsPlayerWin;
    public GameState gsDoubleDownSingle;
    public GameState gsDoubleDownSplit;



    private void OnEnable()
    {
        ScoreManager.OnDealerScoreUpdated           += DealerScoreUpdateEvent;
        ScoreManager.OnPlayerScoreUpdated           += PlayerScoreUpdateEvent;
        CardGraphicUpdater.OnCardAnimationFinished  += CardAnimationFinishEvent;
        BetManager.OnBetUpdated                     += BetsUpdatedEvent;
        ButtonManager.OnButtonClicked               += ButtonClick;

    }

    private void Start()
    {
        //switch to state on start since it references another script
        activeState = activeState.SwitchToThisState();

    }

    private void OnDisable()
    {
        ScoreManager.OnDealerScoreUpdated           -= DealerScoreUpdateEvent;
        ScoreManager.OnPlayerScoreUpdated           -= PlayerScoreUpdateEvent;
        CardGraphicUpdater.OnCardAnimationFinished  -= CardAnimationFinishEvent;
        BetManager.OnBetUpdated                     -= BetsUpdatedEvent;
        ButtonManager.OnButtonClicked               -= ButtonClick;

    }

    private void ButtonClick(ButtonManager.ButtonType button)
    {
        if (button == ButtonManager.ButtonType.Hit)
            AddCardClickedEvent();
        else if (button == ButtonManager.ButtonType.NextRound)
            NextRoundEvent();
        else if (button == ButtonManager.ButtonType.StartRound)
            StartRoundEvent();
        else if (button == ButtonManager.ButtonType.Stand)
            StandClickedEvent();
        else if (button == ButtonManager.ButtonType.Split)
            SplitClickedEvent();
        else if (button == ButtonManager.ButtonType.DoubleDown)
            DoubleDownClickedEvent();
        else if (button == ButtonManager.ButtonType.StartGame)
            StartGameClickedEvent();
        else if (button == ButtonManager.ButtonType.EndGame)
            EndGameClickedEvent();

    }


    private void DealerCallEvent()
    {
        #if UNITY_EDITOR
        Debug.Log("dealer Called Event");
        #endif
        activeState.RegisterTrigger(GameEventTriggers.DealerCalled);
    }

    private void BetsUpdatedEvent(int bet, int splitBet, int total)
    {
        #if UNITY_EDITOR
        Debug.Log("Bets Resolved Event");
        #endif
        activeState.RegisterTrigger(GameEventTriggers.BetsUpdated);

    }

    private void CardAnimationFinishEvent()
    {
        #if UNITY_EDITOR
        Debug.Log("Card Animation Finish event");
        #endif
        activeState.RegisterTrigger(GameEventTriggers.AnimationFinish);
    }
    private void PlayerScoreUpdateEvent(int score)
    {
        #if UNITY_EDITOR
        Debug.Log("Player Score Update Event");
        #endif
        activeState.RegisterTrigger(GameEventTriggers.PlayerScoreUpdated);
    }

    private void AddCardClickedEvent()
    {
        #if UNITY_EDITOR
        Debug.Log("Add Card Clicked Event");
        #endif
        activeState.RegisterTrigger(GameEventTriggers.AddCardClicked);
    }

    private void DealerScoreUpdateEvent(int score)
    {
        #if UNITY_EDITOR
        Debug.Log("Dealer Score Update Event");
        #endif
        activeState.RegisterTrigger(GameEventTriggers.DealerScoreUpdated);
    }

    private void NextRoundEvent()
    {
        #if UNITY_EDITOR
        Debug.Log("Next Round Event");
        #endif
        activeState.RegisterTrigger(GameEventTriggers.NextRoundClicked);
    }

    private void StartRoundEvent()
    {
        #if UNITY_EDITOR
        Debug.Log("Start Round Event");
        #endif
        activeState.RegisterTrigger(GameEventTriggers.StartRoundClicked);
    }

    private void StandClickedEvent()
    {
        #if UNITY_EDITOR
        Debug.Log("Stand Clicked Event");
        #endif
        activeState.RegisterTrigger(GameEventTriggers.StandClicked);
    }

    private void SplitClickedEvent()
    {
        #if UNITY_EDITOR
        Debug.Log("Split Clicked Event");
        #endif
        activeState.RegisterTrigger(GameEventTriggers.SplitClicked);
    }

    private void DoubleDownClickedEvent()
    {
        activeState.RegisterTrigger(GameEventTriggers.DoubleDownClicked);
    }

    private void StartGameClickedEvent()
    {
        activeState.RegisterTrigger(GameEventTriggers.StartGameClicked);

    }

    private void EndGameClickedEvent()
    {
        activeState.RegisterTrigger(GameEventTriggers.EndGameClicked);

    }

    public void RegisterSplit()
    {
        split = true;
    }

    public void ResetSplit()
    {
        split = false;
        splitProgression = 0;
    }

    public bool IsSplitHand()
    {
        return split;
    }

    public int CheckSplitProgression()
    {
        return splitProgression;
    }

    public void IncrementSplitProgression()
    {

        #if UNITY_EDITOR
        Debug.Log("incrementing split progression");
        #endif

        splitProgression += 1;
    }

}
