using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public string[] enemyObjs;
    public Transform[] spawnPoints;

    public float nextSpawnDelay;
    public float curspawnDelay;

    public GameObject player;
    public Text scoreText;
    public Image[] LifeImage;
    public Image[] BoomImage;
    public GameObject gameOverSet;

    public ObjectManager objectManager;

    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;

    void Awake() {
        spawnList = new List<Spawn>();
        enemyObjs = new string[]{"EnemyL", "EnemyM", "EnemyS", "EnemyB"};
        ReadSpawnFile();
    }

    void ReadSpawnFile(){
        //#1. 변수 초화
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        //#2. 리스폰 파일 읽기
        TextAsset textFile = Resources.Load("Stage0") as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        //#3. 리스폰 데이터 생성
        while (stringReader != null)
        {
            string line = stringReader.ReadLine();
            if (line == null)
                break;

            string[] split = line.Split(',');
            Spawn spawn = new Spawn();
            spawn.delay = float.Parse(split[0]);
            spawn.type = split[1];
            spawn.point = int.Parse(split[2]);
            spawnList.Add(spawn);
            //Debug.Log(split[1]);
        }

        //#4. 텍스트파일 닫기.
        stringReader.Close();
        nextSpawnDelay = spawnList[0].delay;
    }

    void Update() {
       curspawnDelay += Time.deltaTime;
       if(curspawnDelay > nextSpawnDelay && !spawnEnd){
           SpawnEnemy();
        //    nextSpawnDelay = Random.Range(0.5f, 3f);
           curspawnDelay = 0;
       }
    
       Player playerLogic = player.GetComponent<Player>();
       //scoreText.text = playerLogic.score.ToString(); 
       scoreText.text = string.Format("{0:n0}" ,playerLogic.score); 
    }

    void SpawnEnemy(){
        int enemyIndex = 0;
        //Debug.Log(spawnList[spawnIndex].type);
        switch(spawnList[spawnIndex].type){
            case "L":
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "S":
                enemyIndex = 2;
                break;
            case "B":
                enemyIndex = 3;
                break;
        }
        // int ranEnemy = Random.Range(0,3);
        // int ranPoint = Random.Range(0,9);
        int enemyPoint = spawnList[spawnIndex].point;
        GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);
        enemy.transform.position = spawnPoints[enemyPoint].position;

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player;
        enemyLogic.objectManager = objectManager;

        if(enemyPoint == 5 || enemyPoint == 6){
            enemy.transform.Rotate(Vector3.back * 90); //회전
            rigid.velocity = new Vector2(enemyLogic.speed * (-1), -1);
        }
        else if(enemyPoint == 7 || enemyPoint == 8){
            enemy.transform.Rotate(Vector3.forward * 90); //회전
            rigid.velocity = new Vector2(enemyLogic.speed, -1);
        }
        else{
            rigid.velocity = new Vector2(0,enemyLogic.speed*(-1));
        }

        //#1. 리스폰 인덱스 증가
        spawnIndex++;
        if(spawnIndex == spawnList.Count){
            spawnEnd = true;
            return;
        }

        //#2. 다음 리스폰 시간 계산
        nextSpawnDelay = spawnList[spawnIndex].delay;
    }

    public void UpdateLifeIcon(int life){
        for(int index = 0; index < 3; index++){
            LifeImage[index].color = new Color(1,1,1,0);
        }

        for(int index = 0; index < life; index++){
            LifeImage[index].color = new Color(1,1,1,1);
        }
    }

    public void UpdateBoomIcon(int boom){
        for(int index = 0; index < 3; index++){
            BoomImage[index].color = new Color(1,1,1,0);
        }

        for(int index = 0; index < boom; index++){
            BoomImage[index].color = new Color(1,1,1,1);
        }
    }

    public void RespawnPlayer(){
        Invoke("RespawnPlayerExe", 2f);
    }
    
    void RespawnPlayerExe(){
        
        player.transform.position = Vector3.down * 4f;
        player.SetActive(true);

        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isShot = false;
    }

    public void GameOver(){
        gameOverSet.SetActive(true);
    }
    public void GameRetry(){
        SceneManager.LoadScene(0);
    }
}

