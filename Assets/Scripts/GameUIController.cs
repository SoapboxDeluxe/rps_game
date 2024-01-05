using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro; 

public class GameUIController : MonoBehaviour {
    // Start is called before the first frame update
    [SerializeField] GameObject fightUICanvas; 
    [SerializeField] GameObject p1hp, p2hp;
    [SerializeField] GameObject p1sp, p2sp;
    [SerializeField] GameObject rc; 
    [SerializeField] GameObject[] p1wins, p2wins; 
    [SerializeField] GameObject[] p1backups, p2backups;
    [SerializeField] Sprite[] hpsprites; 
    [SerializeField] Sprite[] sissprites; 

    List<int> p1order, p2order; 

    public void TwoPSetup(List<int> p1, List<int> p2) {
        Debug.Log("activate fight ui");
        fightUICanvas.GetComponent<Canvas>().enabled = true;
        p1hp.GetComponent<Image>().sprite = hpsprites[3];
        p2hp.GetComponent<Image>().sprite = hpsprites[3];
        p1order = p1;
        p2order = p2; 
        SetSisterPortrait(0, p1order[0]);
        SetSisterPortrait(1, p2order[0]);
    }

    public void CloseUp() {
        fightUICanvas.GetComponent<Canvas>().enabled = false;
    }

    public void SetHP(int p, int h) {
        int hs = h;
        if(hs < 0) {
            hs = 0; 
        }
        Sprite newHP = hpsprites[hs];
        if(p == 0) {
            p1hp.GetComponent<Image>().sprite = newHP;
        } else if(p == 1) {
            p2hp.GetComponent<Image>().sprite = newHP;
        }
    }

    public void SetWins(int p, int w) {
        if(p == 0) {
            for(int i = 0; i < p1wins.Length; i++) {
                if(i < w) {
                    p1wins[i].SetActive(true); 
                } else {
                    p1wins[i].SetActive(false); 
                }
            }
        } else if (p == 1) {
            for(int i = 0; i < p2wins.Length; i++) {
                if(i < w) {
                    p2wins[i].SetActive(true); 
                } else {
                    p2wins[i].SetActive(false); 
                }
            }
        }
    }

    public void DisplayRoundCount(int c) {
        // Ensure the GameObject and the TextMeshPro component are not null
        if (rc != null) {
            var textMesh = rc.GetComponent<TextMeshProUGUI>(); 
            if (textMesh != null) {
                textMesh.text = "Round " + c; 
                StartCoroutine(RoundCountDisplay());
            } else {
                Debug.LogError("TextMeshPro component not found on the GameObject.");
            }
        } else {
            Debug.LogError("GameObject rc is not assigned.");
        }
    }

    public void ClearWins() {
        for(int i = 0; i < p1wins.Length; i++) {
            p1wins[i].SetActive(false); 
            p2wins[i].SetActive(false); 
        }
    }

    public void SetSisterPortrait(int p, int s) {
        int ss = s;
        if(ss < 0) {
            ss = 0;
        }
        Sprite newSis = sissprites[ss];
        if(p == 0) {
            p1sp.GetComponent<Image>().sprite = newSis;
        } else if(p == 1) {
            p2sp.GetComponent<Image>().sprite = newSis;
        }
    }

    private IEnumerator RoundCountDisplay() {
        rc.SetActive(true);
        yield return new WaitForSeconds(2f);
        rc.SetActive(false); 
    }
}
