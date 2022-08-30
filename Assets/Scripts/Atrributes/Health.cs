using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Core;
using RPG.Stats;
using System;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float regenerateFraction = 0.7f;
        [SerializeField] UnityEvent<float> takeDamage;

        // [System.Serializable]
        // public class TakeDamageEvent : UnityEvent<float>
        // {
        // }

        LazyValue<float> healthPoints;

        bool isDead = false;

        private void Awake() 
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Start() 
        {
            healthPoints.ForceInit();
        }

        private void OnEnable() 
        {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        private void OnDisable() 
        {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            print($"{gameObject.name} took damage {damage}");

            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            print(healthPoints); 

            if (healthPoints.value == 0)
            {
                Die();
                GainExperience(instigator);
            }
            else takeDamage.Invoke(damage);
        }

        public float GetHealthPoints()
        {
            return healthPoints.value;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPercentage()
        {
            return 100 * (healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        private void Die()
        {
            if (isDead) return;
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void GainExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;
            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperiencePoints));
        }

        private void RegenerateHealth()
        {
            float regeneratedHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * regenerateFraction;
            healthPoints.value = Mathf.Max(healthPoints.value, regeneratedHealthPoints);
        }

        public object CaptureState()
        {
            return healthPoints;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;

            if (healthPoints.value == 0)
            {
                Die();
            }
        }
    }
}
