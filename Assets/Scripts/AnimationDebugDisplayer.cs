using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AnimationDebugDisplayer : MonoBehaviour {
    [SerializeField]
    float offset; 

    void Update() {
        int i = 0; 
        foreach(Transform a in transform) {
            a.position = new Vector3(transform.position.x + ((i +1) * offset), transform.position.y, 0f); 
            i++;
        }
    }
}
