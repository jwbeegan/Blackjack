using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardStatContainer : MonoBehaviour
{
    public int cardNumber;
    public sPlayingCard.SuitEnum cardSuit;

    public void SetValues(int number, sPlayingCard.SuitEnum suit)
    {
        cardNumber = number;
        cardSuit = suit;
    }
}
