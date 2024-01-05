using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour {

    [SerializeField] private GameObject fxPrefab;
    private List<GameObject> fxPool = new List<GameObject>();   

    private void Start() {
        // Instantiate initial pool objects
        GameObject fxParent = new GameObject(); 
        for (int i = 0; i < 50; i++) {
            GameObject fx = Instantiate(fxPrefab, fxParent.transform);
            fx.SetActive(false);
            fxPool.Add(fx);
        }
    }

    public void GetFX(int type, bool fR) {
        GameObject fx = null;
        for (int i = 0; i < fxPool.Count; i++) {
            if (!fxPool[i].activeSelf) {
                fx = fxPool[i];
                break;
            }
        }
        // display the fx in the right place if all has gone according to plan
        if(fx) {
            fx.transform.position = transform.position; 
            fx.GetComponent<SpriteRenderer>().flipX = fR; 
            fx.GetComponent<FXController>().PlayFX(type);

        }
    }
}
