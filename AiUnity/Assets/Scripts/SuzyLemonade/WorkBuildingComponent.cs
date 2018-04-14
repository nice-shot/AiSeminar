using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkBuildingComponent : MonoBehaviour {

    public GameObject rDoor;
    public GameObject lDoor;
    public float doorMovement = 1.25f;
    public float doorSpeed = 8f;

    private bool someoneNearEntrance = false;

    void OnTriggerStay(Collider other) {
        someoneNearEntrance = true;
    }

    void OnTriggerExit(Collider other) {
        someoneNearEntrance = false;
    }

    void Update() {
        if (someoneNearEntrance) {
            OpenDoors();
        } else {
            CloseDoors();
        }
    }

    void OpenDoors() {
        MoveDoor(rDoor, doorMovement);
        MoveDoor(lDoor, -doorMovement);
    }

    void CloseDoors() {
        MoveDoor(rDoor, 0f);
        MoveDoor(lDoor, 0f);
    }

    // Moves door on the X axis to the given position
    void MoveDoor(GameObject door, float xPos) {
        if (door.transform.localPosition.x == xPos) {
            // Already at target position
            return;
        }

        float currentX = Mathf.Lerp(door.transform.localPosition.x, xPos, doorSpeed * Time.deltaTime);
        Vector3 movePos = new Vector3(currentX, 0f, 0f);
        door.transform.localPosition = movePos;
    }

}
