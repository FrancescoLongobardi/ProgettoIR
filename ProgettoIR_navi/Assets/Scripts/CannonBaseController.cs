using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBaseController : MonoBehaviour
{
    private GameObject cannon;
    public float horizontalInput;
    public float rotationSpeed = 10f;
    // Start is called before the first frame update
    private float target_angle = float.NaN;

    void Start(){
        cannon = GameObject.Find("Cannon");
    }

    // Update is called once per frame
    /*
    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, horizontalInput * Time.deltaTime * rotationSpeed);
    }
    */

    public void rotateCannonBase(float rot_input){
        float local_y_angle = Get180Angle(transform.localEulerAngles.y);
        
        if (Mathf.Abs(GetLocalYAngle() - target_angle) < 0.01f){
            //Debug.Log("<color=green>Equal angles cannon base, RETURN </color>");
            return;
        }

        if(rot_input == 0){
            //Debug.Log("<color=red> Diff angles cannon base</color>"+ target_angle+ " "+ GetLocalYAngle());
            transform.localEulerAngles = new Vector3(0f, target_angle, 0f);
            return;
        }

        transform.Rotate(Vector3.up, rot_input * rotationSpeed * Time.deltaTime);
    }

    public void rotateCannonBase_training(float rot_input){
        float local_y_angle = Get180Angle(transform.localEulerAngles.y);
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime * rot_input);
    }

    /*
    private float Get180Angle(float angle){
        return (angle > 180f ? angle - 360f : angle); 
    }
    */
    
    private float Get180Angle(float angle){
        if(angle > 180f)
            return -(180f - (angle - 180f));
        else if(angle < -180f)
            return 180f + (angle + 180f);
        else return angle;
    }

    public float CalculateInputForAimbot(GameObject enemy, float agent_y_rot){
        Vector3 direction = (enemy.transform.position - transform.position).normalized;
        direction.y = 0f;
        Quaternion _lookRotation = Quaternion.LookRotation(direction, transform.forward);
        //Debug.DrawRay(transform.position, transform.forward*10, Color.red);
        //Debug.DrawRay(transform.position, direction*10, Color.blue);    
        float possible_rotation = Get180Angle(Get180Angle(_lookRotation.eulerAngles.y) - agent_y_rot);
        target_angle = possible_rotation;
        float local_y_angle = GetLocalYAngle();
        
        float rot_input = 0;

        if(Mathf.Abs(Mathf.DeltaAngle(GetLocalYAngle(), target_angle)) >= Time.deltaTime * rotationSpeed)
            //rot_input = Mathf.Sign(Get360Angle(target_angle) - Get360Angle(GetYAngle()) );
            rot_input = Mathf.Sign(Mathf.DeltaAngle(GetLocalYAngle(), target_angle));

        /*
        //Debug.Log(possible_rotation + " " + local_y_angle + " " + Time.deltaTime*rotationSpeed);
        if(Mathf.Abs(possible_rotation - local_y_angle) >= Time.deltaTime*rotationSpeed)
            rot_input = Mathf.Sign(possible_rotation - local_y_angle);
        */
        return rot_input;
    }

    private float GetLocalYAngle(){
        return Get180Angle(transform.localEulerAngles.y);
    }

    public bool CheckRotationCompleted(){
        //Debug.Log(Mathf.Abs(GetLocalYAngle() - target_angle));
        return Mathf.Abs(GetLocalYAngle() - target_angle) < 0.01f;
    }

    public GameObject Shoot(){
        return cannon.GetComponent<CannonController>().Shoot();
    }
}
