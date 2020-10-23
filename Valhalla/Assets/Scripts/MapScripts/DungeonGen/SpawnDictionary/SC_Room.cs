using System.Collections.Generic;
using UnityEngine;

public class SC_Room : MonoBehaviour
{
    public AttachPoint[] attachPoints;
    [Space]
    public int maxAmountOfEnemies;
    public Transform[] spawnPosEnemies;
    public BoxCollider roomCollider;
    public AvailableSlots roomType;
    public Transform propsGeo;

    [HideInInspector]
    public List<SC_EnemyStats> enemiesInRoom = new List<SC_EnemyStats>();
    [HideInInspector]
    public bool hasEnemies;
    [HideInInspector]
    public List<SpawnablePosAndRot> spawnablePosAndRots = new List<SpawnablePosAndRot>();
    [HideInInspector]
    public bool fullyAttached;
    [HideInInspector]
    public bool isChecker;
    [HideInInspector]
    public bool isChecked;
    [HideInInspector]
    public MeshRenderer[] meshRenderers;
    //private void OnDrawGizmosSelected()
    //{
    //    if (attachPoints.Length > 0)
    //    {
    //        foreach (AttachPoint attachPoint in attachPoints)
    //        {
    //            Gizmos.color = Color.red;
    //            Gizmos.DrawWireCube(attachPoint.attachCollider.transform.position + attachPoint.attachCollider.center, attachPoint.attachCollider.size * 2f);
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("No Attachpoints Assigned to " + gameObject.name);
    //    }

    //    if (roomCollider)
    //    {
    //        Gizmos.matrix = transform.worldToLocalMatrix;
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireCube(roomCollider.transform.position + roomCollider.center, roomCollider.size);
    //    }
    //    else
    //    {
    //        Debug.LogError("No RoomCollider Assigned to " + gameObject.name);
    //    }
    //}

    public void MakeEverythingStatic()
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].gameObject.isStatic = true;
        }
    }

    private void Update()
    {
        if(hasEnemies && enemiesInRoom.Count <= 0)
        {
            foreach (AttachPoint wallPoint in attachPoints)
            {
                if (wallPoint.wall.activeSelf == true && !wallPoint.mapWall)
                {
                    wallPoint.wall.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Player") && hasEnemies)
        {
            foreach (SC_EnemyStats enemyStats in enemiesInRoom)
            {
                enemyStats.roomClosed = true;
            }

            foreach(AttachPoint wallPoint in attachPoints)
            {
                wallPoint.wall.SetActive(true);
            }
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
    [HideInInspector]
    public GameObject wall;
    public bool mapWall;
    [HideInInspector]
    public BoxCollider AttachCollider
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
    [HideInInspector]
    public AvailableSlots nextSpawn;
    [HideInInspector]
    public bool attached;
    [HideInInspector]
    public bool canBeAttached;
    [HideInInspector]
    public SC_Room attachedTo;
    [HideInInspector]
    public Vector3 off;
}
