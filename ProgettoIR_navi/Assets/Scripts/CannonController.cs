using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    private GameObject ball_spawner;
    public float verticalInput;
    private const float max_elevation = 45f;
    private const float min_elevation = 100f;
   
    private float rotationSpeed = 30f;

    void Start(){
        ball_spawner = GameObject.Find("FirePoint");
    }

    void FixedUpdate()
    {
        verticalInput = Input.GetAxis("Vertical");

        if(verticalInput < 0 && transform.rotation.eulerAngles.z <= min_elevation)
            transform.Rotate(Vector3.back, verticalInput * rotationSpeed * Time.deltaTime);
        if(verticalInput > 0 && transform.rotation.eulerAngles.z >= max_elevation)
            transform.Rotate(Vector3.back, verticalInput * rotationSpeed * Time.deltaTime);

        //transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);

    }

    public void Shoot(){
        LaunchProjectile ball_spawner_script = ball_spawner.GetComponent<LaunchProjectile>();
        ball_spawner_script.Shoot();
    }

    public void LookWithSlerp(GameObject enemy){
        //Debug.DrawRay(cannon.transform.position, direction*5f, Color.green);
        //_lookRotation_cannon = Quaternion.LookRotation(direction, Vector3.right);
        //cannon.transform.localRotation =  Quaternion.Euler(cannon.transform.localRotation.eulerAngles.x, cannon.transform.localRotation.eulerAngles.y, 90 + _lookRotation_cannon.eulerAngles.x);
        LaunchProjectile ball_spawner_script = ball_spawner.GetComponent<LaunchProjectile>();
        float angle = 0.5f * (Mathf.Asin((Physics.gravity.y * Vector3.Distance(ball_spawner.transform.position, enemy.transform.position)) / (ball_spawner_script.speed * ball_spawner_script.speed)) * Mathf.Rad2Deg);
        
        if(!float.IsNaN(angle)){
            float possible_angle = 90f+angle;
            angle = Mathf.Clamp(-possible_angle, -min_elevation, -max_elevation);
            //transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0f, 0f, angle), Time.deltaTime * rotationSpeed);
            /*if(possible_angle>=max_elevation && possible_angle <= min_elevation){
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0f, 0f, angle), Time.deltaTime * rotationSpeed);
            }
            else if(possible_angle < max_elevation){
                Quaternion rot = Quaternion.AngleAxis(transform.localEulerAngles.z - max_elevation, Vector3.forward);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, rot, Time.deltaTime * rotationSpeed);
            } 
            else if(possible_angle > min_elevation){
                Quaternion rot = Quaternion.AngleAxis(min_elevation - transform.localEulerAngles.z, Vector3.forward);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, rot, Time.deltaTime * rotationSpeed);
            } */   
            transform.localEulerAngles = new Vector3(0f, 90f, -angle);
        }
    }
}
