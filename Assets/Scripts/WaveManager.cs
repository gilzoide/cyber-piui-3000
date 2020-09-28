﻿using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    public WeaponInfo weaponInfo;
    [System.Serializable]
    public class Waves { public GameObject[] enemies; }
    public Waves[] waves;

    [HideInInspector]
    public Transform train;
    [HideInInspector]
    public UnityEvent endLevel;

    private int enemies;
    private int wave;

    void Awake()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(3);

        enemies = waves[wave].enemies.Length;

        foreach (GameObject enemyPrefab in waves[wave].enemies)
        {
            Vector2 spawnPosition = new Vector2(
                Random.Range(0, 2) == 0 ? 10 : -10,
                Random.Range(-5f, 5f)
            );

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemy.transform.localScale = new Vector3(spawnPosition.x / 10, 1, 1);
            enemy.GetComponent<GetNear>().target = train;
            enemy.GetComponent<GetNear>().onNear.AddListener(() => enemy.GetComponentInChildren<Crosshair>().LookAt(train));
            enemy.GetComponent<GetNear>().onNear.AddListener(() => enemy.GetComponent<RepeatedShooter>().Shoot(weaponInfo));
            enemy.GetComponent<EnergyHolder>().onEnergyEmpty.AddListener(EnemyDied);
        }
    }

    void EnemyDied()
    {
        enemies--;

        if (enemies <= 0)
        {
            if (wave < waves.Length - 1)
            {
                wave++;
                StartCoroutine(SpawnEnemies());
            }
            else
            {
                endLevel.Invoke();
                Destroy(gameObject);
            }
        }
    }
}
