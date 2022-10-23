using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{

    public List<GameObject> enemies;
    public GameObject plane;
    public GameObject enemy_prefab;
    private int n_enemies = 4; //TODO vedere se si può cancellare
    private float min_x, max_x, min_z, max_z;
    private float min_x_demonstration, max_x_demonstration, min_z_demonstration, max_z_demonstration;
    private const float min_distance = 5f;
    private float boundary_limit = 3;
    public bool spawned = false;

    void Start(){
        Vector3 bounds = plane.GetComponent<MeshRenderer>().localBounds.size;
        min_x = -1 * plane.transform.localScale.x * (bounds.x / 2) + boundary_limit;
        min_z = -1 * plane.transform.localScale.z * (bounds.z / 2) + boundary_limit;
        max_x = plane.transform.localScale.x * (bounds.x / 2) - boundary_limit;
        max_z = plane.transform.localScale.z * (bounds.z / 2) - boundary_limit;
    }

    public void SpawnForTraining(){   
        enemies = new List<GameObject>();
        SpawnEnemies();
        spawned = true;
    }

    void CheckBounds(ref Vector3 pos){
        pos.x = Mathf.Clamp(pos.x, min_x, max_x);
        pos.z = Mathf.Clamp(pos.z, min_z, max_z);
    }

    public void SpawnForDemonstration(Vector3 cannonPosition, Quaternion cannon_base_rotation, float cannonMaxRange, float angle1, float angle2){
        Quaternion randAng = Quaternion.Euler(0, Random.Range(angle1, angle2), 0);
        randAng = cannon_base_rotation * randAng;
        float randomRange = Random.Range(5f, -cannonMaxRange);
        Vector3 spawnPos = cannonPosition + randAng * Vector3.forward * randomRange;
        Debug.Log("Old: " + spawnPos);
        CheckBounds(ref spawnPos);
        Debug.Log("New: " + spawnPos);
        spawnPos.y = 0.6f; //Over the plane
        //Debug.Log(spawnPos);
        Vector3 enemy_rot = new Vector3(0f, Random.Range(0f,360f), 0f);
        enemies.Add(Instantiate(enemy_prefab, spawnPos, Quaternion.Euler(enemy_rot)));
    }
    
    void Update()
    {      
        
        
    }
    
    public void RemoveEnemyFromList(GameObject enemy){
        enemies.Remove(enemy);
        Destroy(enemy);
    }

    private void SpawnEnemies(){
        bool ok_coords = false;
        float pos_x, pos_z;
        for(int i = 0; i < n_enemies; i++){
            Vector3 enemy_pos = Vector3.zero;
            Vector3 enemy_rot = Vector3.zero;
            while(!ok_coords){
                pos_x = Random.Range(min_x, max_x);
                pos_z = Random.Range(min_z, max_z);
                enemy_pos = new Vector3(pos_x, plane.transform.localPosition.y + 0.51f, pos_z);
                enemy_rot = new Vector3(0f, Random.Range(0f,360f), 0f);
                bool ok_enemy = true;
                for(int j = 0; j < i; j++){
                    //Debug.Log("Distanza: " + Vector3.Distance(enemy_pos, enemies[j].transform.position));
                    if(Vector3.Distance(enemy_pos, enemies[j].transform.position) < min_distance*2){
                        ok_enemy = false;
                        break;
                    }
                }
                if(ok_enemy)
                    ok_coords = true;
                
            }
            GameObject enemy = Instantiate(enemy_prefab, enemy_pos, Quaternion.Euler(enemy_rot)); 
            float[] parameters = new float[4];
            parameters[0] = min_x;
            parameters[1] = min_z;
            parameters[2] = max_x;
            parameters[3] = max_z;
            enemy.SendMessage("Construct", parameters);
            enemies.Add(enemy);
            ok_coords = false;
        }
        /*
        for(int i = 0; i < n_enemies; i++){
            for(int j = 0; j < n_enemies; j++){
                Debug.Log(Vector3.Distance(enemies[i].transform.position, enemies[j].transform.position));
            }
        }
        */
    }

}
