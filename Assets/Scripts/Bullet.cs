using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Rigidbody myRigidbody = null;
    [SerializeField] float speed = 0f;
    Vector3 lastPos = Vector3.zero;
    [SerializeField] Transform myBodyTransform = null;
    int myId = 0;
    DamageInfo myDi = default;
    Health myHealth = null;
    [SerializeField] GameObject[] trails = null;
    [SerializeField] GameObject particleVFX = null;
    float currentLifeTime = 0f;
    [SerializeField] float maxLifeTime = 10f;


    public void Shoot(Vector3 targetPos,DamageInfo di,int id,Health health,float power)
    {
        myDi = di;
        myId = id;
        myHealth = health;

        speed = power;

        float totalTime = Vector3.Distance(transform.position, targetPos) / speed;
        float yPower = 9.81f * totalTime * 0.5f;

        myRigidbody.AddRelativeForce(0f,0f,speed, ForceMode.Impulse);
        myRigidbody.AddForce(0f, yPower, 0f,ForceMode.Impulse);
        myRigidbody.useGravity = true;
    }

    
    void Update()
    {
        if(lastPos!= Vector3.zero)
        {
            myBodyTransform.LookAt(lastPos);
        }
        
        myBodyTransform.Rotate(0f, 180f, 0f, Space.Self);
        lastPos = transform.position;

        currentLifeTime += Time.deltaTime;
        if(currentLifeTime > maxLifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetInstanceID() == myId) return;  // 如果是自己就return

        Health otherHealth = collision.gameObject.GetComponent<Health>();
        if (otherHealth != null && !otherHealth.IsDead() && myHealth.IsMyEnemy(otherHealth)) //如果有生命,還沒死,並且是我的敵人
        {
            otherHealth.TakeDamage(myDi, DamageType.瞬間傷害);
        }


        foreach (GameObject trail in trails)
        {
            trail.transform.parent = null;
            Destroy(trail, 3f);
        }
        Instantiate(particleVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }



}
