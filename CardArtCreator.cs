using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardArtCreator : MonoBehaviour
{
    [SerializeField] private Sprite heartSprite;
    [SerializeField] private Sprite clubSprite;
    [SerializeField] private Sprite spadeSprite;
    [SerializeField] private Sprite diamondSprite;

    [SerializeField] private Image topSymbol;
    [SerializeField] private Image middleSymbol;
    [SerializeField] private Image bottomSymbol;

    [SerializeField] private TextMeshPro topNumber;
    [SerializeField] private TextMeshPro bottomNumber;
    private string textSetting;

    [SerializeField] private bool test;
    [SerializeField] private int testNum;
    [SerializeField] private sPlayingCard.SuitEnum testSuit;

    private void Update()
    {
        if (test)
        {
            AssignCardArt(testNum, testSuit);
            test = false;
        }
    }
    public void AssignCardArt(int number, sPlayingCard.SuitEnum suit)
    {
        switch (suit) {

            case sPlayingCard.SuitEnum.Heart:
                topSymbol.sprite    = heartSprite;
                middleSymbol.sprite = heartSprite;
                bottomSymbol.sprite = heartSprite;
                break;

            case sPlayingCard.SuitEnum.Club:
                topSymbol.sprite    = clubSprite;
                middleSymbol.sprite = clubSprite;
                bottomSymbol.sprite = clubSprite;
                break;
            case sPlayingCard.SuitEnum.Diamond:
                topSymbol.sprite    = diamondSprite;
                middleSymbol.sprite = diamondSprite;
                bottomSymbol.sprite = diamondSprite;
                break;
            case sPlayingCard.SuitEnum.Spade:
                topSymbol.sprite    = spadeSprite;
                middleSymbol.sprite = spadeSprite;
                bottomSymbol.sprite = spadeSprite;
                break;
            default:
                Debug.LogError("Default reached in card art creator");
                break;

        }

        if ((number >= 2) && (number <= 10))
            textSetting = "" + number;
        else if (number == 1)
            textSetting = "A";
        else if (number == 11)
            textSetting = "J";
        else if (number == 12)
            textSetting = "Q";
        else if (number == 13)
            textSetting = "K";
        else
        {
            //set back to default T
            textSetting = "T";
            Debug.LogError("text setting not found");
        }

        topNumber.text = textSetting;
        bottomNumber.text = textSetting;


    }
}
