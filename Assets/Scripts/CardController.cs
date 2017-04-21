using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardController : MonoBehaviour {
    
    public GameObject[] deck = new GameObject[52]; // our deck of cards
    public GameObject backCard;
    public Button cardShuffleButton;
    public static int value; // for our random value between 0 and 1  

    //************************************************************************************

    //Random number generator: Fisher-Yates method --> I don't think this is very good
    public void CardShuffle()
    {
        int deckSize = deck.Length; // number of cards in a deck: 52
        //Shuffle three times
        for (int x = 0; x < 3; x++)
        {

            for (int i = 0; i < deckSize; i++)
            {
                int r = i + (int)((Random.value) * (deckSize - i));
                GameObject oneCard = deck[r];
                deck[r] = deck[i];
                deck[i] = oneCard;
            }

        }
    }

    //************************************************************************************

    public GameObject ReturnCardfromDeck(int index)
    {
        GameObject playerCard = new GameObject();
        playerCard = deck[index];
        return playerCard;    
    }

    //************************************************************************************

    public GameObject ReturnBackCardfromDeck()
    {
        return backCard;
    }

}


/* GAME NOTES

    28/06/2016
    1. Dealer Cards are now showing
    2. Win condition for Natural BlackJack for both player and dealer are now OK
    3. Stick button now working but does not work if you push hit first - fix it!

    To Do:
        1. Set player and dealer card locations into an array 

    01/07/2016
    1. New game button is now operational
    2. Player and Dealer cards are now in the array and clears after New Game 

    To Do: 
        1. Fix Stick button malfunction after pressing hit button --> Dealer cards not revealing if dealerScore > 17

    02/07/2016
    1. Fixed 2nd Hit button press error --> Return Card Value from GameObject was point to "n" instead of 2
    2. 

    To Do:
        
    */
