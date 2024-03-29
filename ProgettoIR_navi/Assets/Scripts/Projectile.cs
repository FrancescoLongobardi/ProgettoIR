using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class Projectile : MonoBehaviour
{
    private float life_time = 5f;
    public Agent agent;
    public EnemySpawnerController enemy_spawner;
    private int agent_type;

    void Update()
    {
        life_time -= Time.deltaTime;
        if(life_time < 0){
            Destroy(this.gameObject);
        }
    }

    void Construct(Object[] parametersConstruct){
        enemy_spawner = (EnemySpawnerController) parametersConstruct[1];
        agent = (AgentController) parametersConstruct[0];
        agent_type = 1;
    }

    void ConstructNoRaycast(Object[] parametersConstruct){
        enemy_spawner = (EnemySpawnerController) parametersConstruct[1];
        agent = (AgentControllerNoRaycast) parametersConstruct[0];
        agent_type = 2;
    }

    void OnCollisionEnter(Collision other)
    {   
        if(other.gameObject.tag == "water"){
            Vector3 contact_point = other.contacts[0].point;
            if(agent_type == 1){
                ((AgentController) agent).enemy_miss(find_nearest_enemy(contact_point));
                Destroy(gameObject);
            }
            else if (agent_type == 2){
                ((AgentControllerNoRaycast) agent).enemy_miss(find_nearest_enemy(contact_point));
                Destroy(gameObject);
            }
 
        }
        else if(other.gameObject.tag == "enemy"){
            if(agent_type == 1){
                Destroy(gameObject);
                ((AgentController) agent).enemy_hit(other.gameObject);
            }
            else if (agent_type == 2){
                Destroy(gameObject);
                ((AgentControllerNoRaycast) agent).enemy_hit(other.gameObject);
            }
        }
    }

    float find_nearest_enemy(Vector3 contact_point){
        float min = float.MaxValue;
        for(int i = 0; i < enemy_spawner.enemies.Count; i++){
            float dist = Vector3.Distance(enemy_spawner.enemies[i].transform.position, contact_point);
            if(dist < min)
                min = dist;
        }
        return min;
    }
}
