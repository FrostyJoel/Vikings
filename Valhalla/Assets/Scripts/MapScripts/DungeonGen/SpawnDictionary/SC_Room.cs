using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Room : MonoBehaviour
{
    public AttachPoint[] attachPoints;
    public BoxCollider roomCollider;
    public AvailableSlots roomType;

    [Header("HideInInspector")]
    public List<SpawnablePosAndRot> spawnablePosAndRots = new List<SpawnablePosAndRot>();
    public bool fullyAttached;
    public bool isChecker;
    public bool isChecked;
    public bool isMinHorizontal;
    public bool isPlusHorizontal;
    public bool isMinVertical;
    public bool isPlusVertical;

    private void OnDrawGizmosSelected()
    {
        //if (attachPoints.Length > 0)
        //{
        //    foreach (AttachPoint attachPoint in attachPoints)
        //    {
        //        Gizmos.color = Color.red;
        //        Gizmos.DrawWireCube(attachPoint.attachCollider.transform.position + attachPoint.attachCollider.center, attachPoint.attachCollider.size * 2f);
        //    }
        //}
        //else
        //{
        //    Debug.LogError("No Attachpoints Assigned to " + gameObject.name);
        //}

        if (roomCollider)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(roomCollider.transform.position + roomCollider.center, roomCollider.size);
        }
        else
        {
            Debug.LogError("No RoomCollider Assigned to " + gameObject.name);
        }
    }
}

[System.Serializable]
public class SpawnablePosAndRot
{
    public Vector3 pos;
    public Quaternion rot;
    public GameObject room;
}

[System.Serializable]
public class AttachPoint
{
    public Transform point;
    [Header ("HideInInspector")]
    public GameObject wall;
    public BoxCollider attachCollider
    {
        get
        {
            BoxCollider myCollider = new BoxCollider();
            if (point)
            {
                myCollider = point.GetComponent<BoxCollider>();
            }
            return myCollider;
        }
    }
    public AvailableSlots nextSpawn;
    public bool attached;
    public bool canBeAttached;
    public SC_Room attachedTo;
    public Vector3 off;
}
