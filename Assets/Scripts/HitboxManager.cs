using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxManager : MonoBehaviour {
    // Start is called before the first frame update
    [SerializeField] PolygonCollider2D baseCollider, attackCollider; 
    [SerializeField] PolygonCollider2D[] hiAttackHitboxes, airAttackHitboxes, lowAttackHitboxes; 
    FightManager game; 
    public HitDetection.HitType hitType; 

    public void SetHitBox(AnimationEvent e) {
        PolygonCollider2D[] hb = new PolygonCollider2D[0]; 
        switch(e.stringParameter) {
            case "hi":
                hitType = HitDetection.HitType.High;
                hb = hiAttackHitboxes; 
                break;
            case "air":
                hitType = HitDetection.HitType.Air;
                hb = airAttackHitboxes; 
                break;
            case "low":
                hitType = HitDetection.HitType.Low;
                hb = lowAttackHitboxes; 
                break;
            case "clear":
                hitType = HitDetection.HitType.Clear; 
                attackCollider.pathCount = 0;
                break; 
        }
        if(hb.Length > 0) {
            attackCollider.SetPath(0, hb[e.intParameter].GetPath(0));
        }
    }

    public void SetBlocking(AnimationEvent e) {
        transform.parent.GetComponent<PlayerController>().blocking = e.intParameter;
    }
}
