using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonManager : MonoBehaviour
{
    //button clicked event
    public delegate void ButtonClicked(ButtonType button);
    public static event ButtonClicked OnButtonClicked;

    [Header("Button references")]
    [SerializeField] private Button addCardButton;
    [SerializeField] private Button nextRoundButton;
    [SerializeField] private Button increaseBetButton;
    [SerializeField] private Button decreaseBetButton;
    [SerializeField] private Button startRoundButton;
    [SerializeField] private Button standButton;
    [SerializeField] private Button splitButton;
    [SerializeField] private Button doubleDownButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button endGameButton;


    public enum ButtonType {Hit = 1, NextRound = 2, StartRound = 3, IncreaseBet = 4, DecreaseBet = 5, Stand = 6, Split = 7, DoubleDown = 8, StartGame = 9, EndGame = 10, All = 11}
    public void ToggleButtonFullOn(ButtonType button)
    {
        #if UNITY_EDITOR
        Debug.Log("toggling button " + button.ToString() + " On");
        #endif

        ShowButton(button);
        EnableButton(button);
    }

    public void ToggleButtonFullOff(ButtonType button)
    {
        #if UNITY_EDITOR
        Debug.Log("toggling button " + button.ToString() + " Off");
        #endif

        DisableButton(button);
        HideButton(button);

    }

    //----------------Show Buttons
    public void ShowButton(ButtonType button)
    {
        #if UNITY_EDITOR
        Debug.Log("Calling Show Button for Button " + button.ToString());
        #endif

        Button tempButton = TranslateButtonEnum(button);

        if ((button != ButtonType.All) && (tempButton != null))
        { if (!tempButton.transform.gameObject.activeSelf) { tempButton.transform.gameObject.SetActive(true); } }
        else if (button == ButtonType.All)
        {
            for (int i = 1; i < (int)ButtonType.All; i++)
                if (!TranslateButtonEnum((ButtonType)i).transform.gameObject.activeSelf) { TranslateButtonEnum((ButtonType)i).transform.gameObject.SetActive(true); }
        }
    }

    //------------------Hide Buttons
    public void HideButton(ButtonType button)
    {
        #if UNITY_EDITOR
        Debug.Log("Calling Hide Button for Button " + button.ToString());
        #endif

        Button tempButton = TranslateButtonEnum(button);

        if ((button != ButtonType.All) && (tempButton != null))
        { if (tempButton.transform.gameObject.activeSelf) { tempButton.transform.gameObject.SetActive(false); } }
        else if (button == ButtonType.All)
        {
            for (int i = 1; i < (int)ButtonType.All; i++)
                if (TranslateButtonEnum((ButtonType)i).transform.gameObject.activeSelf) { TranslateButtonEnum((ButtonType)i).transform.gameObject.SetActive(false); }
        }
    }


    //------------------Enable Buttons
    public void EnableButton(ButtonType button)
    {
        #if UNITY_EDITOR
        Debug.Log("Calling Enable Button for Button " + button.ToString());
        #endif

        Button tempButton = TranslateButtonEnum(button);

        if ((button != ButtonType.All) && (tempButton != null))
        { if (!tempButton.interactable) { tempButton.interactable = true; } }
        else if (button == ButtonType.All)
        {
            for (int i = 1; i < (int)ButtonType.All; i++)
                if (TranslateButtonEnum((ButtonType)i).interactable) { TranslateButtonEnum((ButtonType)i).interactable = true; }
        }


    }



    //------------------Disable buttons
    public void DisableButton(ButtonType button)
    {
        #if UNITY_EDITOR
        Debug.Log("Calling Disable Button for Button " + button.ToString());
        #endif
        Button tempButton = TranslateButtonEnum(button);

        if ((button != ButtonType.All) && (tempButton != null))
        { if (tempButton.interactable) { tempButton.interactable = false; } }
        else if (button == ButtonType.All)
        {
            for (int i = 1; i < (int)ButtonType.All; i++)
                if (TranslateButtonEnum((ButtonType)i).interactable) { TranslateButtonEnum((ButtonType)i).interactable = false; }
        }

    }



    //-----------------Register Button Presses
    public void RegisterButton(int button)
    {
        #if UNITY_EDITOR
        Debug.Log("Activating Button " + TranslateUIButton(button).ToString());
        #endif

        OnButtonClicked(TranslateUIButton(button));
    }

    public static ButtonType TranslateUIButton(int buttonInt)
    {
        if ((buttonInt > 0) && (buttonInt < (int)ButtonType.All))
            return (ButtonType)buttonInt;
        else return 0;
    }

    private Button TranslateButtonEnum(ButtonType buttonEnum) //find a way to do all of them at once
    {
        #if UNITY_EDITOR
        Debug.Log("translating enum " + buttonEnum);
        #endif

        switch (buttonEnum)
        {
            case ButtonType.Hit:
                return addCardButton;

            case ButtonType.NextRound:
                return nextRoundButton;

            case ButtonType.IncreaseBet:
                return increaseBetButton;

            case ButtonType.DecreaseBet:
                return decreaseBetButton;

            case ButtonType.StartRound:
                return startRoundButton;

            case ButtonType.Stand:
                return standButton;

            case ButtonType.Split:
                return splitButton;

            case ButtonType.DoubleDown:
                return doubleDownButton;

            case ButtonType.StartGame:
                return startGameButton;

            case ButtonType.EndGame:
                return endGameButton;

            case ButtonType.All:
                return null;
            default:
                Debug.LogError("No button found in translate button enum");
                return null;
        }
    }


    

}
