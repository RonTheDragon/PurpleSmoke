using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : CharacterDeath , IEnemyComponent
{
    //private EnemyWalk _enemyWalk;
    private EnemyKnockout _enemyKnockout;
    private EnemyAnimations _enemyAnimations;
    [SerializeField] private float _despawnTime = 10;
    private PickupPooler _pickupPooler;
    [SerializeField] private List<Loot> _loot;
    public void InitializeEnemyComponent(EnemyComponentRefrences enemyComponents)
    {
        //_enemyWalk = enemyComponents.GetEnemyWalk;
        _enemyKnockout = enemyComponents.GetEnemyKnockout;
        _enemyAnimations = enemyComponents.GetEnemyAnimations;
        _pickupPooler = GameManager.Instance.GetPickupPooler;
    }

    public override void Die()
    {
        _enemyAnimations.PlayAnimation("Stumble");
        _enemyKnockout.StunCharacter();
        DropLoot();
        Invoke(nameof(DespawnBody), _despawnTime);
    }

    protected void DropLoot()
    {
        foreach(Loot loot in _loot)
        {
            int amount = 0;
            float n = Random.Range(0, 100f);
            float growingNumber = 0;
            foreach (Loot.DropChance dropChance in loot.DropChances)
            {
                growingNumber += dropChance.PrecentageChance; // Increment growingNumber by chance

                if (n <= growingNumber) // If random number falls within this range
                {
                    amount = dropChance.Amount; // Set the amount to drop
                    break; // Exit the loop since we've found the chance
                }
            }

            if (amount <= 0) return;
            Pickup p = _pickupPooler.CreateOrSpawnFromPool(loot.LootTag,transform.position,Quaternion.identity);
            if (p is ItemPickUp)
            {
                ItemPickUp p2 = (ItemPickUp)p;
                p2.SetAmount(amount);
            }
        }
    }

    public override void Revive()
    {
        CancelInvoke(nameof(DespawnBody));
        _enemyKnockout.UnStunCharacter();
    }

    private void DespawnBody()
    {
        _enemyAnimations.AnimationRebind();
        gameObject.SetActive(false);
    }

    [System.Serializable]
    public class Loot
    {
        public string LootTag;
        public List<DropChance> DropChances;

        [System.Serializable]
        public class DropChance
        {
            public int Amount;
            public float PrecentageChance;
        }
    }
}
