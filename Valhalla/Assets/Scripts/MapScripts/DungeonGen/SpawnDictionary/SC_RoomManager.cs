using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_RoomManager : MonoBehaviour
{
    public static SC_RoomManager single;

    public StandardRoomRots sRoomRots;
    [Space(10)]
    public AvailableSlots startRoom;
    public List<SC_RoomTest> allspawnedRooms = new List<SC_RoomTest>();
    public List<SC_RoomTest> allFinishedSpawningRooms = new List<SC_RoomTest>();
    public LayerMask overlapLayer;
    public LayerMask attachPointsLayer;
    public GameObject prefabWall;
    public float spawnDelay;
    public int MaxamountOfRooms;

    public SC_RoomTest currRoomToCheck;
    public int attachment;
    [HideInInspector]
    public GameObject customDungeonParent;
    [HideInInspector]
    public int currentAmountOfRooms;
    public int rotationAmount;
    public bool currRoomInverted;

    private void Awake()
    {
        single = this;
    }

    public void CreateNewDungeon()
    {
        if (customDungeonParent != null)
        {
            Debug.LogWarning("Already a Dungeon Existing Destroy First");
        }
        customDungeonParent = new GameObject("Custom Dungeon");

        GameObject mainRoom = SC_RoomPooler.single.SpawnFromPool(startRoom, transform.position, Quaternion.identity);
        //RoomCheckApply

        SpawnRoom(mainRoom);

        GetNextAvaialableRoom();
    }

    public void DestroyDungeon()
    {
        if (customDungeonParent != null)
        {
            Destroy(customDungeonParent);
        }
    }

    public void GetNextAvaialableRoom()
    {
        for (int i = 0; i < allspawnedRooms.Count; i++)
        {
            currRoomToCheck = allspawnedRooms[i];
            if (!currRoomToCheck.fullyAttached)
            {
                StartCoroutine(CheckRoomAndSpawn(currRoomToCheck));
                break;
            }
            else
            {
                if (!allFinishedSpawningRooms.Contains(currRoomToCheck))
                {
                    allFinishedSpawningRooms.Add(currRoomToCheck);
                }
                //allspawnedRooms.Remove(allspawnedRooms[i]);
            }
        }
    }

    public IEnumerator CheckRoomAndSpawn(SC_RoomTest roomToCheck)
    {
        //Debug.Log(currentAmountOfRooms);
        if (currentAmountOfRooms < MaxamountOfRooms)
        {
            while (!CheckIfFullyAttached(roomToCheck))
            {
                //Debug.Log(roomToCheck.name);
                for (int i = 0; i < roomToCheck.attachPoints.Length; i++)
                {
                    attachment = i;
                    AttachPoints attachpoint = roomToCheck.attachPoints[i];

                    //Debug.Log(roomToCheck.name + " " + roomToCheck.attachPoints[i] + " " + i);
                    if (!attachpoint.attached)
                    {
                        yield return new WaitForSeconds(spawnDelay);
                        //Debug.Log(roomToCheck.name + " " + roomToCheck.attachPoints[i] + " " + i + "Is Not Attached");
                        GameObject pooledRoomObject = SC_RoomPooler.single.SpawnFromPool(attachpoint.nextSpawn, Vector3.zero, Quaternion.identity);
                        SC_RoomTest pooledRoom = pooledRoomObject.GetComponent<SC_RoomTest>();
                        AttachPoints[] aPpooledRoom = pooledRoom.attachPoints;
                        bool hasSpawned = false;
                        for (int j = 0; j < aPpooledRoom.Length; j++)
                        {
                            //Debug.Log("Attachpoint: " + j);
                            
                            yield return new WaitForSeconds(spawnDelay);
                            for (int l = 0; l < sRoomRots.presetRots.Length; l++)
                            {
                                CheckOffset(roomToCheck, attachpoint, pooledRoomObject, aPpooledRoom[j]);

                                Collider[] col = Physics.OverlapBox(pooledRoom.transform.position + pooledRoom.roomCollider.center, pooledRoom.roomCollider.bounds.extents * 0.5f, pooledRoomObject.transform.rotation, overlapLayer);
                                int overlapsWithSelf = CheckOverlapSelf(col, pooledRoom);
                            
                                if (col.Length == 0 || col.Length == overlapsWithSelf)
                                {
                                    CheckRot(l, pooledRoomObject);
                                    yield return new WaitForSeconds(spawnDelay);

                                    CheckAttachMent(roomToCheck);
                                    if (attachpoint.canBeAttached == true)
                                    {
                                        SpawnRoom(pooledRoomObject);
                                        hasSpawned = true;
                                        break;
                                    }
                                }
                                else if (col.Length > overlapsWithSelf)
                                {
                                    Debug.Log("Overlapping With ROom");
                                    InvertRoom(pooledRoomObject, attachpoint);
                                    yield return new WaitForSeconds(spawnDelay);
                                    CheckAttachMent(roomToCheck);
                                    break;
                                }
                            }
                            if (hasSpawned)
                            {
                                break;
                            }
                        }


                        if (!hasSpawned)
                        {
                            CheckAttachMent(roomToCheck);
                            if (!attachpoint.attached && attachpoint.wall != null)
                            {
                                attachpoint.wall.SetActive(true);
                                attachpoint.attached = true;
                            }
                            pooledRoomObject.SetActive(false);
                        }
                    }
                }
            }

            if (CheckIfFullyAttached(roomToCheck))
            {
                roomToCheck.fullyAttached = true;
            }

            if (roomToCheck.fullyAttached)
            {
                GetNextAvaialableRoom();
            }

        }
        else
        {
            Debug.Log("Dungeon Finished");
            foreach (SC_RoomTest test in allspawnedRooms)
            {
                foreach (AttachPoints points in test.attachPoints)
                {
                    if (!points.attached)
                    {
                        points.wall.SetActive(true);
                        CheckAttachMent(test);
                    }
                }
            }
        }

    }

    public void InvertRoom(GameObject pooledRoomObject,AttachPoints currentAttachpoint)
    {
        currRoomInverted = true;
        if (Mathf.Abs(currentAttachpoint.point.localPosition.x) < Mathf.Abs(currentAttachpoint.point.localPosition.z))
        {
            if(currentAttachpoint.point.localPosition.z < 0f)
            {
                Debug.Log("Rot 1");
                pooledRoomObject.transform.rotation = Quaternion.Euler(sRoomRots.presetRots[(int)Rots.ZMin].rot);
            }
            else
            {
                Debug.Log("Rot 2");
                pooledRoomObject.transform.rotation = Quaternion.Euler(sRoomRots.presetRots[(int)Rots.ZPlus].rot);
            }
        }
        else
        {
            if (currentAttachpoint.point.localPosition.x < 0f)
            {
                Debug.Log("Rot 3");
                pooledRoomObject.transform.rotation = Quaternion.Euler(sRoomRots.presetRots[(int)Rots.XMin].rot);
            }
            else
            {
                Debug.Log("Rot 4");
                pooledRoomObject.transform.rotation = Quaternion.Euler(sRoomRots.presetRots[(int)Rots.XPlus].rot);
            }
        }
    }

    public void CheckRot(int amount, GameObject pooledRoomObject)
    {
        pooledRoomObject.transform.rotation = Quaternion.Euler(sRoomRots.presetRots[amount].rot);
        rotationAmount++;
    }

    private void CheckOffset(SC_RoomTest roomToCheck, AttachPoints attachpoint, GameObject pooledRoomObject, AttachPoints aPpooledRoom)
    {
        Transform nrAttachPoint = aPpooledRoom.point;
        Vector3 newOffset = Vector3.zero;
        if (currRoomInverted)
        {
            newOffset = new Vector3(nrAttachPoint.localPosition.z, nrAttachPoint.localPosition.y, nrAttachPoint.localPosition.x);
            currRoomInverted = false;
        }
        else
        {
            newOffset = new Vector3(nrAttachPoint.localPosition.x, nrAttachPoint.localPosition.y, nrAttachPoint.localPosition.z);
        }
        pooledRoomObject.transform.position = attachpoint.point.position + newOffset;

        #region OldRotation
        //Vector3 testRot = GetRotation(attachpoint);
        //Vector3 offsetRoom = Vector3.zero;
        //Vector3 newPos = Vector3.zero;
        //Vector3 offsetAttachpoint = Vector3.zero;
        //Debug.Log("Rot :" + testRot);

        //if (testRot == sRoomRots.xMinRot || testRot == sRoomRots.xPlusRot)
        //{
        //    aPpooledRoom.rotated = true;
        //    Debug.Log("True Rotation");
        //}
        //else if(testRot == sRoomRots.zMinRot || testRot == sRoomRots.zPlusRot)
        //{
        //    aPpooledRoom.rotated = false;
        //    Debug.Log("No Rotation");
        //}

        //if (attachpoint.rotated)
        //{
        //    //Vector3 ReverseOffsetpooledRoom = new Vector3(aPpooledRoom.Off.z, aPpooledRoom.Off.y, aPpooledRoom.Off.x);
        //    //Vector3 ReverseOffsetAttachpoint = new Vector3(attachpoint.Off.z, attachpoint.Off.y, attachpoint.Off.x);
        //    offsetRoom = aPpooledRoom.Off;
        //    offsetAttachpoint = attachpoint.Off;

        //    offsetRoom = transform.TransformPoint(aPpooledRoom.Off);
        //    offsetAttachpoint = transform.TransformPoint(attachpoint.Off);

        //    Debug.Log("Room Off WorldPos: " + offsetRoom);
        //    Debug.Log("Attachpoint Off WorldPos: " + offsetAttachpoint);
        //}
        //else
        //{
        //    offsetRoom = aPpooledRoom.Off;
        //    offsetAttachpoint = attachpoint.Off;

        //    offsetRoom = transform.TransformPoint(offsetRoom);
        //    offsetAttachpoint = transform.TransformPoint(offsetAttachpoint);

        //    Debug.Log("Attachpoint Off LocalPos: " + attachpoint.Off);
        //    Debug.Log("Room Off LocalPos: " + offsetRoom);
        //}

        //offsetRoom = aPpooledRoom.Off;
        //offsetAttachpoint = attachpoint.Off;

        //newPos = roomToCheck.transform.position + offsetAttachpoint + offsetRoom;

        //Debug.Log("RoomToCheck: " + roomToCheck.transform.position);
        ////Debug.Log(aPpooledRoom.Off + " On " + roomToCheck);
        //Debug.Log("NewPos " + newPos + " Off " + pooledRoomObject.name);

        //for (int i = 0; i < sRoomRots.presetRots.Length; i++)
        //{

        //}

        //pooledRoomObject.transform.position = newPos;

        //pooledRoomObject.transform.rotation = Quaternion.Euler(testRot);
        #endregion
    }

    int CheckOverlapSelf(Collider[] colliding,SC_RoomTest selfParent)
    {
        int amountOverlapping = 0;
        for (int i = 0; i < colliding.Length; i++)
        {
            //Debug.Log(selfParent.name + " Is CollidingWith " + colliding[i].gameObject.name);
            if (colliding[i].GetComponentInParent<SC_RoomTest>() == selfParent)
            {
                //Debug.Log(selfParent.name + " Is CollidingWith  Self");
                amountOverlapping++;
            }
        }
        return amountOverlapping;
    }

    public void SpawnRoom(GameObject room)
    {
        //Debug.Log("SpawningRoom");
        SC_RoomTest pooledRoom = room.GetComponent<SC_RoomTest>();

        foreach (AttachPoints roomPoints in pooledRoom.attachPoints)
        {
            roomPoints.attached = false;
            roomPoints.attachedTo = null;
        }

        GameObject newRoom = Instantiate(room);
        SC_RoomTest newRoomScript = newRoom.GetComponent<SC_RoomTest>();

        newRoomScript.isChecker = false;
        newRoom.transform.SetParent(customDungeonParent.transform);
        allspawnedRooms.Add(newRoomScript);

        if(newRoomScript.roomType == AvailableSlots.SmallRooms)
        {
            currentAmountOfRooms++;
        }

        room.SetActive(false);
        rotationAmount = 0;
        CheckAttachMent(newRoomScript);
    }

    public void CheckAttachMent(SC_RoomTest ownerRoom)
    {
        AttachPoints[] attachments = ownerRoom.attachPoints;

        for (int i = 0; i < attachments.Length; i++)
        {
            AttachPoints attachPoint = attachments[i];
            Collider[] col = Physics.OverlapBox(attachPoint.point.position + attachPoint.attachCollider.center, attachPoint.attachCollider.bounds.extents * 0.5f, attachPoint.point.rotation, attachPointsLayer);
            int overlapsWithSelf = CheckOverlapSelf(col, ownerRoom);

            //Debug.Log(ownerRoom.name + " Has " + col.Length + " Collisions In Total ");
            //Debug.Log(ownerRoom.name + " Has " + overlapsWithSelf + " Collisions With Self");

            for (int k = 0; k < col.Length; k++)
            {
                if (col[k].GetComponentInParent<SC_RoomTest>() != ownerRoom)
                {
                    SC_RoomTest otherRoom = col[k].GetComponentInParent<SC_RoomTest>();
                    for (int j = 0; j < otherRoom.attachPoints.Length; j++)
                    {
                        if(col[k] == otherRoom.attachPoints[j].attachCollider && !attachPoint.attached)
                        {
                            attachPoint.canBeAttached = true;
                            if (!otherRoom.isChecker)
                            {
                                Destroy(otherRoom.attachPoints[j].wall);

                                attachPoint.attached = true;
                                attachPoint.attachedTo = otherRoom;

                                //Debug.Log(otherAttachPoint.name + " Is Attached");


                                otherRoom.attachPoints[j].attached = true;
                                otherRoom.attachPoints[j].attachedTo = ownerRoom;
                                otherRoom.attachPoints[j].wall = attachPoint.wall;
                                break;
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    bool CheckIfFullyAttached(SC_RoomTest roomToCheck)
    {
        bool allAttached = true;
        AttachPoints[] attachPoints = roomToCheck.attachPoints;
        for (int i = 0; i < attachPoints.Length; i++)
        {
            if (!attachPoints[i].attached)
                allAttached = false;
        }
        return allAttached;
    }

    #region GetRot
    Vector3 GetRotation(AttachPoints currentAttachpoint)
    {
        Vector3 sRot = Vector3.zero;
        //Debug.Log(currentAttachpoint.point.localPosition.x + " " + currentAttachpoint.point.localPosition.z);

        if (Mathf.Abs(currentAttachpoint.point.position.x) < Mathf.Abs(currentAttachpoint.point.position.z))
        {
            //Debug.Log(currentAttachpoint.point.localPosition.z + " " + currentAttachpoint.point.localPosition.z);

            if(currentAttachpoint.point.position.z < 0f)
            {
                //sRot = sRoomRots.zMinRot;
            }
            else
            {
                //sRot = sRoomRots.zPlusRot;
            }
        }
        else
        {
            //Debug.Log(currentAttachpoint.point.localPosition.x);
            if (currentAttachpoint.point.position.x < 0f)
            {
                //sRot = sRoomRots.xMinRot;
            }
            else
            {
                //sRot = sRoomRots.xPlusRot;
            }
        }

        return sRot;
    }
    #endregion
}

public enum Rots
{
    XPlus,
    XMin,
    ZPlus,
    ZMin
}

[System.Serializable]
public class PresetRoomRots
{
    public Rots axisRot;
    public Vector3 rot;
}

[System.Serializable]
public class StandardRoomRots
{
    public PresetRoomRots[] presetRots;
}

public interface IPosCheck
{
    bool InsideBounds(Transform attachPoint);
}
