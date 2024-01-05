using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerScreen : MonoBehaviour {
    [SerializeField] GameObject p1Wins, p2Wins; 

    public void SetWinner(int p) {
        p1Wins.SetActive(false);
        p2Wins.SetActive(false);
        if(p == 1) {
            p1Wins.SetActive(true);
        }
        if(p == 2) {
            p2Wins.SetActive(true); 
        }
    }
}
