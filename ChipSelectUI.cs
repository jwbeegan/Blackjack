using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChipSelectUI : MonoBehaviour
{
    [Header("Movement paremeters")]
    [SerializeField] private float startingPositionX;
    [SerializeField] private float moveInterval;
    [SerializeField] private int speed;
    [SerializeField] private int maxChipNum;
    [SerializeField] private float lerpMod;
    [SerializeField] private float selectedScale;
    [Space(10)]
    private int currentChipSelected=0;

    private bool movePosition;
    private Vector3 moveVector;
    private Vector3 moveTarget;

    [Header("Button/Script references")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Image darkImage;
    [SerializeField] private RectTransform[] chipRects;
    [SerializeField] private RectTransform chipParent;
    [SerializeField] private GameStateManager stateManager;
    [Space(10)]

    private RectTransform startingChipRect;
    private RectTransform endingChipRect;
    private int localPlayerMoney = 50;


    private void Update()
    {

        if (movePosition)
        {
            float diff = Vector3.Distance(chipParent.localPosition, moveTarget);
            float modifier = diff * lerpMod;
            float distPerTick = speed * Time.deltaTime * modifier;
            if (distPerTick < 0.2f)
                distPerTick = 0.2f;
            chipParent.localPosition += moveVector * distPerTick;

            float startSizeMod = 1 + diff*((selectedScale-1)/moveInterval);
            float endSizeMod = selectedScale + diff * ((1 - selectedScale) / moveInterval);
            startingChipRect.localScale = new Vector3(startSizeMod, startSizeMod, 1);
            endingChipRect.localScale = new Vector3(endSizeMod, endSizeMod, 1);

            if (Vector3.Distance(chipParent.localPosition, moveTarget) < 0.3)
            {
                chipParent.localPosition = moveTarget;
                startingChipRect.localScale = new Vector3(1, 1, 1);
                endingChipRect.localScale = new Vector3(selectedScale, selectedScale, 1);
                movePosition = false;

                #if UNITY_EDITOR
                Debug.Log("move done with current chip select = " + currentChipSelected);
                #endif
            }
        }


    }

    private void OnEnable()
    {
        ResetChipUIPosition();
        BetManager.OnBetUpdated += UpdateChips;
    }

    private void OnDisable()
    {
        BetManager.OnBetUpdated -= UpdateChips;

    }
    private void UpdateChips(int bet, int splitBet, int playerMoney)
    {
        localPlayerMoney = playerMoney;
    }


    public void MoveChipUILeft()
    {
        if ((!movePosition) && (currentChipSelected > 0))
        {
            startingChipRect = chipRects[currentChipSelected];
            currentChipSelected -= 1;
            endingChipRect = chipRects[currentChipSelected];
            LerpPosition(new Vector3(chipParent.localPosition.x + moveInterval, chipParent.localPosition.y, chipParent.localPosition.z));
            CheckLRButtons();
        }
    }

    public void MoveChipUIRight()
    {
        if ((!movePosition) && (currentChipSelected < maxChipNum-1))
        {
            startingChipRect = chipRects[currentChipSelected];
            currentChipSelected += 1;
            endingChipRect = chipRects[currentChipSelected];
            LerpPosition(new Vector3(chipParent.localPosition.x - moveInterval, chipParent.localPosition.y, chipParent.localPosition.z));
            CheckLRButtons();
        }

    }

    public void ResetChipUIPosition()
    {

        #if UNITY_EDITOR
        Debug.Log("Reseting chip UI position");
        #endif

        currentChipSelected = 0;
        chipParent.localPosition = new Vector3(startingPositionX, chipParent.localPosition.y, chipParent.localPosition.z);

        for (int i = 0; i < maxChipNum; i++)
        {
            chipRects[i].localScale = new Vector3(1, 1, 1);
            chipRects[0].localScale = new Vector3(selectedScale, selectedScale, 1);
        }
    }

    public void EnableChipSelect()
    {
        chipParent.gameObject.SetActive(true);
        darkImage.gameObject.SetActive(true);
        CheckLRButtons();
    }

    public void DisableChipSelect()
    {
        chipParent.gameObject.SetActive(false);
        leftButton.gameObject.SetActive(false);
        rightButton.gameObject.SetActive(false);
        darkImage.gameObject.SetActive(false);

    }

    private void CheckLRButtons()
    {
        //check against both the maximum/minimum chip to start, but then against the max money you have
        if (currentChipSelected > 0)
            leftButton.gameObject.SetActive(true);
        else
            leftButton.gameObject.SetActive(false);

        //if selecting max chip dont set true, just set false
        if (currentChipSelected < maxChipNum-1)
        {
            //get current chip selected value
            int nextChipValue = (int)stateManager.chipSpawner.GetChipTypeFromArrayIndex(currentChipSelected + 1);

            if ((currentChipSelected < maxChipNum - 1) && (localPlayerMoney >= nextChipValue))
                rightButton.gameObject.SetActive(true);
            else
                rightButton.gameObject.SetActive(false);
        }
        else
            rightButton.gameObject.SetActive(false);
    }

    public int GetChipSelected()
    {
        return currentChipSelected;
    }

    private void LerpPosition(Vector3 target)
    {
        //determine movement vector
        moveVector = (target - chipParent.localPosition).normalized;
        moveTarget = target;
        //startmovement
        movePosition = true;

    }


}
