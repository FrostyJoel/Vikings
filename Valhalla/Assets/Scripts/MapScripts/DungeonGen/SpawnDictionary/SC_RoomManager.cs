using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_RoomManager : MonoBehaviour
{
    public static SC_RoomManager single;
    public StandardRoomRots sRoomRots;
    [Space (10)]
    public Vector3 mapSize;
    public AvailableSlots startRoom;
    public List<SC_RoomTest> allspawnedRooms = new List<SC_RoomTest>();
    public LayerMask overlapLayer;
    public GameObject wall;

    [HideInInspector]
    public GameObject customDungeonParent;

    private void Awake()
    {
        single = this;
    }

    public void SpawnStart()
    {
        if (GameObject.Find("Custom Dungeon"))
        {
            Destroy(GameObject.Find("Custom Dungeon"));
        }
        customDungeonParent = new GameObject("Custom Dungeon");

        GameObject mainRoom = SC_RoomPooler.single.SpawnFromPool(startRoom, GetCenter(mapSize), Quaternion.identity);
        //RoomCheckApply

        StartCoroutine(SpawnRoom(mainRoom));
    }

    public void CheckRoomAndSpawn(SC_RoomTest roomToCheck)
    {
        for (int i = 0; i < roomToCheck.attachPoints.Length; i++)
        {
            AttachPoints attachpoint = roomToCheck.attachPoints[i];

            if (!attachpoint.attached)
            {
                GameObject pooledRoomObject = SC_RoomPooler.single.SpawnFromPool(attachpoint.nextSpawn, Vector3.zero, Quaternion.identity);
                SC_RoomTest pooledRoom = pooledRoomObject.GetComponent<SC_RoomTest>();
                AttachPoints[] aPpooledRoom = pooledRoom.attachPoints;
                bool hasSpawned = false;

                for (int j = 0; j < aPpooledRoom.Length; j++)
                {
                    Vector3 offsetRoom = aPpooledRoom[j].Off;
                    Vector3 newPos = roomToCheck.transform.position + attachpoint.Off + offsetRoom;
                    pooledRoomObject.transform.position = newPos;
                    pooledRoomObject.transform.rotation = Quaternion.Euler(GetRotation(attachpoint));
                    aPpooledRoom[j].hasOffset = false;

                    Collider[] col = Physics.OverlapBox(pooledRoom.transform.position + pooledRoom.roomCollider.center, pooledRoom.roomCollider.bounds.extents * 0.5f, pooledRoomObject.transform.rotation, overlapLayer);
                    int overlapsWithSelf = CheckOverlapSelf(col, pooledRoom);
                    
                    if (col.Length == 0 || col.Length == overlapsWithSelf)
                    {
                        //Debug.Log("Not Colliding with Room");
                        StartCoroutine(SpawnRoom(pooledRoomObject));
                        attachpoint.attached = true;
                        hasSpawned = true;
                        break;
                    }
                    else if(col.Length > overlapsWithSelf)
                    {
                        //Debug.Log("Colliding with Room");
                    }
                }
                if (!hasSpawned)
                {
                    roomToCheck.CheckAttachMent();
                    if (!attachpoint.attached)
                    {
                       SpawnWall(attachpoint.point);
                    }
                    pooledRoomObject.SetActive(false);
                }
            }
        }
    }

    int CheckOverlapSelf(Collider[] colliding,SC_RoomTest selfParent)
    {
        int amountOverlapping = 0;
        for (int i = 0; i < colliding.Length; i++)
        {
            if (colliding[i].GetComponentInParent<SC_RoomTest>() == selfParent)
            {
                amountOverlapping++;
            }
        }
        //print(selfParent.gameObject.name + "Is Colliding With " + amountOverlapping);
        return amountOverlapping;
    }

    public void SpawnWall(Transform wallPos)
    {
        GameObject newWall = Instantiate(wall, wallPos.position, wallPos.rotation);
        newWall.transform.SetParent(customDungeonParent.transform);
    }

    public IEnumerator SpawnRoom(GameObject room)
    {
        //Debug.Log("SpawningRoom");
        
        GameObject newRoom = Instantiate(room);
        room.SetActive(false);
        newRoom.GetComponent<SC_RoomTest>().CheckAttachMent();

        newRoom.transform.SetParent(customDungeonParent.transform);
        yield return new WaitForSeconds(1f);
        IInit init = newRoom.GetComponent<IInit>();
        if (init != null)
        {
            init.Init();
        }
    }

    #region GetRot
    Vector3 GetRotation(AttachPoints currentAttachpoint)
    {
        Vector3 sRot = Vector3.zero;
        //Debug.Log(currentAttachpoint.point.localPosition.x + " " + currentAttachpoint.point.localPosition.z);

        if (Mathf.Abs(currentAttachpoint.point.localPosition.x) < Mathf.Abs(currentAttachpoint.point.localPosition.z))
        {
            //Debug.Log(currentAttachpoint.point.localPosition.z + " " + currentAttachpoint.point.localPosition.z);

            if(currentAttachpoint.point.localPosition.z < 0f)
            {
                sRot = sRoomRots.zMinRot;
            }
            else
            {
                sRot = sRoomRots.zPlusRot;
            }
        }
        else
        {
            //Debug.Log(currentAttachpoint.point.localPosition.x);
            if (currentAttachpoint.point.localPosition.x < 0f)
            {
                sRot = sRoomRots.xMinRot;
            }
            else
            {
                sRot = sRoomRots.xPlusRot;
            }
        }

        return sRot;
    }
    #endregion

    #region GetPos

    public bool InsideBounds(Transform attachPoint, Vector3 size)
    {
        bool isInside = false;
        if (CompareVectors(size, attachPoint.position))
        {
            isInside = true;
        }
        return isInside;
    }

    bool CompareVectors(Vector3 center, Vector3 compare)
    {
        bool isInside = false;
        if (CompareFloat(center.x, compare.x) && CompareFloat(center.y, compare.y) && CompareFloat(center.z, compare.z))
        {
            isInside = true;
        }
        return isInside;
    }

    bool CompareFloat(float boundingFloat, float compareFloat)
    {
        bool isInside = false;
        if (compareFloat < boundingFloat && compareFloat > 0)
        {
            isInside = true;
        }
        return isInside;
    }

    Vector3 GetCenter(Vector3 bounds)
    {
        Vector3 center = Vector3.zero;
        center = new Vector3(bounds.x, bounds.y, bounds.z) * 0.5f;
        return center;
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(GetCenter(mapSize), mapSize);
    }
}

[System.Serializable]
public class StandardRoomRots
{
    public Vector3 xPlusRot, xMinRot, zPlusRot, zMinRot;
}

public interface IPosCheck
{
    bool InsideBounds(Transform attachPoint);
}
