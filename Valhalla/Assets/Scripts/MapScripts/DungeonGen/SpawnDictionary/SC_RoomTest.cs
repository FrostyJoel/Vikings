using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_RoomTest : MonoBehaviour, IInit
{
    public AttachPoints[] attachPoints;
    public BoxCollider roomCollider;
    public LayerMask attachPointsLayer;

    public void Init()
    {
        print("I Did Init");
        SC_RoomManager.single.CheckRoomAndSpawn(this);
    }

    public void CheckAttachMent()
    {
        for (int i = 0; i < attachPoints.Length; i++)
        {
            Collider[] col = Physics.OverlapBox(attachPoints[i].point.position + attachPoints[i].attachCollider.center, attachPoints[i].attachCollider.bounds.extents * 0.5f, attachPoints[i].point.rotation, attachPointsLayer);
            for (int k = 0; k < col.Length; k++)
            {
                if (col.Length > 1)
                {
                    attachPoints[i].attached = true;
                }
                else
                {
                    attachPoints[i].attached = false;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (roomCollider)
        {
            Gizmos.DrawWireCube(transform.position + roomCollider.center, roomCollider.bounds.extents*2f);
        }
        else
        {
            Debug.LogError("No RoomCollider Assigned to " + gameObject.name);
        }
    }
}


public interface IInit
{
    void Init();
}

[System.Serializable]
public class AttachPoints
{
    public Transform point;
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
    public bool hasOffset;
    public bool attached;
    private Vector3 def = Vector3.zero;
    public Vector3 Off
    {
        get
        {
            Vector3 v = def;
            if (!hasOffset)
            {
                if (point)
                {
                    hasOffset = true;
                    v = point.transform.localPosition;
                    def = v;
                }
            }
            return v;
        }
    }
}
