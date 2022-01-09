using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private CharacterController characterController;
    private Vector3 moveVector;
    private float healthPoints;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 15f);
        healthPoints = 100;
        characterController = GetComponent<CharacterController>();
        moveVector = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
    }
    public void TakeDamage(float damage)
    {
        healthPoints -= damage;
    }
    private void Die()
    {
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
