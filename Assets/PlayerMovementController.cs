using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    Rigidbody rigidbody;
    [SerializeField]
    float playerSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 m_Input = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");
        rigidbody.MovePosition(transform.position + m_Input * Time.deltaTime * playerSpeed);
    }
}
