using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TripleBossManager : MonoBehaviour
{
    [SerializeField] private float timeUntilPatternStart = 5f;

    [Serializable]
    public class BossPairings
    {
        public BossPatternEnum boss1;
        public BossPatternEnum boss2;
        [HideInInspector] public int patternIndex;
    }

    public BossPairings[] bossPairings = new BossPairings[12];

    [Header("")]
    [Header("Debug Variables")]

    public List<int> bossPatternIndexes = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
    private int currentBossPatternIndex = -1;

    [Serializable]
    public class BossStatus
    {
        public TripleBoss boss;
        public bool alive = true;
        public bool turnToAttack = false;
        public bool readyToStartBattle = false;
        public List<int> attackOnePatternIndexes = new List<int>(4);
        public List<int> attackTwoPatternIndexes = new List<int>(4);

    }
    public List<BossStatus> bosses = new List<BossStatus>(3);

    public Phase phase;
    public enum Phase
    {
        ThreeBosses,
        TwoBosses,
        OneBoss,
        PlayerWin
    }

    private void Start()
    {
        bosses[0].boss = GetComponentInChildren<MeleeBoss>();
        bosses[1].boss = GetComponentInChildren<GrenadeBoss>();
        bosses[2].boss = GetComponentInChildren<LaserBoss>();
        SetPatternIndexes();
    }

    private void SetPatternIndexes()
    {
        for (int i = 0; i < bossPairings.Length; i++)
        {
            bossPairings[i].patternIndex = i;

            if (bossPairings[i].boss1 == BossPatternEnum.Melee1 || bossPairings[i].boss2 == BossPatternEnum.Melee1)
            {
                bosses[0].attackOnePatternIndexes.Add(bossPairings[i].patternIndex);
            }

            if (bossPairings[i].boss1 == BossPatternEnum.Melee2 || bossPairings[i].boss2 == BossPatternEnum.Melee2)
            {
                bosses[0].attackTwoPatternIndexes.Add(bossPairings[i].patternIndex);
            }

            if (bossPairings[i].boss1 == BossPatternEnum.Grenade1 || bossPairings[i].boss2 == BossPatternEnum.Grenade1)
            {
                bosses[1].attackOnePatternIndexes.Add(bossPairings[i].patternIndex);
            }

            if (bossPairings[i].boss1 == BossPatternEnum.Grenade2 || bossPairings[i].boss2 == BossPatternEnum.Grenade2)
            {
                bosses[1].attackTwoPatternIndexes.Add(bossPairings[i].patternIndex);
            }

            if (bossPairings[i].boss1 == BossPatternEnum.Laser1 || bossPairings[i].boss2 == BossPatternEnum.Laser1)
            {
                bosses[2].attackOnePatternIndexes.Add(bossPairings[i].patternIndex);
            }

            if (bossPairings[i].boss1 == BossPatternEnum.Laser2 || bossPairings[i].boss2 == BossPatternEnum.Laser2)
            {
                bosses[2].attackTwoPatternIndexes.Add(bossPairings[i].patternIndex);
            }
        }
    }

    public void PrepareForBattle()
    {
        GameEvents.Instance.TripleBossMoveToStart();
    }

    public void ReadyToStartBattle(TripleBoss boss)
    {
        bool readyToStart = true;

        for (int i = 0; i < bosses.Count; i++)
        {
            bosses[i].turnToAttack = false;

            if(bosses[i].boss == boss)
            {
                bosses[i].readyToStartBattle = true;
            }

            if(bosses[i].readyToStartBattle == false)
            {
                readyToStart = false;
            }
        }
        if(readyToStart)
        {
            StartCoroutine(StartPattern());
        }
    }

    private IEnumerator StartPattern()
    {
        yield return new WaitForSeconds(timeUntilPatternStart);

        bool mirror = UnityEngine.Random.Range(0, 2) == 0;

        for(int i = 0; i < bosses.Count; i++)
        {
            bosses[i].readyToStartBattle = false;
            switch(phase)
            {
                case Phase.ThreeBosses:
                    SetAttackingBosses(i, mirror);

                    for (int j = 0; j < bosses.Count; j++)
                    {
                        if (!bosses[j].turnToAttack)
                        {
                            bosses[j].boss.state = TripleBoss.State.MovingToStart;
                        }
                    }
                    break;
                case Phase.TwoBosses:
                    currentBossPatternIndex = UnityEngine.Random.Range(1, 3);
                    bosses[i].boss.StartPattern(currentBossPatternIndex, mirror);
                    break;
                case Phase.OneBoss:
                    bosses[i].boss.StartPattern(3, mirror);
                    break;
            }
        }
        if(currentBossPatternIndex + 1 >= bossPatternIndexes.Count)
        {
            currentBossPatternIndex = 0;
        } else
        {
            currentBossPatternIndex++;
        }
    }

    private void SetAttackingBosses(int index, bool mirror)
    {
        for (int i = 0; i < bosses[index].attackOnePatternIndexes.Count; i++)
        {
            if (bosses[index].attackOnePatternIndexes[i] == currentBossPatternIndex)
            {
                bosses[index].turnToAttack = true;
                bosses[index].boss.StartPattern(1, mirror);
                break;
            }
        }

        for (int i = 0; i < bosses[index].attackTwoPatternIndexes.Count; i++)
        {
            if (bosses[index].attackTwoPatternIndexes[i] == currentBossPatternIndex)
            {
                bosses[index].turnToAttack = true;
                bosses[index].boss.StartPattern(2, mirror);
                break;
            }
        }
    }

    public void BossDead(TripleBoss boss)
    {
        for(int i = 0; i < bosses.Count; i++)
        {
            bosses[i].readyToStartBattle = false;
            if (bosses[i].boss == boss)
            {
                bosses[i].alive = false;

                switch(phase)
                {
                    case Phase.ThreeBosses:
                        phase = Phase.TwoBosses;
                        break;

                    case Phase.TwoBosses:
                        phase = Phase.OneBoss;
                        break;

                    case Phase.OneBoss:
                        phase = Phase.PlayerWin;
                        break;
                }
                bosses.RemoveAt(i);
                break;
            }
        }

        PrepareForBattle();
        GameEvents.Instance.TripleBossPatternEnd();

        Debug.Log("Boss Dead " + boss.name);

        if (phase == Phase.PlayerWin)
        {
            Debug.Log("All Bosses Dead");
        }
    }
}