using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{

    public float verticalInput;
    private const float max_elevation = 63.823f;
    private const float min_elevation = 108.663f;
    //private const float max_right = 55.031f;
    //private const float max_left = -55.031f;
    private float rotationSpeed = 30f;
    void FixedUpdate()
    {
        verticalInput = Input.GetAxis("Vertical");

        if(verticalInput < 0 && transform.rotation.eulerAngles.z <= min_elevation)
            transform.Rotate(Vector3.back, verticalInput * rotationSpeed * Time.deltaTime);
        if(verticalInput > 0 && transform.rotation.eulerAngles.z >= max_elevation)
            transform.Rotate(Vector3.back, verticalInput * rotationSpeed * Time.deltaTime);

        //transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);

    }
}
