using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationLocker : MonoBehaviour {
    public bool moveLocked, flipLocked; 
    public void SetMotionLock(int a) {
        if(a == 0) {
            moveLocked = false;
        } else {
            moveLocked = true; 
        }
    }

    public void SetFlipLock(int f) {
        if(f == 0) {
            flipLocked = false;
        } else {
            flipLocked = true; 
        }
    }
}