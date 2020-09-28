using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Rooms : MonoBehaviour
{
    public AttachPoints[] attachPoints;
    public BoxCollider roomCollider;
    public AvailableSlots roomType;

    [Header("HideInInspector")]
    public List<SpawnablePosAndRot> spawnablePosAndRots = new List<SpawnablePosAndRot>();
    public bool fullyAttached;
    public bool isChecker;
    public bool isClosed;

    private void OnDrawGizmosSelected()
    {
        if (attachPoints.Length > 0)
        {
            foreach (AttachPoints attachPoint in attachPoints)
            {
                Gizmos.DrawWireCube(attachPoint.point.position + attachPoint.attachCollider.center, attachPoint.attachCollider.bounds.extents * 2f);
            }
        }
        else
        {
            Debug.LogError("No Attachpoints Assigned to " + gameObject.name);
        }

        if (roomCollider)
        {
            Gizmos.DrawWireCube(transform.position + roomCollider.center, roomCollider.bounds.extents * 2f);
        }
        else
        {
            Debug.LogError("No RoomCollider Assigned to " + gameObject.name);
        }
    }

    public void ResetAttachPoint()
    {
        for (int i = 0; i < attachPoints.Length; i++)
        {
            attachPoints[i].Off = attachPoints[i].point.localPosition;
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
public class AttachPoints
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
    public SC_Rooms attachedTo;
    public Vector3 Off;
}
