using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
    public MeshRenderer[] meshRenderers;

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
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(roomCollider.transform.position + roomCollider.center, roomCollider.size);
        }
        else
        {
            Debug.LogError("No RoomCollider Assigned to " + gameObject.name);
        }
    }
    public void MakeEverythingStatic()
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].gameObject.isStatic = true;
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

#if UNITY_EDITOR
[CustomEditor(typeof(SC_Room)), CanEditMultipleObjects]
public class SC_RoomEditor : Editor
{
    SC_Room target_Room;

    public void OnEnable()
    {
        target_Room = (SC_Room)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Set MeshRenderers"))
        {
            MeshRenderer[] iaArray = target_Room.GetComponentsInChildren<MeshRenderer>();
            List<MeshRenderer> iaList = new List<MeshRenderer>();
            int i;
            for (i = 0; i < iaArray.Length; i++)
            {
                MeshRenderer interactable = iaArray[i];
                iaList.Add(interactable);
                EditorUtility.SetDirty(interactable);
            }
            target_Room.meshRenderers = iaList.ToArray();
            Debug.LogWarning($"Successfully set {i} meshrenderers, don't forget to save!");
        }
    }
}
#endif
