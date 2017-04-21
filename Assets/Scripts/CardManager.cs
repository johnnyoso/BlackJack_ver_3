using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardManager : MonoBehaviour {

    public GameObject card;
    public Sprite cardImage;
    public Text cardValue;
    public Text cardSuit;
    public string faceCardValue = "10";

    //************************************************************************************
    //Return Card Value
    public string ReturnCardValue()
    {
        if (cardValue.text == "13" || cardValue.text == "12" || cardValue.text == "11")
        {
            return faceCardValue;
        }

        else
        {
            return cardValue.text;
        }       
    }

    //************************************************************************************
    //Return Card Suit
    public string ReturnCardSuit()
    {
        return cardSuit.text;
    }

    //************************************************************************************
    //Return Card Face
    public string ReturnCardFace()
    {        
        if (cardValue.text == "11")
        {
            return "jack";
        }
        else if (cardValue.text == "12")
        {
            return "queen";
        }
        else if (cardValue.text == "13")
        {
            return "king";
        }
        else
        {
            return cardValue.text;
        }
    }
}
