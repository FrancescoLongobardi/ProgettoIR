using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{   
    private float min_x, min_z, max_x, max_z;
    public float speed;
    private int direction;
    // Start is called before the first frame update
    void Start()
    {
        int[] dirs = {1, -1};
        speed = Random.Range(4f, 7f);
        direction = dirs[Random.Range(0, 2)];
    }

    void Construct(float[] boundaries){
        min_x = boundaries[0];
        min_z = boundaries[1];
        max_x = boundaries[2];
        max_z = boundaries[3];
    }

    // Update is called once per frame
    void Update()
    {
        /*
        Vector3 newPos = transform.localPosition + direction * transform.forward * Time.deltaTime * speed;
        if (newPos.x < min_x || newPos.x > max_x || newPos.z < min_z || newPos.z > max_z){
            direction = -direction;
            transform.localPosition += direction * transform.forward * Time.deltaTime * speed;
        }
        else{
            transform.localPosition += direction * transform.forward * Time.deltaTime * speed;
        }
        */

    }


}
