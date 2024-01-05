using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; 

public class OrderPicker : MonoBehaviour {

    [SerializeField] GameObject orderPickerObject; 
    MenuController menu; 
    public bool readyStart, active;
    public bool mainPlayer = false;
    public List<int> playerOrder = new List<int>(); 
    bool startIn, rockIn, paperIn, scissorsIn, canPick = true;
    [SerializeField] float pickCooldown; 
    [SerializeField] GameObject[] labels; 
    int pickCount = 0; 

    void Awake() {
        StartCoroutine(PickCooldown()); 
        if(GameObject.Find("s_mainmenu")) {
            menu = GameObject.Find("s_mainmenu").GetComponent<MenuController>(); 
        }
    }

    void Update() {
        if(active) {
            orderPickerObject.SetActive(true); 
            if(pickCount > 2) {
                canPick = false; 
                readyStart = true; 
            }
            if(canPick) {
                if(rockIn) {
                    playerOrder.Add(0); 
                    labels[pickCount].GetComponent<TextMeshPro>().text = "R"; 
                    pickCount++; 
                    StartCoroutine(PickCooldown()); 
                } else if(paperIn) {
                    playerOrder.Add(1); 
                    labels[pickCount].GetComponent<TextMeshPro>().text = "P"; 
                    pickCount++; 
                    StartCoroutine(PickCooldown());
                } else if(scissorsIn) {
                    playerOrder.Add(2);
                    labels[pickCount].GetComponent<TextMeshPro>().text = "S"; 
                    pickCount++; 
                    StartCoroutine(PickCooldown());
                }
            } 
            if(!mainPlayer) {
                orderPickerObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, .7f, 0f); 
            }
        } else {
            orderPickerObject.SetActive(false); 
        }
    }

    public void StartInput(InputAction.CallbackContext c) {
        if(c.ReadValue<float>() > 0) {
            startIn = true; 
        } else {
            startIn = false; 
        }
    }

    public void StartupDelay() {
        StartCoroutine(PickCooldown());
    }

    public void Deactivate() {
        orderPickerObject.SetActive(false); 
    }

    public void Activate() {
        orderPickerObject.SetActive(true); 
        StartCoroutine(PickCooldown()); 
        pickCount = 0; 
        readyStart = false; 
        playerOrder.Clear(); 
        labels[0].GetComponent<TextMeshPro>().text = ""; 
        labels[1].GetComponent<TextMeshPro>().text = ""; 
        labels[2].GetComponent<TextMeshPro>().text = ""; 
    }

    public List<int> DefaultOrder() {
        List<int> dO = new List<int>() {1, 1, 2}; 
        return dO; 
    }

    public void UpInput(InputAction.CallbackContext c) {
        if(mainPlayer) {
            menu.AdjustSelection(false);
        }
    }

    public void DownInput(InputAction.CallbackContext c) {
        if(mainPlayer) {
            menu.AdjustSelection(true);
        }
    }
    
    public void SelectInput(InputAction.CallbackContext c) {
        if(mainPlayer) {
            menu.Select(); 
        }
    }    

    public void RockInput(InputAction.CallbackContext c) {
        if(c.ReadValue<float>() > 0) {
            rockIn = true; 
        } else {
            rockIn = false; 
        }
    }

    public void PaperInput(InputAction.CallbackContext c) {
        if(c.ReadValue<float>() > 0) {
            paperIn = true; 
        } else {
            paperIn = false; 
        }
    }

    public void ScissorsInput(InputAction.CallbackContext c) {
        if(c.ReadValue<float>() > 0) {
            scissorsIn = true; 
        } else {
            scissorsIn = false; 
        }
    }
    private IEnumerator PickCooldown() { canPick = false; yield return new WaitForSeconds(pickCooldown); canPick = true; }
}