using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    private GameObject ball_spawner;
    public float verticalInput;
    private const float max_elevation = 45f;
    private const float min_elevation = 100f;
   
    public float rotationSpeed = 30f;
    private LaunchProjectile ball_spawner_script;

    void Start(){
        ball_spawner = GameObject.Find("FirePoint");
        ball_spawner_script = ball_spawner.GetComponent<LaunchProjectile>();
    }

    void FixedUpdate()
    {
        verticalInput = Input.GetAxis("Vertical");

        if(verticalInput < 0 && transform.rotation.eulerAngles.z <= min_elevation)
            transform.Rotate(Vector3.back, verticalInput * rotationSpeed * Time.deltaTime);
        if(verticalInput > 0 && transform.rotation.eulerAngles.z >= max_elevation)
            transform.Rotate(Vector3.back, verticalInput * rotationSpeed * Time.deltaTime);

    }

    public GameObject Shoot(){
        return ball_spawner_script.Shoot();
    }

    public void LookWithSlerp(GameObject enemy){
        LaunchProjectile ball_spawner_script = ball_spawner.GetComponent<LaunchProjectile>();
        float angle = 0.5f * (Mathf.Asin((Physics.gravity.y * Vector3.Distance(ball_spawner.transform.position, enemy.transform.position)) / (ball_spawner_script.speed * ball_spawner_script.speed)) * Mathf.Rad2Deg);
        
        if(!float.IsNaN(angle)){
            float possible_angle = 90f+angle;
            float fixed_angle = Mathf.Clamp(-possible_angle, -min_elevation, -max_elevation);
            transform.localEulerAngles = new Vector3(0f, 90f, -fixed_angle);
        }
    }

    public void rotateCannon(float elevate){
        float angle = transform.localEulerAngles.z + elevate * rotationSpeed * Time.deltaTime;
        //float possible_angle = 90f+angle;
        if (angle >= max_elevation && angle <= min_elevation){
            transform.Rotate(Vector3.forward, elevate * rotationSpeed * Time.deltaTime);
        }

    }

    public float GetShootingCooldownLeft(){
        return ball_spawner_script.GetShootingCooldownLeft();
    }
}
