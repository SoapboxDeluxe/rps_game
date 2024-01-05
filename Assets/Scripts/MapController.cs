using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {
    [SerializeField] List<GameObject> stages = new List<GameObject>(); 

    public void Activate(int stageNumber) {
        if(stageNumber == 0) {
            stageNumber = Random.Range(0, 3); 
        }
        stages[stageNumber].SetActive(true); 
    }

    public void Clear() {
        foreach(GameObject stage in stages) {
            stage.SetActive(false); 
        }
    }
}