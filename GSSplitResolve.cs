using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GSSplitResolve : GameState
{
    [SerializeField] private GameStateManager stateManager;

    private int splitScore1;
    private int splitScore2;
    private int dealerScore;

    private int split1Result;
    private int split2Result;
    private int totalResult;

    //win = 1, lose = 2, tie = 3
    private enum SplitScoreResult {FWinSWin = 11, FWinSLose = 12, FWinSTie = 13, FLoseSWin = 21, FLoseSLose = 22, FLoseSTie = 23, FTieSWin = 31, FTieSLose = 32, FTieSTie = 33 }

    public override GameState SwitchToThisState()
    {

        #if UNITY_EDITOR
        Debug.Log("switching to state split resolve");
        #endif

        //enable buttons and text
        stateManager.buttonManager.ToggleButtonFullOff(ButtonManager.ButtonType.All);
        stateManager.textManager.DisableText(TextManager.TextEnum.All);
        stateManager.textManager.SetupDefaultText(TextManager.Defaults.SplitMoneyScore);

        splitScore1 = stateManager.scoreManager.GetPlayerScore();
        splitScore2 = stateManager.scoreManager.GetSplitScore();
        dealerScore = stateManager.scoreManager.GetDealerScore();

        stateManager.activeState = this;

        split1Result = CheckScore(splitScore1, dealerScore);

        split2Result = CheckScore(splitScore2, dealerScore);

        totalResult = (split1Result * 10) + split2Result;

        #if UNITY_EDITOR
        Debug.Log("split result = " + totalResult);
        #endif

        stateManager.soundManager.PlaySound(SoundManager.SoundType.Returnchips);

        switch((SplitScoreResult)totalResult)
        {
            case SplitScoreResult.FWinSWin:
                #if UNITY_EDITOR
                Debug.Log("player wins hand 1 and 2");
                #endif

                //win hand 1
                stateManager.betManager.WinPool();

                //wind hand 2
                stateManager.betManager.WinSplitBet();

                //display win/win
                stateManager.textManager.EnableText(TextManager.TextEnum.Winner);

                break;
            case SplitScoreResult.FWinSLose:
                #if UNITY_EDITOR
                Debug.Log("player wins hand 1, loses 2");
                #endif

                //win hand 1
                stateManager.betManager.WinPool();

                //wind hand 2
                stateManager.betManager.LoseSplitBet();

                //display win/win
                stateManager.textManager.EnableText(TextManager.TextEnum.WinLose);

                break;
            case SplitScoreResult.FWinSTie:
                #if UNITY_EDITOR
                Debug.Log("player wins hand 1, ties 2");
                #endif

                //win hand 1
                stateManager.betManager.WinPool();

                //wind hand 2
                stateManager.betManager.ReturnSplitBet();

                //display win/win
                stateManager.textManager.EnableText(TextManager.TextEnum.WinTie);

                break;
            case SplitScoreResult.FLoseSWin:
                #if UNITY_EDITOR
                Debug.Log("player loses hand 1, wins 2");
                #endif

                //win hand 1
                stateManager.betManager.LoseBet();

                //wind hand 2
                stateManager.betManager.WinSplitBet();

                //display win/win
                stateManager.textManager.EnableText(TextManager.TextEnum.WinLose);

                break;
            case SplitScoreResult.FLoseSLose:
                #if UNITY_EDITOR
                Debug.Log("player loses hand 1 and 2");
                #endif

                //win hand 1
                stateManager.betManager.LoseBet();

                //wind hand 2
                stateManager.betManager.LoseSplitBet();

                //display win/win
                stateManager.textManager.EnableText(TextManager.TextEnum.Loser);

                break;
            case SplitScoreResult.FLoseSTie:
                #if UNITY_EDITOR
                Debug.Log("player loses hand 1, ties 2");
                #endif

                //win hand 1
                stateManager.betManager.LoseBet();

                //wind hand 2
                stateManager.betManager.ReturnSplitBet();

                //display win/win
                stateManager.textManager.EnableText(TextManager.TextEnum.LoseTie);

                break;
            case SplitScoreResult.FTieSWin:
                
                #if UNITY_EDITOR
                Debug.Log("player tie 1, wins 2");
                #endif

                //win hand 1
                stateManager.betManager.ReturnBet();

                //wind hand 2
                stateManager.betManager.WinSplitBet();

                //display win/win
                stateManager.textManager.EnableText(TextManager.TextEnum.WinLose);

                break;
            case SplitScoreResult.FTieSLose:
                #if UNITY_EDITOR
                Debug.Log("player ties hand 1, loses 2");
                #endif

                //win hand 1
                stateManager.betManager.ReturnBet();

                //wind hand 2
                stateManager.betManager.LoseSplitBet();

                //display win/win
                stateManager.textManager.EnableText(TextManager.TextEnum.LoseTie);

                break;
            case SplitScoreResult.FTieSTie:
                #if UNITY_EDITOR
                Debug.Log("player ties hand 1, ties 2");
                #endif

                //win hand 1
                stateManager.betManager.ReturnBet();

                //wind hand 2
                stateManager.betManager.ReturnSplitBet();

                //display win/win
                stateManager.textManager.EnableText(TextManager.TextEnum.Tie);
                break;
            default: break;
        }

        if (stateManager.betManager.GetPlayerMoney() <= 0)
        {
            //turn on the end game button if you run out of money at the end of a round
            stateManager.buttonManager.ToggleButtonFullOn(ButtonManager.ButtonType.EndGame);
        }
        else
        {
            //otherwise turn on the next round button
            stateManager.buttonManager.ToggleButtonFullOn(ButtonManager.ButtonType.NextRound);

        }

        return this;
    }

    public override GameState RegisterTrigger(GameStateManager.GameEventTriggers trigger)
    {
        switch (trigger)
        {
            case GameStateManager.GameEventTriggers.NextRoundClicked:
                stateManager.ResetSplit();
                stateManager.graphicUpdater.EndSplit();
                return stateManager.gsBet.SwitchToThisState();

            case GameStateManager.GameEventTriggers.EndGameClicked:
                stateManager.ResetSplit();
                stateManager.graphicUpdater.EndSplit();
                return stateManager.gsMenu.SwitchToThisState();
            default: return this;
        }
    }



    private int CheckScore(int playerScore, int dealerScore)
    {
        if (dealerScore <= 21)
        {
            if (playerScore <= 21)
            {
                if (playerScore > dealerScore)
                {
                    return 1;
                }
                else if (playerScore < dealerScore)
                {
                    return 2;
                }
                else
                {
                    return 3;
                }
            }
            else
            {
                return 2;
            }
        }
        else
        {
            if (playerScore <= 21)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
    }
}
