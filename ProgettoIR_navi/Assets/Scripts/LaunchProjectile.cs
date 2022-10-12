using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{    
    public GameObject projectile;
    public float speed = 20f;

    void Update()
    {   

        if(Input.GetButtonDown("Fire1")){
            GameObject launched = Instantiate(projectile, transform.position, transform.rotation);
            launched.GetComponent<Rigidbody>().velocity = transform.up * speed;
        }
    }
}
