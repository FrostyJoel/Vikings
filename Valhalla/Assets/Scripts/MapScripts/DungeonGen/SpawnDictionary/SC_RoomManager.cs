using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_RoomManager : MonoBehaviour
{
    public static SC_RoomManager single;

    public StandardRoomRots sRoomRots;
    [Space(10)]
    public AvailableSlots startRoom;
    public List<SC_Rooms> allspawnedRooms = new List<SC_Rooms>();
    public List<SC_Rooms> allFinishedSpawningRooms = new List<SC_Rooms>();
    public LayerMask overlapLayer;
    public LayerMask attachPointsLayer;
    public GameObject prefabWall;
    public float spawnDelay;
    public int MaxamountOfRooms;

    public SC_Rooms currRoomToCheck;
    public int attachment;
    [HideInInspector]
    public GameObject customDungeonParent;
    [HideInInspector]
    public int currentAmountOfRooms;
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
        mainRoom.SetActive(false);
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

    public IEnumerator CheckRoomAndSpawn(SC_Rooms roomToCheck)
    {
        //Debug.Log(currentAmountOfRooms);
        if (currentAmountOfRooms < MaxamountOfRooms)
        {
            while (!CheckIfFullyAttached(roomToCheck))
            {
                List<SpawnablePosAndRot> newPosAndRot = new List<SpawnablePosAndRot>();

                //Debug.Log(roomToCheck.name);
                for (int k = 0; k < sRoomRots.presetRots.Length; k++)
                {
                    for (int i = 0; i < roomToCheck.attachPoints.Length; i++)
                    {
                        AttachPoints currAttachpoint = roomToCheck.attachPoints[i];

                        if (!currAttachpoint.attached)
                        {
                            GameObject pooledRoomObject = SC_RoomPooler.single.SpawnFromPool(currAttachpoint.nextSpawn, Vector3.zero, Quaternion.identity);
                            SC_Rooms pooledRoom = pooledRoomObject.GetComponent<SC_Rooms>();
                            AttachPoints[] aPpooledRoom = pooledRoom.attachPoints;
                            //Debug.Log(roomToCheck.name + " " + roomToCheck.attachPoints[i] + " " + i + "Is Not Attached");

                            for (int j = 0; j < aPpooledRoom.Length; j++)
                            {

                                Transform nrAttachPoint = aPpooledRoom[j].point;
                                float multi = 1f;
                                if(nrAttachPoint.localPosition.x != 0 && nrAttachPoint.localPosition.z != 0)
                                {
                                    multi = -1f;
                                }
                                Vector3 newOffset = new Vector3(nrAttachPoint.localPosition.x*multi, nrAttachPoint.localPosition.y, nrAttachPoint.localPosition.z*multi);
                                pooledRoomObject.transform.position = currAttachpoint.point.position + newOffset;
                                pooledRoomObject.transform.rotation = Quaternion.Euler(sRoomRots.presetRots[k].rot);
                                
                                bool overlap = CheckCollision(pooledRoomObject);
                                if (overlap)
                                {
                                    SpawnablePosAndRot newRoomPosAndRot = new SpawnablePosAndRot();
                                    newRoomPosAndRot.pos = pooledRoom.transform.position;
                                    newRoomPosAndRot.rot = pooledRoom.transform.rotation;
                                    newRoomPosAndRot.room = pooledRoomObject;
                                    newPosAndRot.Add(newRoomPosAndRot);
                                }
                                else
                                {
                                    Debug.Log("Overlapping With Room");
                                }
                                pooledRoomObject.SetActive(false);
                                yield return new WaitForSeconds(spawnDelay);
                            }
                            Debug.Log(pooledRoom.name + " Has :" + pooledRoom.spawnablePosAndRots.Count + "SpawnablePosses");
                        }
                    }
                }

                if (newPosAndRot.Count > 0)
                {
                    int randomPosAndRot = Random.Range(0, newPosAndRot.Count - 1);
                    GameObject finalRoom = newPosAndRot[randomPosAndRot].room;
                    finalRoom.transform.position = newPosAndRot[randomPosAndRot].pos;
                    finalRoom.transform.rotation = newPosAndRot[randomPosAndRot].rot;
                    SpawnRoom(finalRoom);
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
            foreach (SC_Rooms test in allspawnedRooms)
            {
                foreach (AttachPoints points in test.attachPoints)
                {
                    if (!points.attached)
                    {
                        points.wall.SetActive(true);
                        SetAttachment(test);
                    }
                }
            }
        }

    }

    private bool CheckCollision(GameObject pooledRoomObject)
    {
        SC_Rooms pooledRoom = pooledRoomObject.GetComponent<SC_Rooms>();

        Collider[] col = Physics.OverlapBox(pooledRoomObject.transform.position + pooledRoom.roomCollider.center, pooledRoom.roomCollider.bounds.extents * 0.5f, pooledRoomObject.transform.rotation, overlapLayer);
        bool isOverlapingWithOthers = CheckOverlapSelf(col, pooledRoom);
        
        return isOverlapingWithOthers;
    }

    bool CheckOverlapSelf(Collider[] colliding, SC_Rooms selfParent)
    {
        bool isOverlapingWithOthers = false;
        for (int i = 0; i < colliding.Length; i++)
        {
            if (colliding[i].GetComponentInParent<SC_Rooms>() != selfParent)
            {
                //Debug.Log(selfParent.name + " Is CollidingWith  Self");
                isOverlapingWithOthers = true;
            }
        }
        return isOverlapingWithOthers;
    }

    public void CheckAttachMent(SC_Rooms ownerRoom)
    {
        AttachPoints[] attachments = ownerRoom.attachPoints;

        for (int i = 0; i < attachments.Length; i++)
        {
            AttachPoints attachPoint = attachments[i];
            Collider[] col = Physics.OverlapBox(attachPoint.point.position + attachPoint.attachCollider.center, attachPoint.attachCollider.bounds.extents * 0.5f, attachPoint.point.rotation, attachPointsLayer);
            bool isOverlapingWithOthers = CheckOverlapSelf(col, ownerRoom);

            //Debug.Log(ownerRoom.name + " Has " + col.Length + " Collisions In Total ");
            //Debug.Log(ownerRoom.name + " Has " + overlapsWithSelf + " Collisions With Self");

            for (int k = 0; k < col.Length; k++)
            {
                if (isOverlapingWithOthers)
                {
                    attachPoint.canBeAttached = true;
                }
            }
        }
    }

    public void SpawnRoom(GameObject room)
    {
        //Debug.Log("SpawningRoom");
        SC_Rooms pooledRoom = room.GetComponent<SC_Rooms>();

        foreach (AttachPoints roomPoints in pooledRoom.attachPoints)
        {
            roomPoints.attached = false;
            roomPoints.attachedTo = null;
        }

        GameObject newRoom = Instantiate(room);
        newRoom.SetActive(true);
        SC_Rooms newRoomScript = newRoom.GetComponent<SC_Rooms>();

        newRoomScript.isChecker = false;
        newRoom.transform.SetParent(customDungeonParent.transform);
        allspawnedRooms.Add(newRoomScript);

        if (newRoomScript.roomType == AvailableSlots.SmallRooms)
        {
            currentAmountOfRooms++;
        }

        pooledRoom.spawnablePosAndRots.Clear();
        SetAttachment(newRoomScript);
    }

    public void SetAttachment(SC_Rooms ownerRoom)
    {
        AttachPoints[] attachments = ownerRoom.attachPoints;
        for (int i = 0; i < attachments.Length; i++)
        {
            AttachPoints attachPoint = attachments[i];
            Collider[] col = Physics.OverlapBox(attachPoint.point.position + attachPoint.attachCollider.center, attachPoint.attachCollider.bounds.extents * 0.5f, attachPoint.point.rotation, attachPointsLayer);

            //Debug.Log(ownerRoom.name + " Has " + col.Length + " Collisions In Total ");
            //Debug.Log(ownerRoom.name + " Has " + overlapsWithSelf + " Collisions With Self");

            for (int k = 0; k < col.Length; k++)
            {
                if (col[k].GetComponentInParent<SC_Rooms>() != ownerRoom)
                {
                    SC_Rooms otherRoom = col[k].GetComponentInParent<SC_Rooms>();
                    for (int j = 0; j < otherRoom.attachPoints.Length; j++)
                    {
                        if (col[k] == otherRoom.attachPoints[j].attachCollider && !attachPoint.attached)
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

    bool CheckIfFullyAttached(SC_Rooms roomToCheck)
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
