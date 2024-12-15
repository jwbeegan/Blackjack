using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/PlayingCard", order = 1)]
public class sPlayingCard : ScriptableObject
{
    public int Rank;
    public enum SuitEnum {Heart, Spade, Club, Diamond};
    public SuitEnum Suit;
}
