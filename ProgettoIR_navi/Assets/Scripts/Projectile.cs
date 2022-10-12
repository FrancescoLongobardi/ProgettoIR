using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float life_time = 5f;

    void Update()
    {
        life_time -= Time.deltaTime;
        if(life_time < 0){
            Destroy(this.gameObject);
        }
    }
}
