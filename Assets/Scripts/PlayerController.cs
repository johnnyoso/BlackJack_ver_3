using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class PlayerController : MonoBehaviour {

    public GameObject[] playerCards = new GameObject[10];
    
    BlackJackController blackJackController;
    CardController cardController;
    DealerController dealerController;  
    public Canvas canvas;
    public Button hitButton;
    public Button standButton;
    public InputField betField;
    public InputField playerAvailableFunds;
    public InputField showPlayerStatus;
    
    public int firstCardScore;
    public int secondCardScore;
    public int playerScore;
    public string playerStatus = "";
    public float playerFunds = 1000;
   
    public float playerCurrentBet;
    int hitPresses = 0;
    public int cardIndex = 2; //the player's hand index

    Vector3 firstCardPosition;
    Quaternion cardAngle;

    

    public void Awake()
    {
        blackJackController = FindObjectOfType(typeof(BlackJackController)) as BlackJackController;
        cardController = FindObjectOfType(typeof(CardController)) as CardController;
        dealerController = FindObjectOfType(typeof(DealerController)) as DealerController;
        HitButtonState(false);
        StandButtonState(false);
        BetFieldState("clear");
        PlayerFundsState(false);
        ShowPlayerStatusState("clear");

        //Shows each players starting funds
        playerAvailableFunds.GetComponent<InputField>().text = playerFunds.ToString();
    }

    //Adjust Player's funds after betting
    public void AdjustPlayerFunds()
    {
        playerFunds = playerFunds - playerCurrentBet;

        playerAvailableFunds.GetComponent<InputField>().text = playerFunds.ToString();
    }

    //Spawn first two cards with accepting which player as a parameter
    public void ShowFirstTwoCards(int playerIndex)
    {
        firstCardPosition = blackJackController.playerLocations[playerIndex].transform.position;       

        //This specifies the second card location slight shifted above the first card diagonally
        Vector3 secondCardPosition = new Vector3(firstCardPosition.x + 0.25f, firstCardPosition.y + 0.25f, firstCardPosition.z);

        //This gets the current Player's rotation and applies that to the spawned card
        cardAngle = blackJackController.playerLocations[playerIndex].transform.rotation;        

        GameObject firstPlayerCard = GameObject.Instantiate(playerCards[0], firstCardPosition, cardAngle) as GameObject;
        firstPlayerCard.GetComponent<SpriteRenderer>().sortingLayerName = "FirstCard";
        firstPlayerCard.transform.SetParent(canvas.transform);

        firstCardScore = int.Parse(firstPlayerCard.GetComponentInChildren<CardManager>().ReturnCardValue());

        GameObject secondPlayerCard = GameObject.Instantiate(playerCards[1],secondCardPosition, cardAngle) as GameObject;

        //This spawns the second card above the first card
        secondPlayerCard.GetComponent<SpriteRenderer>().sortingLayerName = "SecondCard";         
        secondPlayerCard.transform.SetParent(canvas.transform);

        secondCardScore = int.Parse(secondPlayerCard.GetComponentInChildren<CardManager>().ReturnCardValue());

        //Update player's status field
        showPlayerStatus.GetComponent<InputField>().text = (firstCardScore + secondCardScore).ToString();
    }

    public void ReturnFirstTwoCards(GameObject firstPlayerCard, GameObject secondPlayerCard)
    {
        firstCardScore = int.Parse(firstPlayerCard.GetComponentInChildren<CardManager>().ReturnCardValue());
        secondCardScore = int.Parse(secondPlayerCard.GetComponentInChildren<CardManager>().ReturnCardValue());       
    }    


    //Manages Hit button behaviour
    public void HitButtonPressed()
    {                

        //the next card in the deck
        int x = blackJackController.cardTracker;

        //the players third card location in his / her hand               

        if (hitPresses == 0)
        {     
            HitSpawnCard(x, cardIndex, 0.5f, "ThirdCard");
            ScoreCheckPerHit();
            
        }
        
        else if (hitPresses == 1)
        {
            HitSpawnCard(x, cardIndex, 0.75f, "FourthCard");
            ScoreCheckPerHit();
            
        }

        else if (hitPresses == 2)
        {
            HitSpawnCard(x, cardIndex, 1f, "FifthCard");
            ScoreCheckPerHit();
        }

        else if (hitPresses == 3)
        {
            HitSpawnCard(x, cardIndex, 1.25f, "SixthCard");
            ScoreCheckPerHit();
        }

        else if (hitPresses == 4)
        {
            HitSpawnCard(x, cardIndex, 1.5f, "SeventhCard");
            ScoreCheckPerHit();
        }
        cardIndex++;
    }

    public void HitSpawnCard(int x, int cardIndex, float distance, string sortingLayer)
    {
        int cardValue;

        //puts the next card in the deck in the players's hand
        playerCards[cardIndex] = cardController.ReturnCardfromDeck(x + 1);

        //Spawns the next card
        Vector3 currentCardPosition = new Vector3(firstCardPosition.x + distance, firstCardPosition.y + distance, firstCardPosition.z);
        GameObject currentPlayerCard = GameObject.Instantiate(playerCards[cardIndex], currentCardPosition, cardAngle) as GameObject;
        currentPlayerCard.GetComponent<SpriteRenderer>().sortingLayerName = sortingLayer;
        currentPlayerCard.transform.SetParent(canvas.transform);

        //Update player's score
        cardValue = int.Parse(playerCards[cardIndex].GetComponent<CardManager>().ReturnCardValue());
        playerScore = playerScore + cardValue;
        showPlayerStatus.GetComponent<InputField>().text = playerScore.ToString();

        //Update the counters
        blackJackController.cardTracker++;
        hitPresses++;
        
    }

    //Scores the player's first two cards
    public void InitialScoreCheck(int firstCardScore, int secondCardScore)
    {
        //Check for natural BlackJack
        if ((firstCardScore == 10 && secondCardScore == 1) || (firstCardScore == 1 && secondCardScore == 10))
        {
            Debug.Log("has BlackJack!");           

            //Update the player's score
            playerScore = 21;

            //update player status
            playerStatus = "blackjack";

            //Update player's status field
            ShowPlayerStatusState(playerStatus);

            //If it's the last player's turn then start dealer and player showdown
            if (blackJackController.playerTurnIndex == blackJackController.numberOfPlayers - 1)
            {
                dealerController.DealerHitCards();
            }

            blackJackController.playerTurnIndex++;
            blackJackController.PlayerTurn();
        }

        else
        {
            playerScore = firstCardScore + secondCardScore;

            HitButtonState(true);
            StandButtonState(true);
            //ShowPlayerStatusState("bet");

        }

        
    }

    //Checks player's score per additional HIT 
    public void ScoreCheckPerHit()
    {
        if (playerScore == 21)
        {
            HitButtonState(false);
            StandButtonState(false);
            playerStatus = "blackjack";

            //Update player's status field
            ShowPlayerStatusState(playerStatus);

            //If it's the last player's turn then start dealer and player showdown
            if (blackJackController.playerTurnIndex == blackJackController.numberOfPlayers - 1)
            {
                dealerController.DealerHitCards();
            }

            blackJackController.playerTurnIndex++;

            blackJackController.PlayerTurn();
        }

        else if (playerScore > 21)
        {
            HitButtonState(false);
            StandButtonState(false);
            playerStatus = "bust";

            //Update player's status field
            ShowPlayerStatusState(playerStatus);

            //If it's the last player's turn then start dealer and player showdown
            if (blackJackController.playerTurnIndex == blackJackController.numberOfPlayers - 1)
            {
                dealerController.DealerHitCards();
            }

            blackJackController.playerTurnIndex++;

            blackJackController.PlayerTurn();
        }
    }

    public void StandButtonPressed()
    {
        HitButtonState(false);
        StandButtonState(false);

        //Update player's status field
        showPlayerStatus.GetComponent<InputField>().text = playerScore.ToString();
                 
        //If it's the last player's turn
        if (blackJackController.playerTurnIndex == blackJackController.numberOfPlayers - 1)
        {
            dealerController.DealerHitCards();
        }

        blackJackController.playerTurnIndex++;

        blackJackController.PlayerTurn();
    }

     
    public void PlayerBets()
    {        
        playerCurrentBet = int.Parse(betField.GetComponentInChildren<Text>().text);

        if (playerCurrentBet > playerFunds)
        {
            Debug.Log("Player bet is more than current funds");

            ShowPlayerStatusState("not enough funds");
        }

        else if (playerCurrentBet % 10 != 0)
        {
            Debug.Log("Please only bet multiples of 10");

            
        }

        else
        {
            BetFieldState("freeze");

            //Deduct player's current bet from total funds
            AdjustPlayerFunds();

            //If the player is the last better: show deal button, hide next round button
            if (blackJackController.bettingTurnIndex == blackJackController.numberOfPlayers - 1)
            {
                blackJackController.DealButtonState(true);
                blackJackController.NextRoundState(false);
            }

            blackJackController.bettingTurnIndex++;            
            blackJackController.BettingTurn();
            
        }                    
    }
   

    //Button States
    public void HitButtonState(bool state)
    {
        if (state == true)
        {
            hitButton.enabled = true;
            hitButton.interactable = true;
            hitButton.GetComponentInChildren<CanvasGroup>().alpha = 1;
            hitButton.GetComponentInChildren<Text>().color = Color.black;
        }

        else
        {
            hitButton.enabled = false;
            hitButton.interactable = false;

            hitButton.GetComponentInChildren<CanvasGroup>().alpha = 0;
            hitButton.GetComponentInChildren<Text>().color = Color.clear;
            
            
        }
    }

    public void StandButtonState(bool state)
    {
        if (state == true)
        {
            standButton.enabled = true;
            standButton.interactable = true;
            standButton.GetComponentInChildren<CanvasGroup>().alpha = 1;
            standButton.GetComponentInChildren<Text>().color = Color.black;
        }

        else 
        {
            standButton.enabled = false;
            standButton.interactable = false;

            standButton.GetComponentInChildren<CanvasGroup>().alpha = 0;
            standButton.GetComponentInChildren<Text>().color = Color.clear;
            
            

        }
    }

    public void BetFieldState(string state)
    {
        if (state == "active")
        {
            betField.enabled = true;
            betField.interactable = true;
            betField.GetComponentInChildren<CanvasGroup>().alpha = 1;
            betField.GetComponentInChildren<Text>().color = Color.black;
        }

        else if (state == "clear")
        {
            betField.enabled = false;
            betField.interactable = false;
            
            betField.GetComponentInChildren<CanvasGroup>().alpha = 0;
            betField.GetComponentInChildren<Text>().color = Color.clear;
            
        }

        else if (state == "freeze")
        {
            betField.interactable = false;
        }

        else if (state == "reset")
        {
            betField.GetComponent<InputField>().text = "";
        }
    }

    public void PlayerFundsState(bool state)
    {
        if (state == true)
        {
            playerAvailableFunds.enabled = true;
            playerAvailableFunds.interactable = true;
            playerAvailableFunds.GetComponentInChildren<CanvasGroup>().alpha = 1;
            playerAvailableFunds.GetComponentInChildren<Text>().color = Color.black;
        }

        else
        {
            playerAvailableFunds.enabled = false;
            playerAvailableFunds.interactable = false;
            playerAvailableFunds.GetComponentInChildren<CanvasGroup>().alpha = 0;
            playerAvailableFunds.GetComponentInChildren<Text>().color = Color.clear;
        }
    }

    public void ShowPlayerStatusState(string state)
    {
        
        if (state == "clear")
        {
            showPlayerStatus.enabled = false;
            showPlayerStatus.interactable = false;

            showPlayerStatus.GetComponentInChildren<CanvasGroup>().alpha = 0;
            //showPlayerStatus.GetComponentInChildren<InputField>().color = Color.clear;
        }

        else if (state == "bet")
        {
            showPlayerStatus.enabled = true;
            showPlayerStatus.interactable = false;

            showPlayerStatus.GetComponentInChildren<CanvasGroup>().alpha = 1;
            //showPlayerStatus.GetComponentInChildren<InputField>().color = Color.black;
            showPlayerStatus.GetComponentInChildren<InputField>().text = "Place Bet";
        }

        else if (state == "reset")
        {
            showPlayerStatus.enabled = true;
            showPlayerStatus.interactable = false;

            showPlayerStatus.GetComponentInChildren<CanvasGroup>().alpha = 1;
            showPlayerStatus.GetComponentInChildren<InputField>().text = "";
        }

        else
        {
            showPlayerStatus.enabled = true;
            showPlayerStatus.interactable = false;

            showPlayerStatus.GetComponentInChildren<CanvasGroup>().alpha = 1;
            showPlayerStatus.GetComponentInChildren<InputField>().text = state;
        }
    }

    public void ClearPlayerCounters()
    {
        hitPresses = 0;
        cardIndex = 2;
        playerStatus = "";
        playerScore = 0;
        firstCardScore = 0;
        secondCardScore = 0;
    }
}
