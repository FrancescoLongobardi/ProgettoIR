using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class EnemySpawnerController : MonoBehaviour
{

    public List<GameObject> permanent_enemies;
    public List<GameObject> enemies;
    public GameObject plane;
    public GameObject enemy_prefab;
    private int n_enemies = 1; //TODO vedere se si può cancellare
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
        SpawnEnemies();
        spawned = true;
    }

    public void SpawnForTraining(){   
        enemies = new List<GameObject>();
        RelocateEnemies();
    }

    void CheckBounds(ref Vector3 pos){
        //Debug.Log("Prima: " + pos);
        pos.x = Mathf.Clamp(pos.x, min_x, max_x);
        pos.z = Mathf.Clamp(pos.z, min_z, max_z);
        //Debug.Log("Dopo: " + pos);
    }

    public void SpawnForCurriculum(Vector3 cannonPosition, Quaternion cannon_base_rotation, float max_range, float angle1, float angle2){
        enemies.Clear();
        Quaternion randAng = Quaternion.Euler(0, Random.Range(angle1, angle2), 0);
        randAng = cannon_base_rotation * randAng;
        float randomRange = Random.Range(40f, max_range);
        Vector3 spawnPos = cannonPosition + randAng * Vector3.forward * randomRange;
        spawnPos.y = 0.52f; //Over the plane
        Vector3 dir = cannonPosition - spawnPos;
        Quaternion rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
        permanent_enemies[0].transform.localPosition = spawnPos;
        permanent_enemies[0].transform.localRotation = rotation;
        permanent_enemies[0].SetActive(true);
        permanent_enemies[0].GetComponent<EnemyController>().speed = Academy.Instance.EnvironmentParameters.GetWithDefault("enemy_speed", 1f);
        enemies.Add(permanent_enemies[0]);
    }

    public void SpawnForDemonstration(Vector3 cannonPosition, Quaternion cannon_base_rotation, float max_range, float angle1, float angle2){
        //Debug.Log(permanent_enemies[0]);
        enemies.Clear();
        Quaternion randAng = Quaternion.Euler(0, Random.Range(angle1, angle2), 0);
        randAng = cannon_base_rotation * randAng;
        float randomRange = Random.Range(5, max_range);
        Vector3 spawnPos = cannonPosition + randAng * Vector3.forward * randomRange;
        //Debug.Log("Old: " + spawnPos);
        CheckBounds(ref spawnPos);
        //Debug.Log("New: " + spawnPos);
        spawnPos.y = 0.52f; //Over the plane
        //spawnPos.y = -0.939f;
        //Debug.Log(spawnPos);
        Vector3 enemy_rot = new Vector3(0f, Random.Range(0f,360f), 0f);
        //Debug.Log(spawnPos);
        //enemies.Add(Instantiate(enemy_prefab, spawnPos, Quaternion.Euler(enemy_rot))); // TODO: Trovare le coordinate e passare a RelocateEnemies
        permanent_enemies[0].transform.localPosition = spawnPos;
        permanent_enemies[0].transform.localRotation = Quaternion.Euler(enemy_rot);
        permanent_enemies[0].SetActive(true);
        enemies.Add(permanent_enemies[0]);
    }
    
    void Update()
    {      
        
        
    }
    
    public void RemoveEnemyFromList(GameObject enemy){
        enemy.SetActive(false);
        enemies.Remove(enemy);
    }

    private void SpawnEnemies(){
        permanent_enemies = new List<GameObject>();
        for(int i = 0; i < n_enemies; i++){
            GameObject en = Instantiate(enemy_prefab, plane.transform.parent, false);
            en.transform.localPosition = new Vector3(0, 0.52f, 0);
            //en.transform.localPosition = new Vector3(0, -0.939f, 0);
            en.transform.localRotation = Quaternion.identity;
            en.SetActive(false);
            permanent_enemies.Add(en);
        }
    }

    private void RelocateEnemies(){
        //enemies = new List<GameObject>();
        bool ok_coords = false;
        float pos_x, pos_z;
        for(int i = 0; i < n_enemies; i++){
            Vector3 enemy_pos = Vector3.zero;
            Vector3 enemy_rot = Vector3.zero;
            while(!ok_coords){
                pos_x = Random.Range(min_x, max_x);
                pos_z = Random.Range(min_z, max_z);
                enemy_pos = new Vector3(pos_x, plane.transform.localPosition.y + 0.51f, pos_z);
                //enemy_pos = new Vector3(pos_x, -0.939f, pos_z);
                enemy_rot = new Vector3(0f, Random.Range(0f,360f), 0f);
                bool ok_enemy = true;
                for(int j = 0; j < i; j++){
                    //Debug.Log("Distanza: " + Vector3.Distance(enemy_pos, enemies[j].transform.position));
                    if(Vector3.Distance(enemy_pos, enemies[j].transform.localPosition) < min_distance*2){
                        ok_enemy = false;
                        break;
                    }
                }
                if(ok_enemy)
                    ok_coords = true;
                
            }
            permanent_enemies[i].transform.localPosition = enemy_pos;
            permanent_enemies[i].transform.localRotation = Quaternion.Euler(enemy_rot);
            float[] parameters = new float[4];
            parameters[0] = min_x;
            parameters[1] = min_z;
            parameters[2] = max_x;
            parameters[3] = max_z;
            permanent_enemies[i].SetActive(true);
            permanent_enemies[i].SendMessage("Construct", parameters);
            enemies.Add(permanent_enemies[i]);
            ok_coords = false;
        }
    }
    
    /*
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
        
    }
    */

}
