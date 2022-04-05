using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public float speed;
    public int health;
    public int enemyScore;
    public Sprite[] sprites;

    SpriteRenderer spriteRenderer;

    public float maxShotDelay;
    public float curShotDelay;
    
    public GameObject bulletObjA;
    public GameObject bulletObjB;

    public GameObject itemCoin;
    public GameObject itemPower;
    public GameObject itemBoom;
    
    public GameObject player;
    public ObjectManager objectManager;
    Animator anim;

    public int patternIndex;
    public int curPatternCount;
    public int[] maxPatternCount;

    void Awake()
    {
      spriteRenderer = GetComponent<SpriteRenderer>();
      if(enemyName == "B"){
        anim = GetComponent<Animator>();
      }
    }

    void OnEnable() 
    {
        switch (enemyName)
        {
            case "B":
                health = 3000;
                Invoke("Stop",2);
                break;
            case "L":
                health = 30;
                break;
            case "M":
                health = 10;
                break;
             case "S":
                health = 3;
                break;
        }
    }
    void Stop()
    {
        if(!gameObject.activeSelf)
            return;

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        Invoke("Think",2);
    }
    void Think(){
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;
        curPatternCount = 0;
        switch(patternIndex)
        {
            case 0:
                FireFoward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
        }
    }

    void FireFoward()
    {
        GameObject bulletR = objectManager.MakeObj("BulletBossA");
        bulletR.transform.position = transform.position + Vector3.right * 0.3f;
        GameObject bulletRR = objectManager.MakeObj("BulletBossA");
        bulletRR.transform.position = transform.position + Vector3.right * 0.45f;
        GameObject bulletL = objectManager.MakeObj("BulletBossA");
        bulletL.transform.position = transform.position + Vector3.left * 0.3f;
        GameObject bulletLL = objectManager.MakeObj("BulletBossA");
        bulletLL.transform.position = transform.position + Vector3.left * 0.45f;

        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                
        rigidR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);   

        curPatternCount++;

        if(curPatternCount < maxPatternCount[patternIndex])
        {
            Invoke("FireFoward",2);
        }
        else
        {
            Invoke("Think",3);
        }
    }

    void FireShot()
    {
        for(int index = 0; index < 5; index++)
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyB");
            bullet.transform.position = transform.position;
                
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = player.transform.position - transform.position;
            Vector2 ranVec = new Vector2(Random.Range(-0.5f,0.5f),Random.Range(0f,2f));
            dirVec += ranVec;
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }
        
        curPatternCount++;
        if(curPatternCount < maxPatternCount[patternIndex])
        {
            Invoke("FireShot",3.5f);
        }
        else
        {
            Invoke("Think",3);
        }
    }

    void FireArc()
    {
        Debug.Log("부채모양으로 발사");
        curPatternCount++;
        if(curPatternCount < maxPatternCount[patternIndex])
        {
            Invoke("FireArc",0.15f);
        }
        else
        {
            Invoke("Think",3);
        }
    } 

    void FireAround()
    {
        Debug.Log("원 형태로 전체 공격");
        curPatternCount++;
        if(curPatternCount < maxPatternCount[patternIndex])
        {
            Invoke("FireAround",0.7f);
        }
        else
        {
            Invoke("Think",3);
        }
    }

    void Update()
    {
        if(enemyName == "B")
            return;
        
        Fire();
        Reload();
    }
    void Fire()
    {
        if (curShotDelay < maxShotDelay)
            return;
        
        if(enemyName == "S")
        {
                GameObject bullet = objectManager.MakeObj("BulletEnemyA");
                bullet.transform.position = transform.position;
                
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                Vector3 dirVec = player.transform.position - transform.position;
                rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        }
        else if(enemyName == "L")
        {
                GameObject bulletR = objectManager.MakeObj("BulletEnemyB");
                bulletR.transform.position = transform.position + Vector3.right * 0.3f;
                GameObject bulletL = objectManager.MakeObj("BulletEnemyB");
                bulletL.transform.position = transform.position + Vector3.left * 0.3f;

                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();

                Vector3 dirVecR = player.transform.position - transform.position + Vector3.right*0.3f;
                Vector3 dirVecL = player.transform.position - transform.position + Vector3.left*0.3f;
                
                rigidR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse);
                rigidL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse);
        }

        curShotDelay = 0;
    }
    
    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    public void OnHit(int dmg)
    {
        if(health <= 0)
            return;

        health -= dmg;
        if(enemyName == "B")
        {
            anim.SetTrigger("onHit");
        }
        else
        {
            spriteRenderer.sprite = sprites[1];
            Invoke("RetrunSprite", 0.1f); //시간차로 돌려놓기.
        }
       
        if(health <= 0)
        {
            Player playLogic = player.GetComponent<Player>();
            playLogic.score += enemyScore;

            int ran = enemyName == "B" ? 0 : Random.Range(0, 10);
            
            if(ran < 3){
                //Debug.Log("Not Item");
            }   
            else if(ran < 6){
                GameObject itemCoin = objectManager.MakeObj("ItemCoin");
                itemCoin.transform.position = transform.position;
            }
            else if(ran < 8){
                GameObject itemPower = objectManager.MakeObj("ItemPower");
                itemPower.transform.position = transform.position;   
            }
            else if(ran < 10){
                GameObject itemBoom = objectManager.MakeObj("ItemBoom");
                itemBoom.transform.position = transform.position;
            }

            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
    }

    void RetrunSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "BorderBullet" && enemyName != "B")
        {
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
        else if(other.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);
            other.gameObject.SetActive(false);
        }
    }
}
    
