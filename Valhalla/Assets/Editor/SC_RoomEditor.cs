using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

#if UNITY_EDITOR
[CustomEditor(typeof(SC_Room))]
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

        if (GUILayout.Button("Set Obstacles"))
        {
            if (target_Room.propsGeo != null)
            {
                Collider[] iaArray = target_Room.propsGeo.GetComponentsInChildren<Collider>();
                foreach (Collider room in iaArray)
                {

                    room.gameObject.AddComponent(typeof(NavMeshObstacle));
                    room.GetComponent<NavMeshObstacle>().carving = true;

                }
                Debug.LogWarning($"Successfully set {iaArray.Length} Obstacles, don't forget to save!");
            }
            else
            {
                Debug.LogWarning("PropsGeo Not Assigned");
            }
        }
    }
}
#endif

