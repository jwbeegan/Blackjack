using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GSMenu : GameState
{
    [SerializeField] private GameStateManager stateManager;
    //what does the state do when it enters

    [SerializeField] private Image chipImage;
    [SerializeField] private TextMeshProUGUI moneySymbol;
    public override GameState SwitchToThisState()
    {

        #if UNITY_EDITOR
        Debug.Log("switching to menu state");
        #endif
        
        stateManager.buttonManager.ToggleButtonFullOff(ButtonManager.ButtonType.All);
        stateManager.textManager.DisableText(TextManager.TextEnum.All);


        stateManager.buttonManager.ToggleButtonFullOn(ButtonManager.ButtonType.StartGame);
        stateManager.textManager.EnableText(TextManager.TextEnum.Title1);
        stateManager.textManager.EnableText(TextManager.TextEnum.Title2);


        stateManager.deckManager.ResetDecks();
        stateManager.graphicUpdater.CleanAllCards();
        stateManager.ResetSplit();
        stateManager.betManager.ResetPlayerMoney();

        //disable the money and top card UI as a special case, as nothing else ever needs to touch it
        chipImage.enabled = false;
        moneySymbol.enabled = false;

        stateManager.activeState = this;
        return this;
    }

    //which triggers does the state react to and how
    public override GameState RegisterTrigger(GameStateManager.GameEventTriggers trigger)
    {
        switch (trigger)
        {
            case GameStateManager.GameEventTriggers.StartGameClicked:
                //re-enable the special cases
                chipImage.enabled = true;
                moneySymbol.enabled = true;
                return stateManager.gsBet.SwitchToThisState();
            default: return this;
        }
    }
}
