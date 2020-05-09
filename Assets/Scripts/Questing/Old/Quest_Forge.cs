////////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2018 Audiokinetic Inc. / All Rights Reserved
//
////////////////////////////////////////////////////////////////////////

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace QuestSystem
{
    public class Quest_Forge : QuestObjectivePreparer
    {
        [Header("During Waves")]
        public List<WaveLines> Waves;

        [Header("Wwizard Info")]
        public WwizardAI Wwizard;
        public Transform WwizardSpot;
        public float WwizardPreperationDuration = 4f;

        [Header("Objective Info")]
        public Transform TheCore;
        public DefaultSpellcraft SpellScript;
        public RuneTrigger Rune;
        public Color CrystalReadyColor;
        public MeshRenderer RuneMat;
        ParticleSystem CrystalParticles;
        public GameObject EnemySpawnVFX;

        #region private variables
        private readonly int magicInterruptHash = Animator.StringToHash("Magic_Interrupted");
        private readonly int canShootMagicHash = Animator.StringToHash("CanShootMagic");
        private readonly int chargingMagicHash = Animator.StringToHash("ChargingMagic");

        private int currentWave = -1;
        private IEnumerator sequenceRoutine;
        private List<Creature> spawnedEnemies = new List<Creature>();
        #endregion

        private void Awake()
        {
            if (SpellScript != null) {
                SpellScript.SetTarget(TheCore.position);
            }
            
            CrystalParticles = RuneMat.gameObject.GetComponent<ParticleSystem>();
        }

        public override void PrepareObjective()
        {
            sequenceRoutine = DefeatTheCoreSequence();
            StartCoroutine(sequenceRoutine);

            PreparationDone();
        }

        public override void ReversePreparations()
        {
            currentWave = -1;

            if (sequenceRoutine != null)
            {
                StopCoroutine(sequenceRoutine);
            }

            for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
            {
                Creature currentEnemy = spawnedEnemies[i];

                if(currentEnemy != null){
                    Destroy(currentEnemy.gameObject);
                }
                spawnedEnemies.RemoveAt(i);
            }

            PreparationReversed();
        }

        IEnumerator DefeatTheCoreSequence()
        {
            //// The core was attacked, and things start to spawn.
            Wwizard.DisableNavMeshAgent();

            Rune.SetHelptextVisibility(false);
            Rune.RuneStatus(false);

            // WAVE 1
            yield return Wave(0);

            // WAVE 2
            yield return Wave(1);

            // WAVE 3
            yield return Wave(2);

            Wwizard.EnableNavMeshAgent();
            SpellScript.DisableMagic();
        }

        IEnumerator Wave(int waveNo)
        {
            Wwizard.transform.rotation = WwizardSpot.rotation;

            currentWave = waveNo;

            // Send dialogue
            if (Waves[waveNo].PreChargeDialogue.Count > 0)
            {
                DialogueManager.Instance.TransferDialogue(Waves[waveNo].PreChargeDialogue, Wwizard.gameObject);
            }

            // Spawn enemies
            yield return new WaitForSeconds(Waves[waveNo].WaitBeforeSpawnDuration);
            for (int i = 0; i < Waves[waveNo].EnemySpawns.Count; i++)
            {
                spawnedEnemies.Add(SpawnEnemy(waveNo, i));
                yield return new WaitForSeconds(0.5f);
            }

            // Wait for Wwizard charging
            yield return StartCoroutine(ChargeSpell(true));

            // Spell is charged, so start the particles and enable the spell-casting
            CrystalParticles.Play();
            Rune.RuneStatus(true);
            Rune.SetHelptextVisibility(true);

            // Send post-charge dialogue
            if (Waves[waveNo].ChargeDoneDialogue.Count > 0)
            {
                DialogueManager.Instance.TransferDialogue(Waves[waveNo].ChargeDoneDialogue, Wwizard.gameObject);
            }

            // Wait until the shield has been destroyed
            yield return new WaitUntil(() => Waves[waveNo].hasBeenDestroyed);
            PlayerManager.Instance.playerAnimator.SetTrigger(magicInterruptHash);
            PlayerManager.Instance.playerAnimator.SetBool(canShootMagicHash, false);
            PlayerManager.Instance.playerAnimator.SetBool(chargingMagicHash, false);
            PlayerManager.Instance.motor.movementPausers.Clear();
            PlayerManager.Instance.attackSystem.AttackPausers.Clear();

            // Spell has been used, so de-charge the rune and stop the particles
            StartCoroutine(ChargeSpell(false));
            CrystalParticles.Stop();
            Rune.RuneStatus(false);
            Rune.SetHelptextVisibility(false);
        }

        IEnumerator ChargeSpell(bool chargeIN)
        {
            if (chargeIN)
            {
                Wwizard.TriggerAnimation_Charge();
                for (float t = 0; t < 1; t += Time.deltaTime / WwizardPreperationDuration)
                {
                    RuneMat.material.SetColor("_EmissionColor", CrystalReadyColor * t);
                    yield return null;
                }
                RuneMat.material.SetColor("_EmissionColor", CrystalReadyColor);
                Wwizard.TriggerAnimation_ChargeEnd();
            }
            else
            {
                for (float t = 1; t > 0; t -= Time.deltaTime / 0.5f)
                {
                    Color newC = Color.black;
                    newC.r = t;
                    newC.b = t;
                    newC.g = t;
                    RuneMat.material.SetColor("_EmissionColor", newC);
                    yield return null;
                }
                RuneMat.material.SetColor("_EmissionColor", Color.black);
            }
        }

        public void NextWave(int c)
        {
            Waves[c].hasBeenDestroyed = true;
            Rune.SetHelptextVisibility(false);
        }

        private Creature SpawnEnemy(int waveNumber, int enemyNumber)
        {
            Vector3 position = Waves[waveNumber].EnemySpawns[enemyNumber].SpawnPosition.transform.position;
            GameObject enemy = Instantiate(Waves[waveNumber].EnemySpawns[enemyNumber].EnemySpawn, position, Quaternion.identity) as GameObject;
            enemy.transform.parent = TheCore.transform;

            // Instantly set the target to the player
            Creature creature = enemy.GetComponent<Creature>();
            creature.SetTarget(PlayerManager.Instance.player, true);

            // Spawn particles
            if (EnemySpawnVFX != null)
            {
                GameObject VFX = Instantiate(EnemySpawnVFX, position, Quaternion.identity) as GameObject;
                Destroy(VFX, 5f);
            }

            return creature;
        }

        public int GetCurrentWave()
        {
            return currentWave;
        }

        [System.Serializable]
        public class Spawn
        {
            public GameObject EnemySpawn;
            public GameObject SpawnPosition;
        }

        [System.Serializable]
        public class WaveLines
        {
            public List<DialogueLine> PreChargeDialogue;
            public List<DialogueLine> ChargeDoneDialogue;
            public float WaitBeforeSpawnDuration = 1f;
            public List<Spawn> EnemySpawns;
            public bool hasBeenDestroyed;
        }

    }
}