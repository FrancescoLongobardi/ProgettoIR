using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{    
    public GameObject projectile;
    public float speed = 25f;
    public bool is_available=true;
    public float cooldown_cannon = 2f;
    public float shooting_cooldown_left;

    void Start(){
        is_available=true;
    }

    void Update()
    {   

            
       /* GameObject launched = Instantiate(projectile, transform.position, transform.rotation);
        launched.GetComponent<Rigidbody>().velocity = transform.up * speed;*/        
    }

    /* Decommentare questa per sparo con cooldown
    public GameObject Shoot(){
        if(is_available==false)
            return null;
        
        GameObject launched = Instantiate(projectile, transform.position, transform.rotation);
        launched.GetComponent<Rigidbody>().velocity = transform.up * speed;
        StartCoroutine(ShootCooldown()); 
        return launched;
    }
    */

    public GameObject Shoot(){   
        GameObject launched = Instantiate(projectile, transform.position, transform.rotation);
        launched.GetComponent<Rigidbody>().velocity = transform.up * speed;
        return launched;
    }

    private IEnumerator StartCooldown(){

        is_available = false;

        yield return new WaitForSeconds(cooldown_cannon);

        is_available = true;

    }

    private IEnumerator ShootCooldown() {
        is_available = false;
        shooting_cooldown_left = cooldown_cannon;
        while(shooting_cooldown_left > 0){
            shooting_cooldown_left -= Time.deltaTime;
            if(shooting_cooldown_left < 0)
                shooting_cooldown_left = 0;
            yield return null;
        }
        /*
        for(shooting_cooldown_left = cooldown_cannon; shooting_cooldown_left > 0; shooting_cooldown_left -= Time.deltaTime){
            yield return null;
        }
        */
        is_available = true;
    }

    public float GetShootingCooldownLeft(){
        return shooting_cooldown_left;
    }
    
    public void ResetCooldown(){
        shooting_cooldown_left = 0;
    }
}
