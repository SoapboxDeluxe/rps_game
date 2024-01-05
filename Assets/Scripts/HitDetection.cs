using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour {
    PlayerController pCon; 
    public enum HitType{
        High,
        Low,
        Air,
        Clear
    }

    void Awake() {
        pCon = GetComponent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.tag == "Hitbox") {
            Vector3 hDir = transform.position - col.transform.position; 
            hDir = hDir.normalized;
            HitType hT = col.gameObject.transform.parent.GetComponent<HitboxManager>().hitType; 
            pCon.Damage(hDir, hT, col.gameObject.transform.parent.transform.parent.GetComponent<PlayerController>().activeSisterId); 
        }
    }
}
