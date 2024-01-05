using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public bool fightActive = false;
    [SerializeField] Transform target; 
    [SerializeField] float cameraSmoothSpeed; 
    [SerializeField] Vector3 offset; 

    public Transform player1, player2; 

    public void ActivateCamera(Transform p1, Transform p2) {
        fightActive = true; 
        player1 = p1;
        player2 = p2; 
    }

    public void DeactivateCamera() {
        fightActive = false; 
        player1 = null;
        player2 = null; 
        transform.position = new Vector3(0f, 0f, -10f); 
        target.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update() {
        if(fightActive) {
            Vector3 center = (player1.position + player2.position) / 2;
            target.position = center + offset;
            transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, target.position.y, -10f), cameraSmoothSpeed * Time.deltaTime); 
        } else {
            transform.position = Vector3.Lerp(transform.position, new Vector3(0f, 0f, -10f), cameraSmoothSpeed * Time.deltaTime); 
        }
        if(Mathf.Abs(transform.position.x) > 4.75f) {
            if(transform.position.x > 0) {
                transform.position = new Vector3(4.75f, transform.position.y, transform.position.z); 
            } else {
                transform.position = new Vector3(-4.75f, transform.position.y, transform.position.z); 
            }
        }
    }
}
