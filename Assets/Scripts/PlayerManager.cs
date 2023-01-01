using System.Collections;
using Cinemachine;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private int numberOfStickmans, numberOfEnemyStickmans;
    [SerializeField] private TextMeshPro CounterTxt;
    [SerializeField] private GameObject stickMan;
    //****************************************************

    [Range(0f, 1f)][SerializeField] private float DistanceFactor, Radius;

    //*********** move the player ********************

    public bool moveByTouch, gameState;
    private Vector3 mouseStartPos, playerStartPos;
    public float playerSpeed, roadSpeed;
    private Camera camera;

    [SerializeField] private Transform road;
    [SerializeField] private Transform enemy;
    private bool attack;
    public static PlayerManager PlayerManagerInstance;
    public ParticleSystem blood;
    public GameObject SecondCam;
    public bool FinishLine, moveTheCamera;

    public bool isForwardMove;
    public Transform stickmanParent;

    void Start()
    {
        numberOfStickmans = stickmanParent.childCount - 1;

        CounterTxt.text = numberOfStickmans.ToString();

        camera = Camera.main;

        PlayerManagerInstance = this;

        gameState = false;
        isForwardMove = false;
    }

    void Update()
    {
        if (attack)
        {
            var enemyDirection = new Vector3(enemy.position.x, transform.position.y, enemy.position.z) - transform.position;

            for (int i = 1; i < stickmanParent.childCount; i++)
            {
                stickmanParent.GetChild(i).rotation =
                    Quaternion.Slerp(stickmanParent.GetChild(i).rotation, Quaternion.LookRotation(enemyDirection, Vector3.up), Time.deltaTime * 3f);
            }

            if (enemy.GetChild(1).childCount > 1)
            {
                for (int i = 0; i < stickmanParent.childCount; i++)
                {
                    var Distance = enemy.GetChild(1).GetChild(0).position - stickmanParent.GetChild(i).position;

                    if (Distance.magnitude < 1.5f)
                    {
                        stickmanParent.GetChild(i).position = Vector3.Lerp(stickmanParent.GetChild(i).position,
                            new Vector3(enemy.GetChild(1).GetChild(0).position.x, stickmanParent.GetChild(i).position.y,
                                enemy.GetChild(1).GetChild(0).position.z), Time.deltaTime * 1f);
                    }
                }
            }

            else
            {
                attack = false;
                roadSpeed = 2f;

                FormatStickMan(0.45f);

                for (int i = 1; i < stickmanParent.childCount; i++)
                    stickmanParent.GetChild(i).rotation = Quaternion.identity;


                enemy.gameObject.SetActive(false);

            }

            if (stickmanParent.childCount == 1)
            {
                enemy.transform.GetChild(1).GetComponent<enemyManager>().StopAttacking();
                gameObject.SetActive(false);
                menuManager.Instance.ChangePanel(menuManager.Instance.levelFailedPanel);

            }
        }
        else
        {
            MoveThePlayer();

        }


        if (stickmanParent.childCount == 1 && FinishLine)
        {
            gameState = false;
        }


        if (isForwardMove)
        {
            road.Translate(road.forward * Time.deltaTime * roadSpeed);
        }

        if (moveTheCamera && stickmanParent.childCount > 1)
        {
            var cinemachineTransposer = SecondCam.GetComponent<CinemachineVirtualCamera>()
              .GetCinemachineComponent<CinemachineTransposer>();

            var cinemachineComposer = SecondCam.GetComponent<CinemachineVirtualCamera>()
                .GetCinemachineComponent<CinemachineComposer>();

            var cinemachineCamera = SecondCam.GetComponent<CinemachineVirtualCamera>();

            if (stickmanParent.childCount > 1)
            {
                cinemachineCamera.Follow = stickmanParent.GetChild(stickmanParent.childCount - 1).transform;
                cinemachineCamera.LookAt = stickmanParent.GetChild(stickmanParent.childCount - 1).transform;

                cinemachineTransposer.m_FollowOffset = new Vector3(4.5f, Mathf.Lerp(cinemachineTransposer.m_FollowOffset.y,
                transform.GetChild(1).position.y + 2f, Time.deltaTime * 1f), -5f);

                cinemachineComposer.m_TrackedObjectOffset = new Vector3(0f, Mathf.Lerp(cinemachineComposer.m_TrackedObjectOffset.y,
                    1f, Time.deltaTime * 1f), 0f);
            }
        }
    }

    void MoveThePlayer()
    {
        if (Input.GetMouseButtonDown(0) && gameState)
        {
            moveByTouch = true;

            var plane = new Plane(Vector3.up, 0f);

            var ray = camera.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out var distance))
            {
                mouseStartPos = ray.GetPoint(distance + 1f);
                playerStartPos = transform.position;
            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            moveByTouch = false;

        }

        if (moveByTouch)
        {
            var plane = new Plane(Vector3.up, 0f);
            var ray = camera.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out var distance))
            {
                var mousePos = ray.GetPoint(distance + 1f);

                var move = mousePos - mouseStartPos;

                var control = playerStartPos + move;


                if (numberOfStickmans > 50)
                    control.x = Mathf.Clamp(control.x, -0.7f, 0.7f);
                else
                    control.x = Mathf.Clamp(control.x, -1.1f, 1.1f);

                transform.position = new Vector3(Mathf.Lerp(transform.position.x, control.x, Time.deltaTime * playerSpeed)
                    , transform.position.y, transform.position.z);

            }
        }
    }

    public void FormatStickMan(float coordinatY)
    {
        for (int i = 1; i < stickmanParent.childCount; i++)
        {
            var x = DistanceFactor * Mathf.Sqrt(i) * Mathf.Cos(i * Radius);
            var z = DistanceFactor * Mathf.Sqrt(i) * Mathf.Sin(i * Radius);

            var NewPos = new Vector3(x, coordinatY, z);

            stickmanParent.GetChild(i).DOLocalMove(NewPos, 0.5f).SetEase(Ease.OutBack);
        }
    }

    public void MakeStickMan(int number)
    {
        for (int i = numberOfStickmans; i < number; i++)
        {
            Instantiate(stickMan, transform.position, quaternion.identity, stickmanParent);
        }

        numberOfStickmans = transform.childCount - 1;
        CounterTxt.text = numberOfStickmans.ToString();
        FormatStickMan(0.45f);
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("gate"))
        {
            other.transform.parent.GetChild(0).GetComponent<BoxCollider>().enabled = false; // gate 1
            other.transform.parent.GetChild(1).GetComponent<BoxCollider>().enabled = false; // gate 2

            var gateManager = other.GetComponent<GateManager>();

            numberOfStickmans = stickmanParent.childCount
                - 1;

            if (gateManager.multiply)
            {
                MakeStickMan(numberOfStickmans * gateManager.randomNumber);
            }
            else
            {
                MakeStickMan(numberOfStickmans + gateManager.randomNumber);

            }
        }

        if (other.CompareTag("enemy"))
        {
            enemy = other.transform;
            attack = true;

            roadSpeed = 0.5f;

            if (stickmanParent.childCount > 1)
            {
                other.transform.GetChild(1).GetComponent<enemyManager>().AttackThem(transform);

                StartCoroutine(UpdateTheEnemyAndPlayerStickMansNumbers());
            }
        }

        if (other.CompareTag("Finish"))
        {
            moveByTouch = false;
            gameState = false;
            SecondCam.SetActive(true);
            FinishLine = true;
            moveTheCamera = true;
            Tower.TowerInstance.CreateTower(stickmanParent.childCount - 1);
            //stickmanParent.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(false);
        }
        if (other.CompareTag("FormatPoint"))
        {
            FormatStickMan(18);
        }
        if (other.CompareTag("FinishLine"))
        {
            Debug.LogError("LEVEL COMPLETED!");
        }
    }

    IEnumerator UpdateTheEnemyAndPlayerStickMansNumbers()
    {

        numberOfEnemyStickmans = enemy.transform.GetChild(1).childCount - 1;
        numberOfStickmans = stickmanParent.childCount - 1;

        while (numberOfEnemyStickmans > 0 && numberOfStickmans > 0)
        {
            numberOfEnemyStickmans--;
            numberOfStickmans--;

            enemy.transform.GetChild(1).GetComponent<enemyManager>().CounterTxt.text = numberOfEnemyStickmans.ToString();
            CounterTxt.text = numberOfStickmans.ToString();

            yield return null;
        }

        if (numberOfEnemyStickmans == 0)
        {
            for (int i = 0; i < stickmanParent.childCount; i++)
            {
                stickmanParent.GetChild(i).rotation = Quaternion.identity;
            }
        }
    }
}
