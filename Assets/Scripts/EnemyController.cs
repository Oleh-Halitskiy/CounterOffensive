using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private CharacterController characterController;
    private Vector3 moveVector;
    private float healthPoints;
    public bool isDead;
    // Start is called before the first frame update
    void Start()
    {
        healthPoints = 100;
        characterController = GetComponent<CharacterController>();
        moveVector = new Vector3(Random.Range(1, 5), 0, Random.Range(1, 5));
    }
    public void TakeDamage(float damage)
    {
        healthPoints -= damage;
    }
    private void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        characterController.Move(moveVector * Time.deltaTime);
        if (healthPoints <= 0)
        {
            Die();
        }

    }
}
