using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetManager : MonoBehaviour
{
    [SerializeField] private GameStateManager stateManager;
    private int currentBet;
    private int playerMoney;
    private int splitBet;

    public delegate void BetUpdated(int bet, int splitBet, int playerTotal);
    public static event BetUpdated OnBetUpdated;



    private void OnEnable()
    {
        ButtonManager.OnButtonClicked += ButtonClick;

    }

    private void OnDisable()
    {
        ButtonManager.OnButtonClicked -= ButtonClick;

    }

    private void Start()
    {
        playerMoney = 50;
        OnBetUpdated(currentBet, splitBet, playerMoney);
    }

    private void ButtonClick(ButtonManager.ButtonType button)
    {
        if (button == ButtonManager.ButtonType.IncreaseBet)
            IncreaseBet();
        else if (button == ButtonManager.ButtonType.DecreaseBet)
            DecreaseBet();
    }

    public void ResetPlayerMoney()
    {
        playerMoney = 50;
        OnBetUpdated(currentBet, splitBet, playerMoney);
    }
    

    public void IncreaseBet()
    {
        if (playerMoney > 0)
        {
            currentBet += 5;
            playerMoney -= 5;
            OnBetUpdated(currentBet, splitBet, playerMoney);
            stateManager.chipSpawner.ResolveChips();
            stateManager.soundManager.PlaySound(SoundManager.SoundType.Chip);
        }
    }

    public void DecreaseBet()
    {
        if (currentBet > 0)
        {
            currentBet -= 5;
            playerMoney += 5;
            OnBetUpdated(currentBet, splitBet, playerMoney);
            stateManager.chipSpawner.ResolveChips();
            stateManager.soundManager.PlaySound(SoundManager.SoundType.Chip);
        }
    }

    public void WinPool()
    {
        playerMoney += currentBet*2;

        currentBet = 0;

        OnBetUpdated(currentBet, splitBet, playerMoney);
        stateManager.chipSpawner.ResolveChips();
    }

    public void LoseBet()
    {
        currentBet = 0;

        OnBetUpdated(currentBet, splitBet, playerMoney);
        stateManager.chipSpawner.ResolveChips();

    }

    public void SetBet(int amount)
    {
        currentBet += amount;
        playerMoney -= amount;
        OnBetUpdated(currentBet, splitBet, playerMoney);
        stateManager.chipSpawner.ResolveChips();
        stateManager.soundManager.PlaySound(SoundManager.SoundType.Chip);

    }

    public void ReturnBet()
    {
        playerMoney += currentBet;

        //return 1x chips

        currentBet = 0;
        OnBetUpdated(currentBet, splitBet, playerMoney);
        stateManager.chipSpawner.ResolveChips();
    }

    public void ReturnSplitBet()
    {
        playerMoney += splitBet;
        splitBet = 0;

        //return 1x chips
        OnBetUpdated(currentBet, splitBet, playerMoney);
        stateManager.chipSpawner.ResolveChips();

    }

    public void LoseSplitBet()
    {
        splitBet = 0;
        OnBetUpdated(currentBet, splitBet, playerMoney);
        stateManager.chipSpawner.ResolveChips();

    }

    public void WinSplitBet()
    {
        playerMoney += splitBet * 2;

        splitBet = 0;
        OnBetUpdated(currentBet, splitBet, playerMoney);
        stateManager.chipSpawner.ResolveChips();

    }

    public int GetCurrentBet()
    {
        return currentBet;
    }

    public int GetPlayerMoney()
    {
        return playerMoney;
    }

    public int GetSplitBet()
    {
        return splitBet;
    }

    public void Split()
    {
        splitBet = currentBet;
        playerMoney -= currentBet;

        OnBetUpdated(currentBet, splitBet, playerMoney);
        stateManager.chipSpawner.ResolveChips();
        stateManager.soundManager.PlaySound(SoundManager.SoundType.Chip);
    }

    public void DoubleDown()
    {
        playerMoney -= currentBet;
        currentBet += currentBet;

        OnBetUpdated(currentBet, splitBet, playerMoney);
        stateManager.chipSpawner.ResolveChips();
        stateManager.soundManager.PlaySound(SoundManager.SoundType.Chip);

    }

    public void DoubleDownSplitBet()
    {
        playerMoney -= splitBet;
        splitBet += splitBet;
        OnBetUpdated(currentBet, splitBet, playerMoney);
        stateManager.chipSpawner.ResolveChips();
        stateManager.soundManager.PlaySound(SoundManager.SoundType.Chip);

    }

    public bool CheckForSecondBetAllowed()
    {
        if (playerMoney >= currentBet)
            return true;
        else return false;
    }

    public bool CheckForSecondSplitBetAllowed()
    {
        if (playerMoney >= splitBet)
            return true;
        else return false;
    }
}
