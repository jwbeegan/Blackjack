using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObjects/Deck", order = 2)]
public class sDeck : ScriptableObject
{
    public sPlayingCard[] CardArray;
}
