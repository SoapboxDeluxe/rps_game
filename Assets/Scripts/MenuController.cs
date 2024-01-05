using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {
    [SerializeField] List<GameObject> menuOptions = new List<GameObject>(); 
    [SerializeField] GameObject togButton; 
    [SerializeField] float menuCooldown; 
    public bool readyArcade, readyVersus, readyReset;
    bool endScreen;  
    int selection = 0; 
    [SerializeField] bool menuCD = false; 
    [SerializeField] public bool menuActive = false; 

    public void WakeUp() {
        menuActive = true; 
        endScreen = false;
        readyArcade = false;
        readyVersus = false; 
        readyReset = false; 
        StartCoroutine("MenuCooldown"); 
    }

    public void EndScreen() {
        menuActive = false; 
        endScreen = true; 
        readyArcade = false;
        readyVersus = false; 
        readyReset = false; 
        StartCoroutine("MenuCooldown"); 
    }

    void Update() {
        togButton.transform.position = new Vector3(-2f, menuOptions[selection].transform.position.y, 0f); 
    }

    public void AdjustSelection(bool up) {
        if(!menuCD && menuActive) {
            if(up) {
                selection++; 
            } else {
                selection--;
            }
            StartCoroutine("MenuCooldown");
        }
        
        if(selection >= menuOptions.Count) {
            selection = menuOptions.Count-1;
        }
        if(selection < 0) {
            selection = 0;
        }
    }

    public void Select() {
        if(!menuCD && menuActive) {
            string sName = menuOptions[selection].name;
            StartCoroutine("MenuCooldown");
            if(sName == "ARCADE") {
                readyArcade = true; 
            }
            if(sName == "VERSUS") {
                readyVersus = true; 
            }
        }
        if(endScreen) {
            readyReset = true; 
        }
    }

    private IEnumerator MenuCooldown() { menuCD = true; yield return new WaitForSeconds(menuCooldown); menuCD = false; }
}
