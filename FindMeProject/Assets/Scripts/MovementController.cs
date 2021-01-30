using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MovementController : MonoBehaviourPunCallbacks
{
    [Header("Player Movement")]
    public bool controlEnable;
    private bool boosterSpeed;
    private float defaultSpeed;
    public float speed = 5;
    public float speedRot = 5;
    public Transform transPivotLight;
    private Vector3 velocity = Vector3.zero;
    private Rigidbody rb;

    [Header("Player Action")]
    public BoxCollider boxTrigger;
    public bool CanAction;
    private bool isActionWrong;
    public GameObject ImageRightAction;
    public GameObject ImageWrongAction;
    private GameObject ImageAction;
    private PhotonView pv;
    public GameObject TargetAttack;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        controlEnable = false;
        ImageAction = FindMeGameManager.instance.ImageAction;
    }

    private void Update()
    {
        if (!pv.IsMine)
        {
            return;
        }

        if (controlEnable)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                PhotonNetwork.LeaveRoom();
                UnityEngine.SceneManagement.SceneManager.LoadScene("LaunchScene");
            }

            velocity = MovingCharacter();

            RotateLight();

            if (Input.GetKeyDown(KeyCode.Space) && !isActionWrong)
            {
                if (CanAction && TargetAttack != null)
                {
                    pv.RPC("CoroutineActionAttack", RpcTarget.AllBuffered);
                    TargetAttack.GetComponent<PhotonView>().RPC("CoroutineGetHit", RpcTarget.AllBuffered);
                    TargetAttack = null;
                    FindMeGameManager.instance.AddScore();
                }
                else
                {
                    pv.RPC("CoroutineWrongAction", RpcTarget.AllBuffered);
                }
            }

            if (Input.GetKeyDown(KeyCode.F) && !boosterSpeed)
            {
                boosterSpeed = true;
                defaultSpeed = speed;
                speed += 5;
                StartCoroutine(WaitingSpeedDown());
            }
        }
    }

    private void FixedUpdate()
    {
        if (velocity != Vector3.zero)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
    }

    Vector3 MovingCharacter()
    {
        float _xMovement = Input.GetAxis("Horizontal");
        float _zMovement = Input.GetAxis("Vertical");

        Vector3 _movementHorizontal = transform.right * _xMovement;
        Vector3 _movementVertical = transform.forward * _zMovement;

        return (_movementHorizontal + _movementVertical).normalized * speed;
    }

    void RotateLight()
    {
        Vector3 targetDirection = (rb.position + velocity) - transPivotLight.position;

        float singlestep = speedRot * Time.deltaTime;

        Vector3 newDir = Vector3.RotateTowards(transPivotLight.forward, targetDirection, singlestep, 0.0f);

        transPivotLight.rotation = Quaternion.LookRotation(newDir);

    }

    [PunRPC]
    public void OtherTriggerActive(bool value)
    {
        ImageRightAction.SetActive(value);
        CanAction = value;
    }

    [PunRPC]
    public void CoroutineActionAttack()
    {
        StartCoroutine(ActionAttack());
    }

    public IEnumerator ActionAttack()
    {
        CanAction = false;
        controlEnable = false;
        if (pv.IsMine)
            ImageAction.SetActive(true);
        ImageRightAction.SetActive(false);
        yield return new WaitForSeconds(3f);
        if (pv.IsMine)
            ImageAction.SetActive(false);
        controlEnable = true;
        yield return new WaitForSeconds(3f);
        gameObject.tag = "Player";
    }

    [PunRPC]
    public void CoroutineGetHit()
    {
        StartCoroutine(GetHit());
    }

    public IEnumerator GetHit()
    {
        CanAction = false;
        controlEnable = false;
        if (pv.IsMine)
            ImageAction.SetActive(true);
        gameObject.tag = "NotPlayer";
        yield return new WaitForSeconds(3f);
        if (pv.IsMine)
        {
            ImageAction.SetActive(false);
            int randomRespawn = Random.Range(0, FindMeGameManager.instance.listSpawnPoint.Count - 1);
            transform.position = FindMeGameManager.instance.listSpawnPoint[randomRespawn].transSpawnPoint.position;
        }
        controlEnable = true;
        gameObject.tag = "Player";

    }

    [PunRPC]
    public void ActiveBoxTrigger(bool value)
    {
        boxTrigger.enabled = false;
    }

    [PunRPC]
    public void CoroutineWrongAction()
    {
        StartCoroutine(WrongAction());
    }

    public IEnumerator WrongAction()
    {
        Debug.Log(PhotonNetwork.NickName + " Wrong Action");
        isActionWrong = true;
        ImageWrongAction.SetActive(true);
        yield return new WaitForSeconds(5f);
        ImageWrongAction.SetActive(false);
        isActionWrong = false;
    }

    IEnumerator WaitingSpeedDown()
    {
        yield return new WaitForSeconds(5);
        speed = defaultSpeed;
        boosterSpeed = false;
    }
}
