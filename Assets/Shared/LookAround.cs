using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAround : MonoBehaviour
{
    public new Transform camera;
    private float speed = 1f;
    private const float anglePerSecond = 1f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        var forward = Input.GetAxis("Vertical");
        var right = Input.GetAxis("Horizontal");
        var up = Input.GetAxis("UpDown");

        speed = Input.GetButton("Fire3") ? 20 : 1;

        var position = transform.position;
        position += camera.forward * (forward * speed);
        position += transform.up * (up * speed);
        position += transform.right * (right * speed);
        transform.position = position;

        var rotateY = Input.GetAxis("Mouse X") != 0f ? Mathf.Sign(Input.GetAxis("Mouse X")) : 0f;
        var rotateX = Input.GetAxis("Mouse Y") != 0f ? Mathf.Sign(Input.GetAxis("Mouse Y")) : 0f;

        // we look side to side
        transform.Rotate(new Vector3(0, rotateY * anglePerSecond));

        // camera looks up and down
        camera.Rotate(new Vector3(-rotateX * anglePerSecond, 0));
    }
}