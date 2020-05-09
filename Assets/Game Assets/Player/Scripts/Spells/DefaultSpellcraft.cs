////////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2018 Audiokinetic Inc. / All Rights Reserved
//
////////////////////////////////////////////////////////////////////////

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DefaultSpellcraft : MonoBehaviour
{
    [System.Serializable]
    public class ChargeInfo
    {
        public List<Spell> OnCharge;
        public float ChargeAmount = 0f;
    }

    [System.Serializable]
    public class ReleaseInfo
    {
        public List<Spell> OnRelease;
    }

    [System.Serializable]
    public class SpellDesigns
    {
        public string spellName;
        public ChargeInfo Charge;
        public ReleaseInfo Release;
        public float MaxDamage = 10f;
        public float ImpactVelocity = 0f;
        public bool IsAvailable;
    }
    public List<SpellDesigns> Spellcraft;

    public int SpellSelect = 0;


    [Header("WWISE")]
    public AK.Wwise.Event SpellCast = new AK.Wwise.Event();
    public AK.Wwise.Event SpellChargeStart = new AK.Wwise.Event();
    public AK.Wwise.Event SpellChargeStop = new AK.Wwise.Event();
    public AK.Wwise.RTPC SpellChargeLevel = new AK.Wwise.RTPC();

    #region private variables
    private Quaternion startRotation;
    private Vector3 targetPosition;
    //Chached animator hashes
    private readonly int canShootMagicHash = Animator.StringToHash("CanShootMagic");
    private readonly int shootMagicHash = Animator.StringToHash("ShootMagic");
    private readonly int chargeMagicHash = Animator.StringToHash("ChargeMagic");
    private readonly int chargingMagicHash = Animator.StringToHash("ChargingMagic");
    #endregion

    public void EnableMagic()
    {

        SpellChargeLevel.SetGlobalValue(0f);
        SpellChargeStart.Post(gameObject);

        InputManager.OnUseDown += OnCharge;
        InputManager.OnUseUp += OffCharge;
    }

    public void DisableMagic()
    {
        SpellChargeStop.Post(gameObject);
        Spellcraft[SpellSelect].Charge.OnCharge[0].Deactivate();
        InputManager.OnUseDown -= OnCharge;
        InputManager.OnUseUp -= OffCharge;
    }

    private void OnDestroy()
    {
        DisableMagic();
    }

    void OnCharge()
    {
        PlayerManager.Instance.playerAnimator.SetBool(canShootMagicHash, false);
        // Activate charges
        if (Spellcraft[SpellSelect].IsAvailable)
        {
            PlayerManager.Instance.PauseMovement(this.gameObject);
            PlayerManager.Instance.PauseAttacking(this.gameObject);
            PlayerManager.Instance.playerAnimator.ResetTrigger(shootMagicHash);
            PlayerManager.Instance.playerAnimator.SetTrigger(chargeMagicHash);
            PlayerManager.Instance.playerAnimator.SetBool(chargingMagicHash, true);
            PlayerManager.Instance.playerAnimator.SetBool(canShootMagicHash, false);
            for (int s = 0; s < Spellcraft[SpellSelect].Charge.OnCharge.Count; s++)
            {
                Spellcraft[SpellSelect].Charge.OnCharge[s].Activate();
            }

            // SPELL SOUND
            SpellChargeStart.Post(gameObject);
            startRotation = transform.rotation;
        }
    }

    void OnCharging(float playbacktime)
    {
        Vector3 targetDir = targetPosition - transform.position;
        Quaternion LRotation = Quaternion.LookRotation(targetDir);

        float s = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f).Evaluate(playbacktime);
        PlayerManager.Instance.player.transform.rotation = Quaternion.Lerp(startRotation, LRotation, s);
    }

    public void SetTarget(Vector3 position)
    {
        targetPosition = position;
    }

    void FixedUpdate()
    {
        if (PlayerManager.Instance != null && Spellcraft.Count > 0)
        {
            if (Spellcraft[SpellSelect].IsAvailable && PlayerManager.Instance.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("ChargeMagic_wEvents") && !PlayerManager.Instance.playerAnimator.GetBool("CanShootMagic"))
            {
                AnimatorStateInfo currentState = PlayerManager.Instance.playerAnimator.GetCurrentAnimatorStateInfo(0);
                float playbackTime = currentState.normalizedTime % 1;
                UpdateCharge(playbackTime);
                OnCharging(playbackTime);
            }
            else if (Spellcraft[SpellSelect].IsAvailable && PlayerManager.Instance.playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("ReadyToShootMagic"))
            {
                UpdateCharge(1f);
                if (!updatedSpellReady)
                {
                    EventManager.SpellReady();
                    updatedSpellReady = true;
                }
                updatedSpellNotReady = false;
            }
            else
            {
                UpdateCharge(0f);
                if (!updatedSpellNotReady)
                {
                    EventManager.SpellNotReady();
                    updatedSpellNotReady = true;
                }
                updatedSpellReady = false;
            }
        }

    }

    bool updatedSpellNotReady;
    bool updatedSpellReady;

    public void UpdateCharge(float charge)
    {
        Spellcraft[SpellSelect].Charge.ChargeAmount = charge;
        SpellChargeLevel.SetGlobalValue(Spellcraft[SpellSelect].Charge.ChargeAmount * 100);
    }

    void OffCharge()
    {
        PlayerManager.Instance.ResumeAttacking(this.gameObject);
        PlayerManager.Instance.ResumeMovement(this.gameObject);
        if (Spellcraft[SpellSelect].IsAvailable)
        {
            PlayerManager.Instance.playerAnimator.ResetTrigger(chargeMagicHash);
            PlayerManager.Instance.playerAnimator.SetTrigger(shootMagicHash);
            PlayerManager.Instance.playerAnimator.SetBool(chargingMagicHash, false);

            if (PlayerManager.Instance.playerAnimator.GetBool(canShootMagicHash))
            {
                SpellChargeStop.Post(gameObject);
                // Activate Spells
                PlayerManager.Instance.playerAnimator.SetBool(canShootMagicHash, false);
                for (int R = 0; R < Spellcraft[SpellSelect].Release.OnRelease.Count; R++)
                {
                    Spellcraft[SpellSelect].Release.OnRelease[R].ChargeValue = Spellcraft[SpellSelect].Charge.OnCharge[0].ChargeValue;
                    Spellcraft[SpellSelect].Release.OnRelease[R].ImpactVel = Spellcraft[SpellSelect].ImpactVelocity;
                    Spellcraft[SpellSelect].Release.OnRelease[R].Damage = Spellcraft[SpellSelect].MaxDamage;
                    Spellcraft[SpellSelect].Release.OnRelease[R].Activate();

                    // SPELL SOUND
                    SpellChargeLevel.SetGlobalValue(Spellcraft[SpellSelect].Charge.ChargeAmount * 100);
                    SpellCast.Post(this.gameObject);
                }
            }
        }

        // Deactivate Charges
        for (int s = 0; s < Spellcraft[SpellSelect].Charge.OnCharge.Count; s++)
        {
            Spellcraft[SpellSelect].Charge.OnCharge[s].Deactivate();
        }
    }

}
