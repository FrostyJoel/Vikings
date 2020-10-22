using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_MinimapArrow : MonoBehaviour
{
    public float turnSpeed;
    public float arrowDistance;
    public GameObject actualArrow;
    public SC_EnemyStats nearestEnemy;
    // Update is called once per frame
    void Update()
    {
        if (SC_GameManager.single.gameStart && SC_GameManager.single.enemies.Count > 0)
        {
            RotateToNearestEnemy();
        }
    }

    private void RotateToNearestEnemy()
    {
        if(nearestEnemy == null)
        {
            nearestEnemy = GetNearestEnemy();
        }
        
        Vector3 nearestEnemyPos = nearestEnemy.transform.position;
        Vector3 playerPos = SC_TopDownController.single.transform.position;

        float dis = Vector3.Distance(nearestEnemyPos, playerPos);
        if(dis >= arrowDistance)
        {
            if (!actualArrow.activeSelf)
            {
                actualArrow.SetActive(true);
            }
            Vector3 dir = nearestEnemyPos - playerPos;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
            transform.rotation = Quaternion.Euler(90f,rotation.y,0f);
        }
        else
        {
            if (actualArrow.activeSelf)
            {
                actualArrow.SetActive(false);
            }
        }
    }

    private SC_EnemyStats GetNearestEnemy()
    {
        SC_EnemyStats bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (SC_EnemyStats potentialTarget in SC_GameManager.single.enemies)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }
}
