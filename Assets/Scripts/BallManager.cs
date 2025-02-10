using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BallManager : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject targetPassing;
    public GameObject ballObject;

    private float passingSpeed = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager == null) return;

        switch (gameManager.currentBallState)
        {
            case BallState.Free:
                
                break;
            case BallState.Carrying:
                followPlayer();
                break;
            case BallState.Passing:
                passingtoOtherPlayer();
                break;
            case BallState.Goal:

                break;
        }
    }

    void followPlayer()
    {
        GameObject player = gameManager.GetPlayerWhoCarryingBall();

        if (player != null)
        {
            Vector3 playerPosition = player.transform.localPosition;
            playerPosition.z += 0.3f;
            transform.localPosition = playerPosition;
        }
    }

    void passingtoOtherPlayer()
    {
        if (targetPassing != null)
        {
            Vector3 direction = (targetPassing.transform.position - transform.position).normalized;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            transform.Translate(Vector3.forward * passingSpeed * Time.deltaTime);
        }
    }
}
