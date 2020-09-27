using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_MinimapCam : MonoBehaviour
{
    public SC_TopDownController player;
    
    public void FollowPlayer()
    {
        Vector3 minimapOffset = new Vector3 (player.transform.position.x,transform.position.y, player.transform.position.z);
        transform.position = minimapOffset;
    }
}
