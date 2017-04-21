using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DealerController : MonoBehaviour {

    public GameObject[] dealerCards = new GameObject[10];

    BlackJackController blackJackController;
    CardController cardController;
    public Canvas canvas;
    int firstDealerScore;
    int secondDealerScore;
    float distance;
    public int dealerScore;
    public string dealerStatus = "";
    public float dealerFunds = 100000;
    public InputField showDealerStatus;
    Vector3 firstDealerCardLocation;

    string[] sortingLayer = { "FirstCard", "SecondCard", "ThirdCard", "FourthCard", "FifthCard", "SixthCard", "SeventhCard" };
    int x;

    public void Awake()
    {
        blackJackController = FindObjectOfType(typeof(BlackJackController)) as BlackJackController;
        cardController = FindObjectOfType(typeof(CardController)) as CardController;
    }

    //Show dealer's first card
    public void ShowDealerCard()
    {
        firstDealerCardLocation = blackJackController.dealerLocation.transform.position;

        GameObject firstDealerCard = GameObject.Instantiate(dealerCards[0], firstDealerCardLocation, Quaternion.identity) as GameObject;
        firstDealerCard.transform.SetParent(canvas.transform);
        firstDealerCard.GetComponent<SpriteRenderer>().sortingLayerName = "FirstCard";

        firstDealerScore = int.Parse(dealerCards[0].GetComponent<CardManager>().ReturnCardValue());
        dealerScore = firstDealerScore;

        ShowDealerStatus(dealerScore.ToString());
    }

    public void DealerHitCards()
    {
        
        int y = 1;
        int cardValue;
        distance = 0.5f;

        x = blackJackController.cardTracker;
        //return the next card from the deck
        dealerCards[y] = cardController.ReturnCardfromDeck(x);

        secondDealerScore = int.Parse(dealerCards[y].GetComponent<CardManager>().ReturnCardValue());


        if ((firstDealerScore == 10 && secondDealerScore == 1) || (firstDealerScore == 1 && secondDealerScore == 10))
        {
            Debug.Log("Dealer has BlackJack");
            dealerStatus = "blackjack";
            dealerScore = 21;

            //Spawn the dealer card
            Vector3 dealerNextCardLocation = new Vector3(firstDealerCardLocation.x + distance, firstDealerCardLocation.y, firstDealerCardLocation.z);
            GameObject dealerNextCard = GameObject.Instantiate(dealerCards[y], dealerNextCardLocation, Quaternion.identity) as GameObject;

            dealerNextCard.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayer[y];
            dealerNextCard.transform.SetParent(canvas.transform);

            ShowDealerStatus("blackjack");
        }

        else
        {
            while (dealerScore < 17)
            {
                x = blackJackController.cardTracker;

                //return the next card from the deck
                dealerCards[y] = cardController.ReturnCardfromDeck(x);

                //Spawn the dealer card
                Vector3 dealerNextCardLocation = new Vector3(firstDealerCardLocation.x + distance, firstDealerCardLocation.y, firstDealerCardLocation.z);
                GameObject dealerNextCard = GameObject.Instantiate(dealerCards[y], dealerNextCardLocation, Quaternion.identity) as GameObject;

                dealerNextCard.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayer[y];
                dealerNextCard.transform.SetParent(canvas.transform);

                //Returns the card value
                cardValue = int.Parse(dealerCards[y].GetComponent<CardManager>().ReturnCardValue());


                //Update the dealer score
                dealerScore = dealerScore + cardValue;

                //Update dealer status
                ShowDealerStatus(dealerScore.ToString());

                //Update counters
                blackJackController.cardTracker++;
                y++;
                distance = distance + 0.5f;

                
            }
        }
        //Check the dealer's score
        DealerScoreCheck();
        blackJackController.FinalScoreCheck();
    }

    public void DealerScoreCheck()
    {
        if (dealerScore > 21)
        {
            dealerStatus = "bust";
            Debug.Log("Dealer BUSTS!!");
            ShowDealerStatus("bust");
        }

        else if (dealerScore == 21)
        {
            dealerStatus = "blackjack";
            Debug.Log("Dealer has BLACKJACK!!");
            ShowDealerStatus("blackjack");
        }
    }

    public void ShowDealerStatus(string state)
    {
        if (state == "blackjack")
        {
            showDealerStatus.GetComponentInChildren<InputField>().text = state;
        }

        else if (state == "bust")
        {
            showDealerStatus.GetComponentInChildren<InputField>().text = state;
        }

        else if (state == "clear")
        {
            showDealerStatus.GetComponentInChildren<InputField>().text = "";
        }

        else
        {
            showDealerStatus.GetComponentInChildren<InputField>().text = dealerScore.ToString();
        }
    }

    //Clear dealer counters
    public void ClearDealerCounters()
    {
        dealerStatus = "";
        dealerScore = 0;
        firstDealerScore = 0;
    }
}
