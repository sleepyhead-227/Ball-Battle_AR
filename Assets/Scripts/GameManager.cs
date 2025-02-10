using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class GameManager : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject objectContainer;
    public AnimatedUIManager animatedUIManager;
    public BallState currentBallState = BallState.Free;
    public bool playerIsAttacking = false;
    public bool gameIsStart = false;
    public bool ARScene = false;
    private int gameRound = 0;

    // Energy variable
    public int playerEnergy = 0;
    public int enemyEnergy = 0;
    private int playerEnergyCost = 0;
    private int enemyEnergyCost = 0;
    const int attackerCost = 2;
    const int defenderCost = 3;

    // Time variable
    private float countPlayerTimer = 0f;
    private float countEnemyTimer = 0f;
    private float timeRemaining = 140f;

    // Player variables
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public List<GameObject> allPlayerList = new List<GameObject>();
    public int playerPoint = 0;
    public int enemyPoint = 0;

    // Ball variables
    public GameObject ballPrefab;
    public GameObject spawnedBall;

    // Field variable
    public GameObject Goal_A;
    public GameObject Goal_B;
    public List<GameObject> fenceList = new List<GameObject>();
    public Vector3 playerFieldMin;
    public Vector3 playerFieldMax;
    public Vector3 enemyFieldMin;
    public Vector3 enemyFieldMax;
    public float fieldScalePercentage = 1.0f;

    // UI variable
    public TextMeshProUGUI enemyName;
    public TextMeshProUGUI enemyPointText;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerPointText;
    public TextMeshProUGUI timeText;
    public List<Image> enemyEnergyBar = new List<Image>();
    public List<Image> playerEnergyBar = new List<Image>();
    public GameObject StartGameContainer;
    public GameObject blankScreen;
    public GameObject winPopup;
    public TextMeshProUGUI winScore;
    public GameObject losePopup;
    public TextMeshProUGUI loseScore;

    public AudioManager audioManager;
    public GameObject warningPopup;

    private void Start()
    {
        audioManager.PlayBGM("PlayGame");
    }

    public void StartGame()
    {
        if (ARScene)
        {
            if (objectContainer != null)
            {
                if (!gameIsStart && Goal_A != null && Goal_B != null)
                {
                    audioManager.PlaySFX("click");
                    StartCoroutine(resetGameWithDelay(0.5f));
                    StartGameContainer.SetActive(false);
                }
            }
            else
            {
                showWarning(true);
            }
        }
        else
        {
            if (!gameIsStart && Goal_A != null && Goal_B != null)
            {
                audioManager.PlaySFX("click");
                StartCoroutine(resetGameWithDelay(0.5f));
                StartGameContainer.SetActive(false);
            }
        }
    }

    public void StartTheGame()
    {
        audioManager.PlaySFX("whistle");
        spawnBall();
        if (playerIsAttacking)
        {
            playerEnergyCost = attackerCost;
            enemyEnergyCost = defenderCost;
            playerName.text = "Player [as Attacker]";
            enemyName.text = "Enemy [as Defender]";
        }
        else
        {
            playerEnergyCost = defenderCost;
            enemyEnergyCost = attackerCost;
            playerName.text = "Player [as Defender]";
            enemyName.text = "Enemy [as Attacker]";
        }

        gameIsStart = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBallState == BallState.Goal || !gameIsStart) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.name == "Field B")
                {
                    Vector3 spawnPosition = hit.point;
                    Vector3 localPosition = objectContainer.transform.InverseTransformPoint(spawnPosition);
                    localPosition.y = 0f;
                    if (playerEnergy >= playerEnergyCost)  spawnPlayer(localPosition);
                }
            }
        }

        if (enemyEnergy >= enemyEnergyCost)
        {
            spawnEnemy();
        }

        PlayerEnergyRegeneration();
        EnemyEnergyRegeneration();
        GameTimeCountdown();
    }

    bool HasCameraPermission()
    {
#if UNITY_ANDROID
        return Permission.HasUserAuthorizedPermission(Permission.Camera);
#else
        return true;
#endif
    }

    public void RequestCameraPermission()
    {
        audioManager.PlaySFX("click");
#if UNITY_ANDROID
        if (!HasCameraPermission())
        {
            Permission.RequestUserPermission(Permission.Camera);
            InvokeRepeating("CheckCameraPermissionStatus", 0f, 0.5f);
        }
        else
        {
            SceneManager.LoadScene("AR");
        }
#endif
    }

    void CheckCameraPermissionStatus()
    {
#if UNITY_ANDROID
        // If permission is granted, handle it
        if (Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            HandleCameraPermissionGranted();
            CancelInvoke("CheckCameraPermissionStatus");  // Stop checking
        }
        // If permission is denied
        else
        {
            // Do nothing if user deny
            CancelInvoke("CheckCameraPermissionStatus");  // Stop checking
        }
#endif
    }

    void HandleCameraPermissionGranted()
    {
        // Trigger the event or handler
        Debug.Log("Camera permission granted.");
        SceneManager.LoadScene("AR");
    }

    void GameTimeCountdown()
    {
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            gameIsStart = false;
            animatedUIManager.TimeoutAnimation();
            Debug.Log("Timer Finished!");
            audioManager.PlaySFX("whistle");
            if (gameRound < 5)
            {
                StartCoroutine(resetGameWithDelay(3f));
            }
            else
            {
                finishTheGame();
            }
        }

        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void PlayerEnergyRegeneration()
    {
        if (playerEnergy < 6)
        {
            countPlayerTimer += Time.deltaTime;
            SetImageAlpha(playerEnergyBar[playerEnergy], 0.5f);
            playerEnergyBar[playerEnergy].fillAmount = countPlayerTimer / 2;
            if (countPlayerTimer >= 2f)
            {
                SetImageAlpha(playerEnergyBar[playerEnergy], 1.0f);
                countPlayerTimer = 0f;
                playerEnergy++;
            }
        }
    }

    void EnemyEnergyRegeneration()
    {
        if (enemyEnergy < 6)
        {
            countEnemyTimer += Time.deltaTime;
            SetImageAlpha(enemyEnergyBar[enemyEnergy], 0.5f);
            enemyEnergyBar[enemyEnergy].fillAmount = countEnemyTimer / 2;
            if (countEnemyTimer >= 2f)
            {
                SetImageAlpha(enemyEnergyBar[enemyEnergy], 1.0f);
                countEnemyTimer = 0f;
                enemyEnergy++;
            }
        }
    }

    void SetImageAlpha(Image targetImage, float alpha)
    {
        Color currentColor = targetImage.color;
        currentColor.a = Mathf.Clamp(alpha, 0f, 1f);
        targetImage.color = currentColor;
    }

    public void BallIsGoal()
    {
        audioManager.PlaySFX("whistle");
        gameIsStart = false;
        currentBallState = BallState.Goal;
        animatedUIManager.GoalAnimation();
        if (playerIsAttacking)
        {
            playerPoint++;
            playerPointText.text = playerPoint.ToString();
        }
        else
        {
            enemyPoint++;
            enemyPointText.text = enemyPoint.ToString();
        }

        if (gameRound < 5)
        {
            StartCoroutine(resetGameWithDelay(3f));
        }
        else
        {
            finishTheGame();
        }
    }

    void finishTheGame()
    {
        blankScreen.SetActive(true);
        if (playerPoint > enemyPoint)
        {
            // Player is win
            winPopup.SetActive(true);
            winScore.text = playerPoint + " - " + enemyPoint;
            audioManager.PlaySFX("applause");
        }
        else if (playerPoint < enemyPoint)
        {
            // Enemy is win
            losePopup.SetActive(true);
            loseScore.text = playerPoint + " - " + enemyPoint;
            audioManager.PlaySFX("boo");
        }
        else
        {
            // Draw
            animatedUIManager.DrawAnimation();
            audioManager.PlaySFX("boo");
        }
    }

    public void GotoMainmenu()
    {
        audioManager.PlaySFX("click");
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator resetGameWithDelay(float delay)
    {
        // Put delay so user/player can prepare their self
        yield return new WaitForSeconds(delay);

        if (playerIsAttacking)
        {
            playerIsAttacking = false;
        }
        else
        {
            playerIsAttacking = true;
        }

        foreach (var player in allPlayerList)
        {
            if (player != null) Destroy(player);
        }
        allPlayerList.Clear();
        if (spawnedBall != null) Destroy(spawnedBall);

        // Reset Energy Bar
        foreach (Image energy in enemyEnergyBar)
        {
            Debug.Log("energy bar : "+energy);
            energy.fillAmount = 0;
        }
        foreach (Image energy in playerEnergyBar)
        {
            energy.fillAmount = 0;
        }
        timeRemaining = 140f;
        timeText.text = string.Format("{0:00}:{1:00}", 2, 20);

        timeRemaining = 140f;
        playerEnergy = 0;
        enemyEnergy = 0;
        countPlayerTimer = 0f;
        countEnemyTimer = 0f;

        gameRound++;
        animatedUIManager.StartAnimationSequance(gameRound);
    }

    void spawnBall()
    {
        float randomX;
        float randomY;
        float randomZ;
        if (playerIsAttacking)
        {
            randomX = UnityEngine.Random.Range(playerFieldMin.x, playerFieldMax.x);
            randomY = UnityEngine.Random.Range(playerFieldMin.y, playerFieldMax.y);
            randomZ = UnityEngine.Random.Range(playerFieldMin.z, playerFieldMax.z);
        }
        else
        {
            randomX = UnityEngine.Random.Range(enemyFieldMin.x, enemyFieldMax.x);
            randomY = UnityEngine.Random.Range(enemyFieldMin.y, enemyFieldMax.y);
            randomZ = UnityEngine.Random.Range(enemyFieldMin.z, enemyFieldMax.z);
        }
        

        Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);

        spawnedBall = Instantiate(ballPrefab);
        spawnedBall.GetComponent<BallManager>().gameManager = this;
        spawnedBall.transform.SetParent(objectContainer.transform);
        currentBallState = BallState.Free;

        // Adjusting transform
        spawnedBall.transform.localPosition = randomPosition;
        spawnedBall.transform.localRotation = Quaternion.identity;
        Vector3 defaultScale = spawnedBall.transform.localScale;
        spawnedBall.transform.localScale = new Vector3(
            objectContainer.transform.localScale.x * defaultScale.x,
            objectContainer.transform.localScale.y * defaultScale.y,
            objectContainer.transform.localScale.z * defaultScale.z
        );
    }

    void spawnPlayer(Vector3 spawnPosition)
    {
        audioManager.PlaySFX("click");
        GameObject spawnedPlayer = Instantiate(playerPrefab);
        spawnedPlayer.GetComponent<PlayerManager>().gameManager = this;
        spawnedPlayer.GetComponent<PlayerManager>().SetPlayerStatus(playerIsAttacking);
        spawnedPlayer.transform.SetParent(objectContainer.transform);

        // Adjusting transform
        spawnedPlayer.transform.localPosition = spawnPosition;
        spawnedPlayer.transform.localRotation = Quaternion.identity;
        Vector3 defaultScale = spawnedPlayer.transform.localScale;
        spawnedPlayer.transform.localScale = new Vector3(
            objectContainer.transform.localScale.x * defaultScale.x,
            objectContainer.transform.localScale.y * defaultScale.y,
            objectContainer.transform.localScale.z * defaultScale.z
        );

        allPlayerList.Add(spawnedPlayer);
        for (int i = 0; i <= playerEnergyCost; i++)
        {
            int resetIndex = playerEnergy - i;
            if (resetIndex < 6) playerEnergyBar[resetIndex].fillAmount = 0;
        }
        playerEnergy -= playerEnergyCost;
    }

    void spawnEnemy()
    {
        float randomX = UnityEngine.Random.Range(enemyFieldMin.x, enemyFieldMax.x);
        float randomY = UnityEngine.Random.Range(enemyFieldMin.y, enemyFieldMax.y);
        float randomZ = UnityEngine.Random.Range(enemyFieldMin.z, enemyFieldMax.z);
        Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);

        GameObject spawnedEnemy = Instantiate(enemyPrefab);
        spawnedEnemy.GetComponent<PlayerManager>().gameManager = this;
        spawnedEnemy.GetComponent<PlayerManager>().SetPlayerStatus(!playerIsAttacking);
        spawnedEnemy.transform.SetParent(objectContainer.transform);

        // Adjusting transform
        spawnedEnemy.transform.localPosition = randomPosition;
        spawnedEnemy.transform.localRotation = Quaternion.Euler(0, 180, 0);
        Vector3 defaultScale = spawnedEnemy.transform.localScale;
        spawnedEnemy.transform.localScale = new Vector3(
            objectContainer.transform.localScale.x * defaultScale.x,
            objectContainer.transform.localScale.y * defaultScale.y,
            objectContainer.transform.localScale.z * defaultScale.z
        );

        allPlayerList.Add(spawnedEnemy);
        for (int i = 0; i <= enemyEnergyCost; i++)
        {
            int resetIndex = enemyEnergy - i;
            if (resetIndex < 6) enemyEnergyBar[resetIndex].fillAmount = 0;
        }
        enemyEnergy -= enemyEnergyCost;
    }

    public GameObject GetPlayerWhoCarryingBall()
    {
        GameObject carryingPlayer = null;
        foreach (var player in allPlayerList)
        {
            if (player.GetComponent<PlayerManager>().playerState == PlayerState.Carrying)
            {
                carryingPlayer = player;
            }
        }

        return carryingPlayer;
    }

    public void PasstoNearestAttackerTeammate(GameObject currentPlayer)
    {
        GameObject nearestPlayer = null;
        float nearestDistance = Mathf.Infinity;
        foreach (var player in allPlayerList)
        {
            if (player.GetComponent<PlayerManager>().isAttacking && player.GetComponent<PlayerManager>().playerState == PlayerState.Active)
            {
                float distance = Vector3.Distance(player.transform.position, currentPlayer.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPlayer = player;
                }
            }
        }

        if (nearestPlayer != null)
        {
            currentBallState = BallState.Passing;
            spawnedBall.GetComponent<BallManager>().targetPassing = nearestPlayer;
        }
        else
        {
            // defender WIN
            gameIsStart = false;
            animatedUIManager.DefenderWinAnimation();
            if (!playerIsAttacking)
            {
                playerPoint++;
                playerPointText.text = playerPoint.ToString();
                audioManager.PlaySFX("applause");
            }
            else
            {
                enemyPoint++;
                enemyPointText.text = enemyPoint.ToString();
                audioManager.PlaySFX("boo");
            }

            if (gameRound < 5)
            {
                StartCoroutine(resetGameWithDelay(3f));
            }
            else
            {
                finishTheGame();
            }
        }
    }

    public void showWarning(bool show)
    {
        warningPopup.SetActive(show);
    }

    public void destroyPlayer(GameObject player)
    {
        allPlayerList.Remove(player);
        Destroy(player);
    }
}

public enum BallState
{
    Free,
    Carrying,
    Passing,
    Goal
}

public enum PlayerState
{
    Active,
    Inactive,
    Carrying,
    Standby,
    ChasingEnemy,
    Return
}
