using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{

    public float verticalInput;
    public float horizontalInput;
    private const float max_elevation = 40.911f;
    private const float min_elevation = 102.764f;
    private const float max_right = 55.031f;
    private const float max_left = -55.031f;
    public float rotationSpeed = 8f;
    void FixedUpdate()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        Debug.Log(transform.rotation.eulerAngles);

        if(verticalInput < 0 && transform.rotation.eulerAngles.z <= min_elevation)
            transform.Rotate(Vector3.back, verticalInput * rotationSpeed * Time.deltaTime);
        if(verticalInput > 0 && transform.rotation.eulerAngles.z >= max_elevation)
            transform.Rotate(Vector3.back, verticalInput * rotationSpeed * Time.deltaTime);

        float y_euler_angle = (transform.rotation.eulerAngles.y > 180) ? transform.rotation.eulerAngles.y - 360 : transform.rotation.eulerAngles.y;
        if(horizontalInput < 0 && y_euler_angle >= max_left)
            transform.Rotate(Vector3.right, horizontalInput * rotationSpeed * Time.deltaTime);
        if(horizontalInput > 0 && y_euler_angle <= max_right)
            transform.Rotate(Vector3.right, horizontalInput * rotationSpeed * Time.deltaTime);

        //transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);

    }
}
