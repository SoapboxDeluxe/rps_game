using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXController : MonoBehaviour {

    private Animator anim;

    void Awake() {
        anim = GetComponent<Animator>(); 
    }

    public void PlayFX(int fxId) {
        gameObject.SetActive(true); 
        switch(fxId) {
            case 0:
                anim.SetTrigger("hithigh");
                break;
            case 1:
                anim.SetTrigger("hitlow");
                break;
            case 2:
                anim.SetTrigger("hitair");
                break;        
            case 3:
                anim.SetTrigger("blockhi");
                break;
            case 4:
                anim.SetTrigger("blocklow");
                break;      
            case 5:
                anim.SetTrigger("jump");
                break;      
            case 6:
                anim.SetTrigger("dash");
                break;      
            case 7:
                anim.SetTrigger("land");
                break;      
        }
        StartCoroutine(DisableObjectAfterDelay());
    }

    IEnumerator DisableObjectAfterDelay() {
        yield return new WaitForSeconds(.4f);
        gameObject.SetActive(false);
    }
}
