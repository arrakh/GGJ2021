using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField]float speed = 5;
    [SerializeField]Transform transPivotLight;
    [SerializeField] float speedRot = 5;

    private Vector3 velocity = Vector3.zero;
    private Rigidbody rb;

    public bool controlEnable;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controlEnable = false;
    }

    private void Update()
    {
        if (controlEnable)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            velocity = MovingCharacter();

            RotateLight();
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
}
