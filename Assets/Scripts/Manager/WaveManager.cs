﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class WaveGroup : IComparable
{
    public Monster[] monsters;
    public int cost;
    public int CompareTo(object obj)
    {
        WaveGroup other = (WaveGroup)obj;
        return this.cost.CompareTo(other.cost);
    }
}
public class WaveManager : MonoBehaviour
{

    public static WaveManager Instance;
    protected bool isSpawning = false;
    [SerializeField]
    protected List<WaveGroup> groups = new List<WaveGroup>();
    [SerializeField]
    protected int bossWaveNumbers = 10;
    [SerializeField]
    protected WaveGroup bossGroup;
    [SerializeField]
    private int maxCost = 30;
    [SerializeField]
    private float timeBetweenEachMonsterSpawn = .5f, timeBetweenEachGroupSpawn = 2f;
    [SerializeField]
    private List<Monster> monstersInTheScene = new List<Monster>();
    protected void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("There should only be one instance of WaveManager running");
            Destroy(this);
        }
    }

    public bool HasMonster
    {
        get
        {
            return monstersInTheScene.Count > 0;
        }
    }

    protected void Start()
    {
        groups.Sort();
    }

    protected void Update()
    {
        monstersInTheScene.RemoveAll(Monster => Monster == null || !Monster.gameObject.activeSelf);
    }

    public Monster GetClosestMonster(Vector3 position)
    {
        float closestDistance = 999999;
        Monster monster = null;
        foreach (Monster mob in monstersInTheScene)
        {
            float dist = Vector3.Distance(mob.transform.position, position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                monster = mob;
            }
        }

        return monster;
    }

    public void SpawnWave(int waveNumber)//waveNumber = cost
    {
        int cost = waveNumber;
        int level = 1;
        if (cost > maxCost)
        {
            level += cost / maxCost;
            cost /= level;
        }

        List<WaveGroup> spawnGroups = new List<WaveGroup>();
        int maximumIndex = -1;
        foreach (WaveGroup group in groups)
        {
            if (group.cost <= waveNumber)
                maximumIndex++;
        }

        while (cost > 0)
        {
            int randomGroupNumber = UnityEngine.Random.Range(0, maximumIndex + 1);
            spawnGroups.Add(groups[randomGroupNumber]);
            cost -= groups[randomGroupNumber].cost;
        }
        if (waveNumber % bossWaveNumbers == 0)
            spawnGroups.Add(bossGroup);
        if (isSpawning)
            StopAllCoroutines();
        StartCoroutine(StartSpawn(spawnGroups, level));
    }

    protected IEnumerator StartSpawn(List<WaveGroup> groups, int level)
    {
        isSpawning = true;
        for (int i = 0; i < groups.Count; i++)
        {

            for (int j = 0; j < groups[i].monsters.Length; j++)
            {
                Monster monster = Instantiate(groups[i].monsters[j], TownManager.Instance.CurrentTown.MonsterPoint.Position, Quaternion.identity).GetComponent<Monster>();
                monstersInTheScene.Add(monster);
                monster.Data.GetJob(JobType.COMBAT).SetLevel(level);
				yield return new WaitForSeconds(timeBetweenEachMonsterSpawn);
            }

            yield return new WaitForSeconds(timeBetweenEachGroupSpawn);
        }
        isSpawning = false;

    }

}