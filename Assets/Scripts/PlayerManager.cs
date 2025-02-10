using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerManager : MonoBehaviour
{
    public List<GameObject> playerBody = new List<GameObject>();
    public GameObject highlight;
    public GameObject arrow;
    public GameObject rangeArea;
    public Material defaultMaterial;
    public Material inactiveMaterial;
    public bool isAttacking = true;

    const float normalAttackerSpeed = 1.5f;
    const float normalDefenderSpeed = 1.5f;
    const float carryingSpeed = 0.75f;
    const float returnSpeed = 2.0f;
    const float reactiveAttacker = 2.5f;
    const float reactiveDefender = 4.0f;
    const float testSpeed = 0.3f;
    private float countTimer = 0f;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;

    public GameManager gameManager;
    public PlayerState playerState;

    public Animator animator;

    public TextMeshProUGUI inactiveCount;

    // Start is called before the first frame update
    void Start()
    {
        defaultPosition = gameObject.transform.position;
        defaultRotation = gameObject.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager == null && playerState == PlayerState.Inactive) return;
        if (gameManager.gameIsStart)
        {
            // For attacker
            switch (gameManager.currentBallState)
            {
                case BallState.Free:
                    chasingBall();
                    break;
                case BallState.Carrying:
                    carryingBall();
                    break;
                case BallState.Passing:
                    chasingBall();
                    break;
                case BallState.Goal:
                    // Stop moving after goal
                    break;
            }

            // For defender
            switch (playerState)
            {
                case PlayerState.Standby:
                    standbyDefending();
                    break;
                case PlayerState.ChasingEnemy:
                    chasingEnemy();
                    break;
                case PlayerState.Return:
                    returnBack();
                    break;
                case PlayerState.Inactive:
                    countTimer += Time.deltaTime;
                    inactiveCount.text = Mathf.Round(countTimer).ToString();
                    if (isAttacking && countTimer >= reactiveAttacker)
                    {
                        activatePlayer();
                    }
                    if (!isAttacking && countTimer >= reactiveDefender)
                    {
                        activatePlayer();
                    }
                    break;
            }
        }
    }

    public void SetPlayerStatus(bool attackingTeam)
    {
        isAttacking = attackingTeam;
        if (isAttacking)
        {
            playerState = PlayerState.Active;
            animator.Play("Run");
        }
        else
        {
            playerState = PlayerState.Standby;
            animator.Play("Idle");
            rangeArea.SetActive(true);
        }
    }

    void chasingBall()
    {
        if (playerState != PlayerState.Active) return;

        Vector3 direction = (gameManager.spawnedBall.transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        float adjustedSpeed = normalAttackerSpeed / gameManager.fieldScalePercentage;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        transform.Translate(Vector3.forward * adjustedSpeed * Time.deltaTime);
    }

    void carryingBall()
    {
        if (playerState == PlayerState.Active)
        {
            Quaternion targetRotation;
            if (gameManager.playerIsAttacking)
            {
                targetRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                targetRotation = Quaternion.Euler(0, 180, 0);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

            float parentScaleFactor = gameManager.fieldScalePercentage;
            float adjustedSpeed = normalAttackerSpeed / parentScaleFactor;
            transform.Translate(Vector3.forward * adjustedSpeed * Time.deltaTime);
        }
        else if (playerState == PlayerState.Carrying)
        {
            Vector3 goalPosition;
            if (gameManager.playerIsAttacking)
            {
                goalPosition = gameManager.Goal_A.transform.position;
            }
            else
            {
                goalPosition = gameManager.Goal_B.transform.position;
            }

            Vector3 direction = (goalPosition - transform.position).normalized;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

            float parentScaleFactor = gameManager.fieldScalePercentage;
            float adjustedSpeed = carryingSpeed / parentScaleFactor;

            transform.Translate(Vector3.forward * adjustedSpeed * Time.deltaTime);
        }
    }

    void standbyDefending()
    {
        GameObject carryingPlayer = gameManager.GetPlayerWhoCarryingBall();
        if (carryingPlayer != null)
        {
            float distance = Vector3.Distance(transform.localPosition, carryingPlayer.transform.position);
            if (distance < 1.3f)
            {
                playerState = PlayerState.ChasingEnemy;
            }
        }
    }

    void chasingEnemy()
    {
        GameObject carryingPlayer = gameManager.GetPlayerWhoCarryingBall();
        if (carryingPlayer != null)
        {
            Vector3 direction = (carryingPlayer.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

            float parentScaleFactor = gameManager.fieldScalePercentage;
            float adjustedSpeed = normalDefenderSpeed / parentScaleFactor;

            transform.Translate(Vector3.forward * adjustedSpeed * Time.deltaTime);
        }
    }

    void returnBack()
    {
        Vector3 direction = (defaultPosition - transform.localPosition).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

        float parentScaleFactor = gameManager.fieldScalePercentage;
        float adjustedSpeed = returnSpeed / parentScaleFactor;

        transform.Translate(Vector3.forward * adjustedSpeed * Time.deltaTime);

        float distance = Vector3.Distance(defaultPosition, transform.localPosition);
        if (distance < 0.2)
        {
            countTimer = 0;
            transform.position = defaultPosition;
            transform.rotation = defaultRotation;
            playerState = PlayerState.Inactive;
            animator.Play("Idle");
            inactiveCount.gameObject.SetActive(true);
        }
    }

    void activatePlayer()
    {
        if (isAttacking)
        {
            playerState = PlayerState.Active;
            animator.Play("Run");
        }
        else
        {
            playerState = PlayerState.Standby;
            animator.Play("Idle");
        }

        foreach (GameObject bodypart in playerBody)
        {
            Renderer renderer = bodypart.GetComponent<Renderer>();
            renderer.material = defaultMaterial;
        }
        inactiveCount.gameObject.SetActive(false);
    }

    void inactivatePlayer()
    {
        gameManager.audioManager.PlaySFX("fall");
        foreach (GameObject bodypart in playerBody)
        {
            Renderer renderer = bodypart.GetComponent<Renderer>();
            renderer.material = inactiveMaterial;
        }

        // Return to default position
        if (isAttacking)
        {
            countTimer = 0;
            highlight.SetActive(false);
            playerState = PlayerState.Inactive;
            animator.Play("Idle");
            gameManager.PasstoNearestAttackerTeammate(gameObject);
            inactiveCount.gameObject.SetActive(true);
        }
        else
        {
            playerState = PlayerState.Return;
            animator.Play("Run");
        }
    }

    void OnTriggerEnter(Collider objectCollide)
    {
        if (gameManager == null) return;

        Debug.Log(objectCollide.name);
        switch (objectCollide.name)
        {
            case "Ball(Clone)":
                if (playerState == PlayerState.Active)
                {
                    if (gameManager.currentBallState == BallState.Free || gameManager.currentBallState == BallState.Passing)
                    {
                        gameManager.audioManager.PlaySFX("kick");
                        gameManager.currentBallState = BallState.Carrying;
                        playerState = PlayerState.Carrying;
                        highlight.SetActive(true);
                    }
                }
                break;
            case "Fence":
                if (playerState == PlayerState.Active)
                {
                    gameManager.audioManager.PlaySFX("fall");
                    gameManager.destroyPlayer(gameObject);
                }
                break;
            case "Goal":
                if (playerState == PlayerState.Carrying)
                {
                    Debug.Log("Goal !!!");
                    gameManager.BallIsGoal();
                }
                else if (playerState == PlayerState.Active)
                {
                    gameManager.audioManager.PlaySFX("fall");
                    gameManager.destroyPlayer(gameObject);
                }
                break;
            case "Player A(Clone)":
                if (gameObject.name != objectCollide.name)
                {
                    if (playerState == PlayerState.Carrying || playerState == PlayerState.ChasingEnemy) inactivatePlayer();
                }
                break;
            case "Player B(Clone)":
                if (gameObject.name != objectCollide.name)
                {
                    if (playerState == PlayerState.Carrying || playerState == PlayerState.ChasingEnemy) inactivatePlayer();
                }
                break;
        }
    }
}
