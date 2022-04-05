using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public float speed;
    public int power;
    public int maxpower;
    public int boom;
    public int maxboom;

    public float maxShotDelay;
    public float curShotDelay;
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    public int life;
    public int score;
    
    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject boomEffect; 

    public GameManager gameManager;
    public ObjectManager objectManager;
    public bool isShot;
    public bool isBoomtype;

    public GameObject[] followers;
    Animator anim;
    void Awake() 
    {
        anim = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
        Boom();
        Reload();
    }

    void Fire()
    {
        if (!Input.GetButton("Fire1"))
            return;

        if (curShotDelay < maxShotDelay)
            return;

        switch(power){
            case 1:
                GameObject bullet = objectManager.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;

                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 2:
                GameObject bulletR = objectManager.MakeObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right*0.1f;
                GameObject bulletL = objectManager.MakeObj("BulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left*0.1f;
                
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            default:
            // case 3:
            // case 4:
            // case 5:
            // case 6:
                GameObject bulletRR = objectManager.MakeObj("BulletPlayerA");
                bulletRR.transform.position = transform.position + Vector3.right*0.35f;
                GameObject bulletCC = objectManager.MakeObj("BulletPlayerB");
                bulletCC.transform.position = transform.position;
                GameObject bulletLL = objectManager.MakeObj("BulletPlayerA");
                bulletLL.transform.position = transform.position + Vector3.left*0.35f;
                
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();

                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
        }

        curShotDelay = 0;
    }


    void Boom(){
        if(!Input.GetButton("Fire2"))
            return;

        if(isBoomtype)
            return;
        if (boom == 0)
            return;

        boom--;
        isBoomtype = true;
        gameManager.UpdateBoomIcon(boom);

        boomEffect.SetActive(true);
        Invoke("OffBoomsEffect", 4f);
        GameObject[] enemieL = objectManager.GetPool("EnemyL");
        GameObject[] enemieM = objectManager.GetPool("EnemyM");
        GameObject[] enemieS = objectManager.GetPool("EnemyS");

        for(int index=0;index<enemieL.Length;index++){
            if(enemieL[index].activeSelf){
                Enemy enemyLogic = enemieL[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }

        for(int index=0;index<enemieM.Length;index++){
            if(enemieM[index].activeSelf){
                Enemy enemyLogic = enemieM[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }

        for(int index=0;index<enemieS.Length;index++){
            if(enemieS[index].activeSelf){
                Enemy enemyLogic = enemieS[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }

        GameObject[] bulletA = objectManager.GetPool("BulletEnemyA");
        GameObject[] bulletB = objectManager.GetPool("BulletEnemyB");
        
        for(int index=0;index<bulletA.Length;index++){
            if(bulletA[index].activeSelf){
              bulletA[index].SetActive(false);
            }
        }

        for(int index=0;index<bulletB.Length;index++){
            if(bulletB[index].activeSelf){
               bulletB[index].SetActive(false);
            }
        }

    }

    void Reload(){
        curShotDelay += Time.deltaTime;
    }
    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        if(h == 1 || h == -1 || h == 0)
        {
            anim.SetInteger("Input",(int)h);
        }
        if((isTouchRight && h > 0) || (isTouchLeft && h < 0))
        {
            h = 0;
        }
        float v = Input.GetAxis("Vertical");
        if ((isTouchTop && v > 0) || (isTouchBottom && v < 0))
        {
            v = 0;
        }
        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h,v,0)*speed*Time.deltaTime;

        transform.position = curPos + nextPos;
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.tag == "Border")
        {
            switch(other.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
            }
        }
        else if(other.gameObject.tag == "Enemy" || other.gameObject.tag == "EnemyBullet")
        {
            if(isShot)
                return;

            isShot = true;
            life--;
            gameManager.UpdateLifeIcon(life);

            if(life == 0){
                gameManager.GameOver();
            }
            else{
                gameManager.RespawnPlayer();     
            }
            // Debug.Log(life.ToString());
            // gameManager.RespawnPlayer();
            gameObject.SetActive(false);
        }
        else if(other.gameObject.tag == "Item")
        {
           Item item = other.gameObject.GetComponent<Item>();
           switch(item.type){
               case "Coin":
                   score += 1000;
                   break;
               case "Power":
                   if(power == maxpower)
                       score += 500;
                    else{
                        power++;
                        AddFollower();
                    }
                   break;
               case "Boom":
                    if(boom == maxboom)
                        score += 500;
                    else
                        boom++;
                        gameManager.UpdateBoomIcon(boom);
                   break;                       
            }
            other.gameObject.SetActive(false);
        }
    }

    void AddFollower(){
        if(power == 4)
            followers[0].SetActive(true);
        else if(power == 5)
            followers[1].SetActive(true);
        else if(power == 6)
            followers[2].SetActive(true);
    }

    void OffBoomsEffect(){
        boomEffect.SetActive(false);
        isBoomtype = false;
    }

    void OnTriggerExit2D(Collider2D other) 
    {
       if(other.gameObject.tag == "Border")
        {
            switch(other.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
            }
        }
    }
}

