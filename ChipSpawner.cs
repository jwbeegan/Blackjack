using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipSpawner : MonoBehaviour
{
    [Header("State manager reference")]
    [SerializeField] private GameStateManager stateManager;
    [Space(10)]

    [Header("chip spawn locations")]
    [SerializeField] private Vector3 whiteLoc;
    [SerializeField] private Vector3 whiteLocSplitR;
    [SerializeField] private Vector3 whiteLocSplitL;
    [SerializeField] private float chipSpawnSpacing;
    [Space(10)]

    [Header("Chip prefabs")]
    [SerializeField] private GameObject whiteChip;
    [SerializeField] private GameObject redChip;
    [SerializeField] private GameObject blueChip;
    [SerializeField] private GameObject greyChip;
    [SerializeField] private GameObject greenChip;
    [SerializeField] private GameObject orangeChip;
    [SerializeField] private GameObject blackChip;
    [Space(10)]

    //chip spawn tracking
    private int[,] chipNumSpawned;
    private GameObject[,,] chipObjectArr;

    //local references to the active bet and split bet
    private int localBet;
    private int localsplit;
    private const int numberChipTypes = 7;

    //enums for chip type and board side
    public enum BoardSide { Bet = 0, Player = 1, SplitRight = 2, SplitLeft = 3 };
    public enum ChipType { White = 1, Red = 5, Blue = 10, Grey = 20, Green = 25, Orange = 50, Black = 100 }

    private void Awake()
    {
        #if UNITY_EDITOR
        Debug.Log("setting up chip num spawned");
        #endif
        chipNumSpawned = new int[4, numberChipTypes];
        chipObjectArr = new GameObject[4, numberChipTypes, 100];

    }

    private void OnEnable()
    {
        BetManager.OnBetUpdated += UpdateChips;
    }

    private void OnDisable()
    {
        BetManager.OnBetUpdated -= UpdateChips;

    }

    public int GetChipArrayIndex(ChipType type)
    {
        switch (type)
        {
            case ChipType.White:
                return 0;
            case ChipType.Red:
                return 1;
            case ChipType.Blue:
                return 2;
            case ChipType.Grey:
                return 3;
            case ChipType.Green:
                return 4;
            case ChipType.Orange:
                return 5;
            case ChipType.Black:
                return 6;
            default:
                Debug.LogError("ERROR: INVLAID CHIP IN GET Chip index");
                return 0;
        }

    }

    public ChipType GetChipTypeFromArrayIndex(int index)
    {
        //Debug.Log("getting chip index " + index);
        switch (index)
        {
            case 0:
                return ChipType.White;
            case 1:
                return ChipType.Red;
            case 2:
                return ChipType.Blue;
            case 3:
                return ChipType.Grey;
            case 4:
                return ChipType.Green;
            case 5:
                return ChipType.Orange;
            case 6:
                return ChipType.Black;
            default:
                Debug.LogError("ERROR: INVLAID INDEX IN GET CHIP FROM INDEX");
                return 0;
        }

    }

    private void UpdateChips(int bet, int splitBet, int playerMoney)
    {
        localBet = bet;
        localsplit = splitBet;
    }

    public void ResolveChips()
    {
        //make it so each time it just calculates the optimal chips, then changes based on whats needed
        int existingBetValue = SumActiveChips(chipNumSpawned, BoardSide.Bet);
        int existingSplitLeftValue = SumActiveChips(chipNumSpawned, BoardSide.SplitLeft);
        int existingSplitRightValue = SumActiveChips(chipNumSpawned, BoardSide.SplitRight);

        #if UNITY_EDITOR
        Debug.Log("resolving chips with bet money = " + existingBetValue + ", new bet money = " + localBet);
        Debug.Log("resolving chips with Split left money = " + existingSplitLeftValue + ", new left split money = " + localBet);
        Debug.Log("resolving chips with Split right money = " + existingSplitRightValue + ", new right split money = " + localsplit);
        #endif

        if (!stateManager.IsSplitHand())
        {
            if (existingBetValue != localBet)
            {
                //if there are already chips on the board, double down is only valid method by which chips are added, so use the via add method
                //if resetting the bet, use the overall resolve method
                if ((existingBetValue == 0) || (localBet == 0))
                {
                    //find ideal construction then find difference in chips, then add or remove that many chips
                    ResolveChipDifference(DecomposeIntoChips(localBet), BoardSide.Bet);
                }
                else
                { 
                    //find out how much to add then add
                    int amountToAdd = localBet - existingBetValue;
                    ResolveChipViaAdd(DecomposeIntoChips(amountToAdd), BoardSide.Bet);
                }

                ResolveChipDifference(DecomposeIntoChips(0), BoardSide.SplitLeft);
                ResolveChipDifference(DecomposeIntoChips(0), BoardSide.SplitRight);
            }
        }
        else
        {
            ResolveChipDifference(DecomposeIntoChips(0), BoardSide.Bet);
            if (existingSplitLeftValue != localBet)
            {
                if ((existingSplitLeftValue == 0) || (localBet == 0))
                {
                    //find ideal construction then find difference in chips, then add or remove that many chips
                    ResolveChipDifference(DecomposeIntoChips(localBet), BoardSide.SplitLeft);
                }
                else
                {
                    int amountToAdd = localBet - existingSplitLeftValue;
                    ResolveChipViaAdd(DecomposeIntoChips(amountToAdd), BoardSide.SplitLeft);

                }

            }
            if (existingSplitRightValue != localsplit)
            {
                if ((existingSplitRightValue == 0) || (localsplit == 0))
                {
                    //find ideal construction then find difference in chips, then add or remove that many chips
                    ResolveChipDifference(DecomposeIntoChips(localsplit), BoardSide.SplitRight);
                }
                else
                {
                    int amountToAdd = localsplit - existingSplitRightValue;
                    ResolveChipViaAdd(DecomposeIntoChips(amountToAdd), BoardSide.SplitRight);

                }

            }
        }
        
    }


    private void ResolveChipDifference(int[] newChipArr, BoardSide side)
    {
        #if UNITY_EDITOR
        Debug.Log("resolving chips for side " + side.ToString());
        #endif

        for (int i = 0; i < numberChipTypes; i++)
        {
            int diff = newChipArr[i] - chipNumSpawned[(int)side,i];
            
            //need to add chips
            if (diff > 0)
            {
                SpawnChips(diff, GetChipTypeFromArrayIndex(i), side);
            }
            //need to remove chips
            else if (diff < 0)
            {
                RemoveChips(-diff, GetChipTypeFromArrayIndex(i), side);
            }
        }

        return;
    }

    private void ResolveChipViaAdd(int[] newChipArr, BoardSide side)
    {
        #if UNITY_EDITOR
        Debug.Log("adding chips for side " + side.ToString());
        #endif

        for (int i = 0; i < numberChipTypes; i++)
        {
            SpawnChips(newChipArr[i], GetChipTypeFromArrayIndex(i), side);
        }

        return;

    }

    private void SpawnChips(int amount, ChipType type, BoardSide side) 
    {
        #if UNITY_EDITOR
        Debug.Log("Spawning " + amount + " of " + type.ToString() + " chips on side " + side.ToString());
        #endif        
        for (int i = 0; i < amount; i++)
        {
            //get index, spawn location
            int chipIndex = GetChipArrayIndex(type);
            float spawnOffset = chipNumSpawned[(int)side, chipIndex] * chipSpawnSpacing;
            Vector3 spawnLoc = getChipSpawnLocation(side) + new Vector3(0, spawnOffset, -spawnOffset);

            //then create chip and assign it into the object array
            chipObjectArr[(int)side, chipIndex, chipNumSpawned[(int)side,chipIndex]] = Instantiate(getChipPrefab(type), spawnLoc, Quaternion.identity);

            //and track that one has been spawned
            chipNumSpawned[(int) side,chipIndex] += 1;

            #if UNITY_EDITOR
            Debug.Log(" after spawn chipIndex = " + chipIndex + ", num spawned = " + chipNumSpawned[(int)side, chipIndex]);
            #endif
        }

    }

    private void RemoveChips(int amount, ChipType type, BoardSide side)
    {
        #if UNITY_EDITOR
        Debug.Log("removing " + amount + " of " + type.ToString() + " chips from " + side.ToString());
        #endif

        for (int i = 0; i < amount; i++)
        {
            int chipIndex = GetChipArrayIndex(type);

            //destroy largest index of type and side selected
            Destroy(chipObjectArr[(int)side, chipIndex, chipNumSpawned[(int)side,chipIndex]-1]);

            //then set the reference to null
            chipObjectArr[(int)side, chipIndex, chipNumSpawned[(int)side, chipIndex] - 1] = null;

            //then track that there is now one less
            chipNumSpawned[(int)side, chipIndex] -= 1;
        }
    }


    private int[] DecomposeIntoChips(int amount)
    {
        int[] returnArr = new int[7];
        var moduloResult = CheckChipModulo((int)ChipType.Black, amount);
        returnArr[6] = moduloResult.Item1;
        moduloResult = CheckChipModulo((int)ChipType.Orange, moduloResult.Item2);
        returnArr[5] = moduloResult.Item1;
        moduloResult = CheckChipModulo((int)ChipType.Green, moduloResult.Item2);
        returnArr[4] = moduloResult.Item1;
        moduloResult = CheckChipModulo((int)ChipType.Grey, moduloResult.Item2);
        returnArr[3] = moduloResult.Item1;
        moduloResult = CheckChipModulo((int)ChipType.Blue, moduloResult.Item2);
        returnArr[2] = moduloResult.Item1;
        moduloResult = CheckChipModulo((int)ChipType.Red, moduloResult.Item2);
        returnArr[1] = moduloResult.Item1;
        moduloResult = CheckChipModulo((int)ChipType.White, moduloResult.Item2);
        returnArr[0] = moduloResult.Item1;

        return returnArr;
    }

    private int SumActiveChips(int[,] chipArr, BoardSide side)
    {
        int result = 0;
        for (int i = 0; i < numberChipTypes; i++)
        {
            #if UNITY_EDITOR
            Debug.Log("sum active chips i = " + i + ", side = " + (int)side);
            #endif

            result += chipArr[(int)side, i] *(int)GetChipTypeFromArrayIndex(i);
        }

        return result;
    }

    //returns (# chips found, remaining amount)
    private (int, int) CheckChipModulo(int chipValue, int amount)
    {
        //Debug.Log("runniing chip modulo for value " + chipValue + " and amount " + amount);
        int chipsInAmount = amount/chipValue;
        if (chipsInAmount > 0)
        {
            return (chipsInAmount, amount - (chipsInAmount * chipValue));
        }
        else
        {
            return (0, amount);
        }
    }

    private Vector3 getChipSpawnLocation(BoardSide side)
    {
        if (side == BoardSide.Bet)
        {
            return whiteLoc;
        }
        else if (side == BoardSide.Player)
        {
            return Vector3.zero;
        }
        else if (side == BoardSide.SplitLeft)
        {
            return whiteLocSplitL;
        }
        else if (side == BoardSide.SplitRight)
        {
            return whiteLocSplitR;
        }
        else
        {
            Debug.LogError("ERROR: INVLAID BOARD SIDE IN GET CHIP SPAWN LOCATION");
            return Vector3.zero;

        }
    }

    private GameObject getChipPrefab(ChipType type)
    {
        switch (type)
        {
            case ChipType.White:
                return whiteChip;
            case ChipType.Red:
                return redChip;
            case ChipType.Blue:
                return blueChip;
            case ChipType.Grey:
                return greyChip;
            case ChipType.Green:
                return greenChip;
            case ChipType.Orange:
                return orangeChip;
            case ChipType.Black:
                return blackChip;
            default:
                Debug.LogError("ERROR: INVLAID CHIP IN GET CIP PREFAB");
                return null;
        }
    }

}
