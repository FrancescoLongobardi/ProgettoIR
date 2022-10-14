using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{   
    public LaunchProjectile ball_spawner;
    public EnemySpawnerController enemy_spawner;
    private Vector3 _direction;
    private Vector3 _direction_cannon;
    private GameObject cannon_base;
    private GameObject cannon;
    private Quaternion _lookRotation;
    private Quaternion _lookRotation_cannon;

    void Start()
    {
        cannon_base = GameObject.Find("CannonBase");
        cannon = GameObject.Find("Cannon");
    }

    void Update()
    {   
        
        GameObject enemies = enemy_spawner.enemies[0];
        _direction = (enemies.transform.position - cannon_base.transform.position).normalized;
        _direction.y = 0f;
        _lookRotation = Quaternion.LookRotation(_direction, Vector3.up);
        cannon_base.transform.rotation = Quaternion.Slerp(cannon_base.transform.rotation, _lookRotation, Time.deltaTime * cannon_base.GetComponent<CannonBaseController>().rotationSpeed);

        _direction_cannon = (enemies.transform.position - cannon.transform.position).normalized;
        Debug.DrawRay(cannon.transform.position, _direction_cannon*5f, Color.green);
        _direction_cannon.x = 0f;
        _direction_cannon.y = 0f;
        //_lookRotation_cannon = Quaternion.LookRotation(_direction_cannon, Vector3.right);
        Debug.Log(_lookRotation_cannon.eulerAngles);
        //cannon.transform.LookAt(enemies.transform, Vector3.right);
        _lookRotation_cannon = Quaternion.FromToRotation(Vector3.forward, Vector3.down) * Quaternion.LookRotation(_direction_cannon, Vector3.right);
        cannon.transform.localRotation = Quaternion.Slerp(cannon.transform.localRotation, _lookRotation_cannon , Time.deltaTime * cannon_base.GetComponent<CannonBaseController>().rotationSpeed);            

        float? highAngle = 0f;
        float? lowAngle = 0f;
            
        /*CalculateAngleToHitTarget(out highAngle, out lowAngle, enemies.transform.position);
        if(highAngle != null){
            float angle = (float)highAngle;
            GameObject.Find("Cannon").transform.localEulerAngles = new Vector3(0f, 90f, angle);
            //transform.eulerAngles = new Vector3(0f, transform.rotation.eulerAngles.y, 0f);
        }*/
        


    }

    void CalculateAngleToHitTarget(out float? theta1, out float? theta2, Vector3 enemypos)
    {
        //Initial speed
        float v = ball_spawner.speed;

        Vector3 targetVec = enemypos - cannon.transform.position;

        //Vertical distance
        float y = targetVec.y;

        //Reset y so we can get the horizontal distance x
        targetVec.y = 0f;

        //Horizontal distance
        float x = targetVec.magnitude;

        //Gravity
        float g = 9.81f;


        //Calculate the angles
        
        float vSqr = v * v;

        float underTheRoot = (vSqr * vSqr) - g * (g * x * x + 2 * y * vSqr);

        //Check if we are within range
        if (underTheRoot >= 0f)
        {
            float rightSide = Mathf.Sqrt(underTheRoot);

            float top1 = vSqr + rightSide;
            float top2 = vSqr - rightSide;

            float bottom = g * x;

            theta1 = Mathf.Atan2(top1, bottom) * Mathf.Rad2Deg;
            theta2 = Mathf.Atan2(top2, bottom) * Mathf.Rad2Deg;
        }
        else
        {
            theta1 = null;
            theta2 = null;
        }
    }
}
