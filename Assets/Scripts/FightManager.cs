using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

public class FightManager : MonoBehaviour {

    [SerializeField] Transform spawn1, spawn2; 
    [SerializeField] GameObject startSplash, mainMenuObject, pickScreenObject, endScreenObject, mapController, fightUI;
    MenuController mainMenu; 

    GameObject player1, player2;
    PlayerController pCon1, pCon2; 
    bool player1Ready, player2Ready; 
    int player1Life = -1;
    int player2Life = -1;
    int player1Stocks, player2Stocks;  
    int player1Wins, player2Wins; 
    int roundCountNumber = 0; 
    int winnerId = 0;
    public List<int> winnerOrder = new List<int>(); 
    List<int> player1Order, player2Order; 
    bool ready, gameActive, gameOver; 

    public enum GameState {
        Pregame,
        MainMenu,
        ArcadePickScreen,
        VersusPickScreen,
        ArcadeFightMode,
        VersusFightMode,
        EndScreen
    } 
    GameState gameState; 

    void Awake() {
        mainMenu = mainMenuObject.GetComponent<MenuController>(); 
    }

    void Update() {
        switch(gameState) {
            case GameState.Pregame:
                gameOver = false; 
                startSplash.SetActive(true); 
                endScreenObject.SetActive(false);
                pickScreenObject.SetActive(false); 
                if(player1) {
                    player1.GetComponent<OrderPicker>().mainPlayer = true; 
                    player1.GetComponent<OrderPicker>().active = false; 
                    if(player2) {
                        player2.GetComponent<OrderPicker>().mainPlayer = false; 
                        player2.GetComponent<OrderPicker>().active = false; 
                    }
                    gameState = GameState.MainMenu; 
                    startSplash.SetActive(false); 
                    mainMenuObject.SetActive(true); 
                }
                break;
            case GameState.MainMenu:
                if(mainMenuObject.GetComponent<MenuController>().menuActive == false) {
                    mainMenuObject.SetActive(true); 
                    mainMenuObject.GetComponent<MenuController>().WakeUp();
                }
                
                if(mainMenu.readyArcade) {
                    gameState = GameState.ArcadePickScreen; 
                    mainMenuObject.SetActive(false); 
                    pickScreenObject.SetActive(true); 
                }
                if(mainMenu.readyVersus) {
                    gameState = GameState.VersusPickScreen;
                    mainMenuObject.SetActive(false); 
                    pickScreenObject.SetActive(true); 
                    if(player1) {
                        player1.GetComponent<OrderPicker>().active = true; 
                        player1.GetComponent<OrderPicker>().StartupDelay(); 
                    }
                    if(player2) {
                        player2.GetComponent<OrderPicker>().active = true; 
                        player2.GetComponent<OrderPicker>().StartupDelay(); 
                    }
                }
                break;
            case GameState.ArcadePickScreen:
                pCon1.enabled = true; 
                pCon1.Activate(); 
                player1Order = player1.GetComponent<OrderPicker>().DefaultOrder(); 
                gameState = GameState.ArcadeFightMode; 
                break;
            case GameState.VersusPickScreen:
                if(player2) {
                    player2.GetComponent<OrderPicker>().active = true; 
                }
                if(player1 && !ready && !player1Ready && player1.GetComponent<OrderPicker>().readyStart) {
                    player1Ready = true; 
                }
            
                if(player2 && !ready && !player2Ready && player2.GetComponent<OrderPicker>().readyStart) {
                    player2Ready = true; 
                }
                if(player1Ready && player2Ready) {
                    ready = true; 
                } else {
                    ready = false; 
                }

                if(ready) {
                    pCon1.enabled = true; 
                    pCon1.Activate(); 
                    player1Order = player1.GetComponent<OrderPicker>().playerOrder; 
                    if(player2) {
                        pCon2.enabled = true; 
                        pCon2.Activate(); 
                        player2Order = player2.GetComponent<OrderPicker>().playerOrder; 
                    }
                    gameState = GameState.VersusFightMode; 
                }
                break;
            case GameState.ArcadeFightMode:
                pickScreenObject.SetActive(false);  
                if(player1 && player1Life < 0) {
                    mapController.GetComponent<MapController>().Activate(0);
                    player1.GetComponent<OrderPicker>().active = false; 
                    player1.GetComponent<PlayerController>().SetSister(player1Order[0], spawn1.position, false);
                    player1Life = 3; 
                }
                break;
            case GameState.VersusFightMode:
                if(!gameActive) {
                    mapController.GetComponent<MapController>().Activate(0);
                    gameActive = true;
                    ready = false; 
                    player1Ready = false;
                    player2Ready = false; 
                    pickScreenObject.SetActive(false);  
                    GameObject.Find("Main Camera").GetComponent<CameraController>().ActivateCamera(player1.transform, player2.transform); 
                    player1.GetComponent<OrderPicker>().active = false; 
                    player2.GetComponent<OrderPicker>().active = false; 
                    player2Wins = 0; 
                    player1Wins = 0; 
                    ResetPlayers(); 
                    fightUI.GetComponent<GameUIController>().TwoPSetup(player1Order, player2Order); 

                } else if(gameActive) {
                    if(player1 && player1Life <= 0) {
                        player1Stocks++; 
                        if(player1Stocks < 3) {
                            player1.GetComponent<PlayerController>().DeathSequence(player1Order[player1Stocks], spawn1.position, true);
                            player1Life = 3; 
                            fightUI.GetComponent<GameUIController>().SetHP(0, player1Life);
                            fightUI.GetComponent<GameUIController>().SetSisterPortrait(0, player1Order[player1Stocks]);

                        } else {
                            player2Wins++;
                            fightUI.GetComponent<GameUIController>().SetWins(1, player2Wins); 
                            ResetPlayers(); 
                        }
                    }
                    if(player2 && player2Life <= 0) {
                        player2Stocks++;
                        if(player2Stocks < 3) {
                            player2.GetComponent<PlayerController>().DeathSequence(player2Order[player2Stocks], spawn2.position, true);
                            player2Life = 3;
                            fightUI.GetComponent<GameUIController>().SetHP(1, player2Life);
                            fightUI.GetComponent<GameUIController>().SetSisterPortrait(1, player2Order[player2Stocks]);
                        } else { 
                            player1Wins++;
                            fightUI.GetComponent<GameUIController>().SetWins(0, player1Wins); 
                            ResetPlayers(); 
                        }
                    }
                    if(player1 && player2) {
                        if(player1.transform.position.x < player2.transform.position.x) {
                            player1.GetComponent<PlayerController>().onLeftSide = true;
                            player2.GetComponent<PlayerController>().onLeftSide = false;
                        } else {
                            player1.GetComponent<PlayerController>().onLeftSide = false;
                            player2.GetComponent<PlayerController>().onLeftSide = true;
                        }
                    }
                }
                break;
            case GameState.EndScreen:
                if(gameActive) {
                    fightUI.GetComponent<GameUIController>().ClearWins(); 
                    mapController.GetComponent<MapController>().Clear(); 
                    mainMenuObject.SetActive(true); 
                    mainMenuObject.GetComponent<MenuController>().EndScreen();
                    endScreenObject.SetActive(true); 
                    endScreenObject.GetComponent<WinnerScreen>().SetWinner(winnerId); 
                    GameObject.Find("Main Camera").GetComponent<CameraController>().DeactivateCamera(); 
                    fightUI.GetComponent<GameUIController>().CloseUp();
                    gameActive = false; 
                    gameOver = true; 
                    GameObject winnerPlayer;
                    if(winnerId == 1) {
                        winnerPlayer = player1; 
                    } else {
                        winnerPlayer = player2; 
                    }
                    winnerOrder = winnerPlayer.GetComponent<OrderPicker>().playerOrder; 
                    player1.GetComponent<PlayerController>().Reset(spawn1.position); 
                    player2.GetComponent<PlayerController>().Reset(spawn2.position); 
                }

                if(mainMenuObject.GetComponent<MenuController>().readyReset) {
                    gameState = GameState.Pregame; 
                    mainMenuObject.GetComponent<MenuController>().WakeUp(); 
                }
                break;
        }
    }

    public void JoinPlayer(PlayerInput pIn) {
        if(pIn.playerIndex == 0) {
            player1 = pIn.gameObject;
            player1.transform.position = spawn1.position; 
            pCon1 = player1.GetComponent<PlayerController>();
            pCon1.playerId = 1; 
        } else if(pIn.playerIndex == 1) {
            player2 = pIn.gameObject;
            player2.transform.position = spawn2.position; 
            pCon2 = player2.GetComponent<PlayerController>();
            pCon2.playerId = 2; 
        }
    }

    public void PlayerHit(int pId, int damage) {
        if(pId == 1) {
            player1Life -= damage; 
            fightUI.GetComponent<GameUIController>().SetHP(0, player1Life);
        }
        if(pId == 2) {
            player2Life -= damage; 
            fightUI.GetComponent<GameUIController>().SetHP(1, player2Life);
        }
    }

    public void BlockBack(int pId) {
        if(pId == 1) {
            pCon2.BlockBack();
        } else {
            pCon1.BlockBack(); 
        }
    }

    void ResetPlayers() {
        roundCountNumber++; 
        if(player2Wins > 2) {
            winnerId = 2; 
            gameState = GameState.EndScreen; 
        } else if(player1Wins > 2) {
            winnerId = 1; 
            gameState = GameState.EndScreen; 
        } else {
            // display the current round number
            fightUI.GetComponent<GameUIController>().DisplayRoundCount(roundCountNumber); 

            player1.GetComponent<PlayerController>().SetSister(player1Order[0], spawn1.position, false);
            fightUI.GetComponent<GameUIController>().SetSisterPortrait(0, player1Order[0]);
            player1Stocks = 0;
            player1Life = 3; 
            
            player2.GetComponent<PlayerController>().SetSister(player2Order[0], spawn2.position, false);
            fightUI.GetComponent<GameUIController>().SetSisterPortrait(1, player2Order[0]);
            player2Stocks = 0; 
            player2Life = 3; 
        }
    }
}