using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float maxShotDelay;
    public float curShotDelay;
    public ObjectManager objectManager;

    public Vector3 folloewPos;
    public int followDelay;
    public Transform parent;
    public Queue<Vector3> parentPos;


    void Awake() {
        parentPos = new Queue<Vector3>();
    }

    void Update()
    {
        Watch();
        Follow();
        Fire();
        Reload();
    }

    void Watch()
    {
        if(parentPos.Contains(parent.position)){
            return;
        }
        //#.FIFO(First Input First Output)
        parentPos.Enqueue(parent.position); //Input Position
        if(parentPos.Count > followDelay){
            folloewPos = parentPos.Dequeue(); //Output Position
        }
        else if(parentPos.Count < followDelay){
            folloewPos = parent.position;
        }
    }

    void Follow(){
        transform.position = folloewPos;
    }

    void Fire()
    {
        if (!Input.GetButton("Fire1"))
            return;

        if (curShotDelay < maxShotDelay)
            return;

        
        GameObject bullet = objectManager.MakeObj("BulletFollower");
        bullet.transform.position = transform.position;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
       
        curShotDelay = 0;
    }

    void Reload(){
        curShotDelay += Time.deltaTime;
    }
}