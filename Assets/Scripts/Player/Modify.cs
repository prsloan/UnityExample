using UnityEngine;
using System.Collections;

public class Modify : MonoBehaviour {

    Vector2 rot;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 100)) {
                EditTerrain.SetBlock(hit, new BlockAir());
            }
        }

       
    }
}