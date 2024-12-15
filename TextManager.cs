using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextManager : MonoBehaviour //really is a action state text manager so rename. 
{
    [Header("Text references")]
    [SerializeField] private TextMeshProUGUI loserText;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI tieText;
    [SerializeField] private TextMeshProUGUI dealerHit;
    [SerializeField] private TextMeshProUGUI dealerStand;
    [SerializeField] private TextMeshProUGUI playerScore;
    [SerializeField] private TextMeshProUGUI dealerScore;
    [SerializeField] private TextMeshProUGUI roundBet;
    [SerializeField] private TextMeshProUGUI playerMoney;
    [SerializeField] private TextMeshProUGUI roundBetText;
    [SerializeField] private TextMeshProUGUI playerMoneyText;
    [SerializeField] private TextMeshProUGUI playerSplit1Score;
    [SerializeField] private TextMeshProUGUI playerSplit2Score;
    [SerializeField] private TextMeshProUGUI playerSplitBet;
    [SerializeField] private TextMeshProUGUI playerSplitBetText;
    [SerializeField] private TextMeshProUGUI winTieText;
    [SerializeField] private TextMeshProUGUI loseTieText;
    [SerializeField] private TextMeshProUGUI winLoseText;
    [SerializeField] private TextMeshProUGUI titleText1;
    [SerializeField] private TextMeshProUGUI titleText2;
    public enum TextEnum {Loser = 1, Winner = 2, Tie = 3, DealerHit = 4, DealerStand = 5, PlayerScore = 6, DealerScore = 7, RoundBet = 8, PlayerMoney = 9, RoundBetText = 10, PlayerMoneyText = 11,
        SplitScore1 = 12, SplitScore2 = 13, SplitBet =14, SplitBetText = 15, WinTie = 16, LoseTie = 17, WinLose = 18, Title1=19, Title2 = 20, All = 21}

    public enum Defaults {MoneyScore = 1, JustMoney = 2, SplitMoneyScore = 3}


    private void OnEnable()
    {
        BetManager.OnBetUpdated += OnBetUpdated;
    }

    private void OnDisable()
    {
        BetManager.OnBetUpdated -= OnBetUpdated;

    }

    private void OnBetUpdated(int currentBet, int splitBet, int playerMoney)
    {
        UpdateText(TextEnum.RoundBet, "" + currentBet);
        UpdateText(TextEnum.SplitBet, "" + splitBet);
        UpdateText(TextEnum.PlayerMoney, "" + playerMoney);

    }


    public void UpdateText(TextEnum textToChange, string textToUse)
    {
        #if UNITY_EDITOR
        Debug.Log("Updating text " + textToChange.ToString() + " to " + textToUse);
        #endif
        
        TranslateTextEnum(textToChange).text = textToUse;
    }

    public void DisableText(TextEnum textToDisable)
    {
        #if UNITY_EDITOR
        Debug.Log("Disabling text " + textToDisable.ToString());
        #endif

        if (textToDisable != TextEnum.All)
            TranslateTextEnum(textToDisable).gameObject.SetActive(false);
        else
        {
            for (int i = 1; i < (int)TextEnum.All; i++)
            {
                TranslateTextEnum((TextEnum)i).gameObject.SetActive(false);
            }
        }
    }

    public void EnableText(TextEnum textToEnable)
    {
        #if UNITY_EDITOR
        Debug.Log("Enabling text " + textToEnable.ToString());
        #endif
        
        if (textToEnable != TextEnum.All)
            TranslateTextEnum(textToEnable).gameObject.SetActive(true);
        else
        {
            for (int i = 1; i < (int)TextEnum.All; i++)
            {
                TranslateTextEnum((TextEnum)i).gameObject.SetActive(true);
            }
        }
    }


    public TextMeshProUGUI TranslateTextEnum(TextEnum textEnum)
    {
        switch(textEnum)
        {
            case TextEnum.Loser:
                return loserText;
            case TextEnum.Winner:
                return winnerText;
            case TextEnum.Tie:
                return tieText;
            case TextEnum.DealerHit:
                return dealerHit;
            case TextEnum.DealerStand:
                return dealerStand;
            case TextEnum.PlayerScore:
                return playerScore;
            case TextEnum.DealerScore:
                return dealerScore;
            case TextEnum.RoundBet:
                return roundBet;
            case TextEnum.PlayerMoney:
                return playerMoney;
            case TextEnum.RoundBetText:
                return roundBetText;
            case TextEnum.PlayerMoneyText:
                return playerMoneyText;
            case TextEnum.SplitScore1:
                return playerSplit1Score;
            case TextEnum.SplitScore2:
                return playerSplit2Score;
            case TextEnum.SplitBet:
                return playerSplitBet;
            case TextEnum.SplitBetText:
                return playerSplitBetText;
            case TextEnum.WinTie:
                return winTieText;
            case TextEnum.LoseTie:
                return loseTieText;
            case TextEnum.WinLose:
                return winLoseText;
            case TextEnum.Title1:
                return titleText1;
            case TextEnum.Title2:
                return titleText2;
            case TextEnum.All:
                Debug.LogError("Attempted to translate all text enum");
                return null;
            default:
                Debug.LogError("NO TEXT FOUND WHEN CONVERTING TEXT ENUM");
                return null;
        }
    }

    public void SetupDefaultText(Defaults type) //change to enum later
    {
        switch (type)
        {
            case Defaults.MoneyScore:
                EnableText(TextEnum.PlayerMoneyText);
                EnableText(TextEnum.PlayerMoney);
                EnableText(TextEnum.PlayerScore);
                EnableText(TextEnum.DealerScore);

                return;
            case Defaults.JustMoney:
                EnableText(TextEnum.PlayerMoneyText);
                EnableText(TextEnum.PlayerMoney);
                return;
            case Defaults.SplitMoneyScore:
                EnableText(TextEnum.PlayerMoneyText);
                EnableText(TextEnum.PlayerMoney);
                EnableText(TextEnum.SplitScore2);
                EnableText(TextEnum.SplitScore1);
                EnableText(TextEnum.DealerScore);
                return;
        }
    }

}
