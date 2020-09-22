using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxRoomManager : MonoBehaviour {
    public static MaxRoomManager single_MRM;

    public Vector3[] rotations;
    [Space(20)]
    public float delay;

    private void Awake() {
        single_MRM = this;
    }
}