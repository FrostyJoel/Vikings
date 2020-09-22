using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxRoom : MonoBehaviour {
    public Transform[] attachPoints;
    [Space(20)]
    public GameObject nextRoom;
    
    [Space]
    public bool first;
    MaxRoomManager mrm;

    private void Start() {
        if (first == true) {
            StartCoroutine(Init());
        }
    }

    public IEnumerator Init() {
        if (nextRoom) {
            mrm = MaxRoomManager.single_MRM;
            GameObject room = Instantiate(nextRoom);
            room.SetActive(true);
            MaxRoom newRoom = room.GetComponent<MaxRoom>();

            for (int i = 0; i < attachPoints.Length; i++) {
                print(gameObject.name + "my points");
                for (int iB = 0; iB < newRoom.attachPoints.Length; iB++) {
                    print(gameObject.name + "new room points");
                    for (int iC = 0; iC < MaxRoomManager.single_MRM.rotations.Length; iC++) {
                        print(gameObject.name + " Rot and pos " + MaxRoomManager.single_MRM.rotations[iC]);
                        Transform nrAttachPoint = newRoom.attachPoints[iB];
                        Vector3 newOffset = new Vector3(nrAttachPoint.localPosition.x, nrAttachPoint.localPosition.y, nrAttachPoint.localPosition.z);
                        newRoom.transform.position = attachPoints[i].position + newOffset;
                        newRoom.transform.rotation = Quaternion.Euler(MaxRoomManager.single_MRM.rotations[iC]);
                        yield return new WaitForSeconds(mrm.delay);
                    }
                }
            }
            StartCoroutine(newRoom.Init());
        }
    }
}