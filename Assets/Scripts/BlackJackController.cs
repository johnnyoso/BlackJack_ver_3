using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BlackJackController : MonoBehaviour {

    //Contains the Players as an Array
    public GameObject[] playerLocations = new GameObject[5];
    public GameObject dealerLocation;
    public GameObject[] decks;
    public GameObject[] allCards;

    CardController cardController;
    PlayerController playerController;
    DealerController dealerController;

    //Dropdowns to pick number of players and decks
    public Dropdown deckCount;
    public Dropdown playerCount;

    public Button startGame;
    public Button dealButton;
    public Button nextRound;
    public Button newGame;

    [Range(1, 5)]
    public int numberOfPlayers;

    [Range(1, 3)]
    public int numberOfDecks;

    public int cardTracker;
    public int playerTurnIndex = 0;
    public int bettingTurnIndex = 0;

    public bool playerSelected, deckSelected = false;


    public void Awake()
    {
        startGame.interactable = false;
        cardController = FindObjectOfType(typeof(CardController)) as CardController;
        playerController = FindObjectOfType(typeof(PlayerController)) as PlayerController;
        dealerController = FindObjectOfType(typeof(DealerController)) as DealerController;

        DealButtonState(false);
        NextRoundState(false);
        NewGameButtonState(false);

        //shuffles the deck 4 times
        for (int i = 0; i < 5; i++)
        {
            cardController.CardShuffle();
        }
                      
    }
    
    //Selects the number of players
    public void SelectNumberOfPlayers()
    {
        numberOfPlayers = playerCount.value;
        playerSelected = true;
        
        EnableStartGameButton(playerSelected, deckSelected);     
    }

    //Selects the number of decks
    public void SelecNumberOfDecks()
    {       
        numberOfDecks = deckCount.value;
        deckSelected = true;
        
        EnableStartGameButton(playerSelected, deckSelected);
    }

    //Checks if Player has selected both the number of players and decks before enabling Start Button
    public void EnableStartGameButton(bool playerSelected, bool deckSelected)
    {
        Debug.Log(playerSelected);
        Debug.Log(deckSelected);

        if (playerSelected == true && deckSelected == true)
        {
            startGame.interactable = true;
        }
    }

    //Start a new game
    public void StartNewGame()
    {
        nextRound.GetComponentInChildren<Text>().text = "Next Round?";
        NextRoundState(false);
        NewGameButtonState(false);
        
        //Reset all player's funds
        for (int i = 0; i < numberOfPlayers; i++)
        {
            playerLocations[i].GetComponent<PlayerController>().playerFunds = 1000;
        }

        //Reset dealer's funds
        dealerController.dealerFunds = 100000;

        StartGame();
    }

    //Starts game
    public void StartGame()
    {
        
        //Disables and vanishes Player Count dropdown
        Debug.Log("Number of Players is: " + numberOfPlayers);
        playerCount.interactable = false;
        playerCount.enabled = false;
        playerCount.GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        playerCount.GetComponentInChildren<Text>().color = Color.clear;

        //Disables and vanishes Deck Count dropdown
        Debug.Log("Number of Decks is:" + numberOfDecks);
        deckCount.interactable = false;
        deckCount.enabled = false;
        deckCount.GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        deckCount.GetComponentInChildren<Text>().color = Color.clear;

        //Disables and vanishes Start Game Button
        startGame.interactable = false;
        startGame.enabled = false;
        startGame.GetComponentInChildren<CanvasRenderer>().SetAlpha(0);
        startGame.GetComponentInChildren<Text>().color = Color.clear;

        //Sets the table with the correct number of users
        UpdateTable(numberOfPlayers, numberOfDecks);

        //Accepts bets one at a time from players

        BettingTurn();
               
        

    }


    //Sets the table with the chosen players and number of decks
    public void UpdateTable(int numberOfPlayers, int numberOfDecks)
    {
        //Update the number of players
        for (int i = playerLocations.Length; i > numberOfPlayers; i--)
        {
            Destroy(playerLocations[i - 1]);
        }

        //Update the number of decks
    }
      
    //Deal first two cards for players and one card for dealer
    public void DealCards()
    {       
        int deckCardIndex = 0;

        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (playerLocations[i] != null)
            {
                playerLocations[i].GetComponent<PlayerController>().playerCards[0] = cardController.ReturnCardfromDeck(i);
                deckCardIndex = i;

                cardTracker++;
            }

            else
            {
                continue;
            }
        }

        deckCardIndex++;

        dealerController.dealerCards[0] = cardController.ReturnCardfromDeck(deckCardIndex);

        deckCardIndex++;

        cardTracker++;

        //Second card
        for (int j = 0; j < numberOfPlayers; j++)
        {
            if (playerLocations[j] != null)
            {
                playerLocations[j].GetComponent<PlayerController>().playerCards[1] = cardController.ReturnCardfromDeck(deckCardIndex + j);

                cardTracker++;
            }

            else
            {
                continue;
            }
        }       
        
        //Spawn the Player/s' first two cards
        for (int playerIndex = 0; playerIndex < numberOfPlayers; playerIndex++)
        {
            //playerController.ShowFirstTwoCards(playerIndex); --> does not work, only shows Player1's cards all the way
            if (playerLocations[playerIndex] != null)
            {
                //This one works as it actually references the PlayerIndex and gets it's very own PlayerController class   
                playerLocations[playerIndex].GetComponent<PlayerController>().ShowFirstTwoCards(playerIndex);
            }
        }

        //Spawn the Dealer's single card
        dealerController.ShowDealerCard();

        //Disable deal button
        DealButtonState(false);

        //Disable shuffle button
        cardController.cardShuffleButton.interactable = false;         
        
        //Shows the player's funds
        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (playerLocations[i] != null)
            {
                playerLocations[i].GetComponent<PlayerController>().PlayerFundsState(true);
            }
            else
            {
                continue;
            }
        }   

        //Starts with the first player
        PlayerTurn();

    }

    //This manages the player's betting order
    public void BettingTurn()
    {
        if (bettingTurnIndex == 0 && playerLocations[0] != null)
        {
            //Reveals player one's betting input area        
            playerLocations[0].GetComponent<PlayerController>().BetFieldState("active");
            playerLocations[0].GetComponent<PlayerController>().ShowPlayerStatusState("bet");
        }

        else if (bettingTurnIndex == 0 && playerLocations[0] == null)
        {

            StackOverflow(1);            
        }

                
        else if (bettingTurnIndex == 1 && bettingTurnIndex < numberOfPlayers && playerLocations[1] != null)
        {
            //Reveals player two's betting input area        
            playerLocations[1].GetComponent<PlayerController>().BetFieldState("active");
            playerLocations[1].GetComponent<PlayerController>().ShowPlayerStatusState("bet");

        }

        else if (bettingTurnIndex == 1 && playerLocations[1] == null)
        {
            Debug.Log("Fuck!!");

            StackOverflow(2);
        }


        else if (bettingTurnIndex == 2 && bettingTurnIndex < numberOfPlayers && playerLocations[2] != null)
        {
            //Reveals player three's betting input area        
            playerLocations[2].GetComponent<PlayerController>().BetFieldState("active");
            playerLocations[2].GetComponent<PlayerController>().ShowPlayerStatusState("bet");
        }

        else if (bettingTurnIndex == 2 && playerLocations[2] == null)
        {
            StackOverflow(3);
        }

        else if (bettingTurnIndex == 3 && bettingTurnIndex < numberOfPlayers && playerLocations[3] != null)
        {
            //Reveals player four's betting input area        
            playerLocations[3].GetComponent<PlayerController>().BetFieldState("active");
            playerLocations[3].GetComponent<PlayerController>().ShowPlayerStatusState("bet");
        }

        else if (bettingTurnIndex == 3 && playerLocations[3] == null)
        {
            StackOverflow(4);
        }

        else if (bettingTurnIndex == 4 && bettingTurnIndex < numberOfPlayers && playerLocations[4] != null)
        {
            //Reveals player five's betting input area        
            playerLocations[4].GetComponent<PlayerController>().BetFieldState("active");
            playerLocations[4].GetComponent<PlayerController>().ShowPlayerStatusState("bet");
        }

        else if (bettingTurnIndex == 4 && playerLocations[4] == null)
        {
            //Go back to player 1
            StackOverflow(0);
        }

    }

    public void StackOverflow(int index)
    {
        bettingTurnIndex++;
        //Reveals player five's betting input area        
        playerLocations[index].GetComponent<PlayerController>().BetFieldState("active");
        playerLocations[index].GetComponent<PlayerController>().ShowPlayerStatusState("bet");
        
    }

    //This manages the order the players play their hand
    public void PlayerTurn()
    {
        if (playerTurnIndex == 0 && playerLocations[0] != null)
        {
            int firstCardScore = playerLocations[0].GetComponent<PlayerController>().firstCardScore;
            int secondCardScore = playerLocations[0].GetComponent<PlayerController>().secondCardScore;
            playerLocations[0].GetComponent<PlayerController>().InitialScoreCheck(firstCardScore, secondCardScore);

        }

        else if (playerLocations[0] == null && playerTurnIndex == 0)
        {
            playerTurnIndex++;
            int firstCardScore = playerLocations[1].GetComponent<PlayerController>().firstCardScore;
            int secondCardScore = playerLocations[1].GetComponent<PlayerController>().secondCardScore;
            playerLocations[1].GetComponent<PlayerController>().InitialScoreCheck(firstCardScore, secondCardScore);


        }

        else if (playerTurnIndex == 1 && playerTurnIndex < numberOfPlayers && playerLocations[1] != null)
        {
            int firstCardScore = playerLocations[1].GetComponent<PlayerController>().firstCardScore;
            int secondCardScore = playerLocations[1].GetComponent<PlayerController>().secondCardScore;
            playerLocations[1].GetComponent<PlayerController>().InitialScoreCheck(firstCardScore, secondCardScore);


        }

        else if (playerLocations[1] == null && playerTurnIndex == 1)
        {
            playerTurnIndex++;
            int firstCardScore = playerLocations[2].GetComponent<PlayerController>().firstCardScore;
            int secondCardScore = playerLocations[2].GetComponent<PlayerController>().secondCardScore;
            playerLocations[2].GetComponent<PlayerController>().InitialScoreCheck(firstCardScore, secondCardScore);

        }

        else if (playerTurnIndex == 2 && playerTurnIndex < numberOfPlayers && playerLocations[2] != null)
        {
            int firstCardScore = playerLocations[2].GetComponent<PlayerController>().firstCardScore;
            int secondCardScore = playerLocations[2].GetComponent<PlayerController>().secondCardScore;
            playerLocations[2].GetComponent<PlayerController>().InitialScoreCheck(firstCardScore, secondCardScore);


        }

        else if (playerLocations[2] == null && playerTurnIndex == 2)
        {
            playerTurnIndex++;
            int firstCardScore = playerLocations[3].GetComponent<PlayerController>().firstCardScore;
            int secondCardScore = playerLocations[3].GetComponent<PlayerController>().secondCardScore;
            playerLocations[3].GetComponent<PlayerController>().InitialScoreCheck(firstCardScore, secondCardScore);

        }

        else if (playerTurnIndex == 3 && playerTurnIndex < numberOfPlayers && playerLocations[3])
        {
            int firstCardScore = playerLocations[3].GetComponent<PlayerController>().firstCardScore;
            int secondCardScore = playerLocations[3].GetComponent<PlayerController>().secondCardScore;
            playerLocations[3].GetComponent<PlayerController>().InitialScoreCheck(firstCardScore, secondCardScore);


        }

        else if (playerLocations[3] == null && playerTurnIndex == 3)
        {
            playerTurnIndex++;
            int firstCardScore = playerLocations[4].GetComponent<PlayerController>().firstCardScore;
            int secondCardScore = playerLocations[4].GetComponent<PlayerController>().secondCardScore;
            playerLocations[4].GetComponent<PlayerController>().InitialScoreCheck(firstCardScore, secondCardScore);

        }

        else if (playerTurnIndex == 4 && playerTurnIndex < numberOfPlayers && playerLocations[4] != null)
        {
            int firstCardScore = playerLocations[4].GetComponent<PlayerController>().firstCardScore;
            int secondCardScore = playerLocations[4].GetComponent<PlayerController>().secondCardScore;
            playerLocations[4].GetComponent<PlayerController>().InitialScoreCheck(firstCardScore, secondCardScore);

        }

        else if (playerLocations[4] == null && playerTurnIndex == 4)
        {
            playerTurnIndex++;
        }

    }

    //This is the showdown between players and dealer 
    public void FinalScoreCheck()
    {
        if (dealerController.dealerStatus != "bust" && dealerController.dealerStatus != "blackjack")
        {
            for (int i = 0; i < numberOfPlayers; i++)
            {
                if (playerLocations[i] != null)
                {
                    if (playerLocations[i].GetComponent<PlayerController>().playerStatus == "bust" && playerLocations[i] != null)
                    {
                        //maybe instantiate a layover text showing BUST
                        Debug.Log(playerLocations[i] + " is bust!");

                        //Player pays the dealer
                        //Player pays dealer
                        float c = dealerController.dealerFunds;
                        float d = playerLocations[i].GetComponent<PlayerController>().playerCurrentBet;
                        c = c + d;

                        dealerController.dealerFunds = c;
                    }

                    else if (playerLocations[i].GetComponent<PlayerController>().playerScore > dealerController.dealerScore && playerLocations[i].GetComponent<PlayerController>().playerStatus != "blackjack" && playerLocations[i] != null)
                    {
                        Debug.Log(playerLocations[i] + "wins");

                        //Update player status field
                        playerLocations[i].GetComponent<PlayerController>().ShowPlayerStatusState("WIN");


                        //Dealer pays Player
                        float e = dealerController.dealerFunds;
                        float f = playerLocations[i].GetComponent<PlayerController>().playerCurrentBet;
                        float g = playerLocations[i].GetComponent<PlayerController>().playerFunds;

                        g = g + (f * 2); //Player gets his initial bet back plus another amount equal to the bet
                        e = e - f;

                        playerLocations[i].GetComponent<PlayerController>().playerFunds = g;
                        dealerController.dealerFunds = e;
                    }

                    else if (playerLocations[i].GetComponent<PlayerController>().playerScore < dealerController.dealerScore && playerLocations[i] != null)
                    {
                        Debug.Log("Dealer wins against: " + playerLocations[i]);

                        //Update player status field
                        playerLocations[i].GetComponent<PlayerController>().ShowPlayerStatusState("LOSE");

                        //Player pays dealer
                        float c = dealerController.dealerFunds;
                        float d = playerLocations[i].GetComponent<PlayerController>().playerCurrentBet;
                        c = c + d;

                        dealerController.dealerFunds = c;
                    }

                    else if (playerLocations[i].GetComponent<PlayerController>().playerScore == dealerController.dealerScore && playerLocations[i] != null)
                    {
                        Debug.Log(playerLocations[i] + " and Dealer SPLITS!");

                        //Update player status field
                        playerLocations[i].GetComponent<PlayerController>().ShowPlayerStatusState("SPLIT");


                        //Player gets his money back
                        float a = playerLocations[i].GetComponent<PlayerController>().playerFunds;
                        float b = playerLocations[i].GetComponent<PlayerController>().playerCurrentBet;
                        a = a + b;

                        playerLocations[i].GetComponent<PlayerController>().playerFunds = a;
                    }

                    else if (playerLocations[i].GetComponent<PlayerController>().playerStatus == "blackjack" && playerLocations[i] != null)
                    {
                        Debug.Log(playerLocations[i] + " is BlackJack!!");

                        //Dealer pays player dearly
                        float e = dealerController.dealerFunds;
                        float f = playerLocations[i].GetComponent<PlayerController>().playerCurrentBet;
                        float g = playerLocations[i].GetComponent<PlayerController>().playerFunds;

                        g = g + f + (f * 1.5f); //Player gets his initial bet back plus another amount more than the the bet
                        e = e - (f + (f * 1.5f));

                        Debug.Log("blackjack pay is: " + g);

                        playerLocations[i].GetComponent<PlayerController>().playerFunds = g;
                        dealerController.dealerFunds = e;
                    }

                }

                else
                {
                    continue;
                }
            }
           
        }


        else if (dealerController.dealerStatus == "blackjack")
        {
            for (int i = 0; i < numberOfPlayers; i++)
            {
                if (playerLocations[i] != null)
                {
                    if (playerLocations[i].GetComponent<PlayerController>().playerStatus == "blackjack" && playerLocations[i] != null)
                    {
                        Debug.Log(playerLocations[i] + " " + "Dealer are split!");

                        //Update Player status field
                        playerLocations[i].GetComponent<PlayerController>().ShowPlayerStatusState("SPLIT"); ;


                        //Player gets his money back
                        float a = playerLocations[i].GetComponent<PlayerController>().playerFunds;
                        float b = playerLocations[i].GetComponent<PlayerController>().playerCurrentBet;
                        a = a + b;

                        playerLocations[i].GetComponent<PlayerController>().playerFunds = a;
                    }

                    else if ((playerLocations[i].GetComponent<PlayerController>().playerStatus == "bust" || playerLocations[i].GetComponent<PlayerController>().playerScore < 21) && playerLocations[i] != null)
                    {
                        Debug.Log(playerLocations[i] + " loses!");

                        //Player pays dealer
                        float c = dealerController.dealerFunds;
                        float d = playerLocations[i].GetComponent<PlayerController>().playerCurrentBet;
                        c = c + d;

                        dealerController.dealerFunds = c;
                    }
                }

                else
                {
                    continue;
                }
            }
           
        }

        else if (dealerController.dealerStatus == "bust")
        {
            for (int i = 0; i < numberOfPlayers; i++)
            {
                if (playerLocations[i] != null)
                {
                    if (playerLocations[i].GetComponent<PlayerController>().playerStatus == "bust" && playerLocations[i] != null)
                    {
                        //maybe instantiate a layover text showing BUST
                        Debug.Log(playerLocations[i] + " is bust!");


                        //Player pays dealer
                        float c = dealerController.dealerFunds;
                        float d = playerLocations[i].GetComponent<PlayerController>().playerCurrentBet;
                        c = c + d;

                        dealerController.dealerFunds = c;
                    }

                    else if (playerLocations[i].GetComponent<PlayerController>().playerStatus != "bust" && playerLocations[i] != null)
                    {
                        Debug.Log(playerLocations[i] + "wins");

                        //Update Player status field
                        playerLocations[i].GetComponent<PlayerController>().ShowPlayerStatusState("WIN");


                        //Dealer pays Player
                        float e = dealerController.dealerFunds;
                        float f = playerLocations[i].GetComponent<PlayerController>().playerCurrentBet;
                        float g = playerLocations[i].GetComponent<PlayerController>().playerFunds;

                        g = g + (f * 2); //Player gets his initial bet back plus another amount equal to the bet
                        e = e - f;

                        playerLocations[i].GetComponent<PlayerController>().playerFunds = g;
                        dealerController.dealerFunds = e;
                    }

                    else if (playerLocations[i] == null)
                    {
                        Debug.Log("Second Stage");
                        continue;
                    }
                }

                else
                {
                    continue;
                }
            }
        }
        //Show next round button
        NextRoundState(true);
        
        //Check if there are any players with ZERO funds and disable them
        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (playerLocations[i] != null)
            {
                if (playerLocations[i].GetComponent<PlayerController>().playerFunds == 0 && playerLocations[i] != null)
                {
                    playerLocations[i] = null;

                }
            }
            
            else
            {
                continue;
            }                
        }

       

        
    }

    //Start Next Game
    public void NextRound()
    {
        //Clear all counters
        cardTracker = 0;
        playerTurnIndex = 0;
        bettingTurnIndex = 0;

        //Check for remaining players
        if (CheckRemainingPlayers())
        {
            nextRound.GetComponentInChildren<Text>().text = "GAME OVER";
            nextRound.interactable = false;
            NewGameButtonState(true);

            //Destroy all cards on the table
            allCards = GameObject.FindGameObjectsWithTag("Card");
            foreach (GameObject allTheCard in allCards)
            {
                Destroy(allTheCard);
            }
        }

       

        //Clear player counters
        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (playerLocations[i] != null)
            {
                playerLocations[i].GetComponent<PlayerController>().ClearPlayerCounters();
            }

            else
            {
                continue;
            }
        }

        //Clear dealer counters
        dealerController.ClearDealerCounters();

        dealerController.ShowDealerStatus("clear");

        //Clear all betting fields
        for (int i = 0; i < numberOfPlayers; i++)
        {
            
            if (playerLocations[i] != null)
            {
                playerLocations[i].GetComponent<PlayerController>().BetFieldState("reset");
                playerLocations[i].GetComponent<PlayerController>().BetFieldState("clear");
            }

            else
            {
                continue;
            }
        }

        //Restart Betting order
        
        BettingTurn();
        
        
        for (int j = 0; j < numberOfPlayers; j++)
        {
            if (playerLocations[j] != null)
            {
                //Clear all Player's current bet values
                playerLocations[j].GetComponent<PlayerController>().playerCurrentBet = 0;

                //Clear player status field
                playerLocations[j].GetComponent<PlayerController>().ShowPlayerStatusState("clear");
            }

            else
            {
                continue;
            }
        }

        if (playerLocations[0] != null)
        {
            playerLocations[0].GetComponent<PlayerController>().ShowPlayerStatusState("bet");

        }

        //Destroy all cards on the table
        allCards = GameObject.FindGameObjectsWithTag("Card");
        foreach (GameObject allTheCard in allCards)
        {
            Destroy(allTheCard);
        }    

        //Reshuffle the deck three times
        for (int i = 0; i < 4; i++)
        {
            cardController.CardShuffle();
        }

        //Disable Next Round button
        NextRoundState(false);     

        
        for (int i = 0; i < numberOfPlayers; i++)
        {
            if (playerLocations[i] != null)
            {
                //Adjust all player's funds
                playerLocations[i].GetComponent<PlayerController>().AdjustPlayerFunds();

                //Clears all the player's hand
                for (int j = 0; j < 10; j++)
                {
                    playerLocations[i].GetComponent<PlayerController>().playerCards[j] = null;
                }
            }

            else
            {
                continue;
            }
        }

        //Clear all the dealer's cards
        for (int j = 0; j < 10; j++)
        {
            dealerController.dealerCards[j] = null;
        }

       
    }

    public bool CheckRemainingPlayers()
    {
        bool state = playerLocations[0] == null &&
            playerLocations[1] == null &&
            playerLocations[2] == null &&
            playerLocations[3] == null &&
            playerLocations[4] == null;

        if (state == true)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    //Button states**********************************************************************
    //Sets the Deal Button's state
    public void DealButtonState(bool state)
    {
        if (state == true)
        {
            //Deal button appears and active
            dealButton.interactable = true;
            dealButton.enabled = true;
            dealButton.GetComponentInChildren<CanvasGroup>().alpha = 1;
            dealButton.GetComponentInChildren<Text>().color = Color.black;
        }

        else
        {
            dealButton.enabled = false;
            dealButton.interactable = false;

            dealButton.GetComponentInChildren<CanvasGroup>().alpha = 0;
            dealButton.GetComponentInChildren<Text>().color = Color.clear;
        }
    }

    public void NextRoundState(bool state)
    {
        if (state == true)
        {
            //Deal button appears and active
            nextRound.interactable = true;
            nextRound.enabled = true;
            nextRound.GetComponentInChildren<CanvasGroup>().alpha = 1;
            nextRound.GetComponentInChildren<Text>().color = Color.black;
        }

        else
        {
            nextRound.enabled = false;
            nextRound.interactable = false;
            nextRound.GetComponentInChildren<CanvasGroup>().alpha = 0;
            nextRound.GetComponentInChildren<Text>().color = Color.clear;
        }
    }

    public void NewGameButtonState(bool state)
    {
        if (state == true)
        {
            //Deal button appears and active
            newGame.interactable = true;
            newGame.enabled = true;
            newGame.GetComponentInChildren<CanvasGroup>().alpha = 1;
            newGame.GetComponentInChildren<Text>().color = Color.black;
        }

        else
        {
            newGame.enabled = false;
            newGame.interactable = false;
            newGame.GetComponentInChildren<CanvasGroup>().alpha = 0;
            newGame.GetComponentInChildren<Text>().color = Color.clear;
        }
    }
}
