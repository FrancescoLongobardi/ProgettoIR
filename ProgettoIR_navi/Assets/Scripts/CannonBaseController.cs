using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBaseController : MonoBehaviour
{

    public float horizontalInput;
    public float rotationSpeed = 30f;
    static float h;
    // Start is called before the first frame update

    void Awake()
    {
        //Can use a less precise h to speed up calculations
        //Or a more precise to get a more accurate result
        //But lower is not always better because of rounding errors
        h = Time.fixedDeltaTime * 1f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, horizontalInput * Time.deltaTime * rotationSpeed);
    }

    void LookWithSlerp(Quaternion lookRotation){
        //TODO: Quaternion.Slerp ma si deve fermare se sta ruotando verso il basso o verso l'alto
    }
}
