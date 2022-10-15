using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBaseController : MonoBehaviour
{
    private GameObject cannon;
    public float horizontalInput;
    public float rotationSpeed = 30f;
    private const float max_left = -115f;
    private const float max_right = -65f;
    // Start is called before the first frame update

    void Start(){
        cannon = GameObject.Find("Cannon");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, horizontalInput * Time.deltaTime * rotationSpeed);
    }

    public void LookWithSlerp(GameObject enemy){
        Vector3 direction = (enemy.transform.position - transform.position).normalized;
        direction.y = 0f;
        Quaternion _lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        float rotation_angle = (_lookRotation.eulerAngles.y > 180f) ? _lookRotation.eulerAngles.y -360f : _lookRotation.eulerAngles.y;
        float possible_rotation = rotation_angle;
        if (possible_rotation >= max_left && possible_rotation <= max_right)
            transform.localRotation = Quaternion.Slerp(transform.localRotation, _lookRotation, Time.deltaTime * rotationSpeed);
        else if(possible_rotation > max_right){
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0f, max_right, 0f), Time.deltaTime * rotationSpeed);
        }
        else if(possible_rotation < max_left){
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0f, max_left, 0f), Time.deltaTime * rotationSpeed);
        }

        cannon.GetComponent<CannonController>().LookWithSlerp(enemy);
    }

    public void Shoot(){
        cannon.GetComponent<CannonController>().Shoot();
    }
}
