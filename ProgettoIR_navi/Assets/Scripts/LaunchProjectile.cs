using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{    
    public GameObject projectile;
    public float speed = 20f;
    public bool is_available=true;
    public float cooldown_cannon = 2f;


    void Start(){
        is_available=true;
    }

    void Update()
    {   

            
       /* GameObject launched = Instantiate(projectile, transform.position, transform.rotation);
        launched.GetComponent<Rigidbody>().velocity = transform.up * speed;*/        
    }

    public void Shoot(){
        if(is_available==false)
            return;
        
        GameObject launched = Instantiate(projectile, transform.position, transform.rotation);
        launched.GetComponent<Rigidbody>().velocity = transform.up * speed;
        StartCoroutine(StartCooldown()); 
    }

    private IEnumerator StartCooldown(){

        is_available = false;

        yield return new WaitForSeconds(cooldown_cannon);

        is_available = true;

    }
    
}
