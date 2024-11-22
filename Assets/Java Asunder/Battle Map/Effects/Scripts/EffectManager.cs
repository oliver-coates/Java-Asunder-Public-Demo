using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Effects
{


public class EffectManager : MonoBehaviour
{
    private static EffectManager _instance;

    private Dictionary<EffectType, EffectPool> _poolDictionary;

    private void Awake()
    {
        _instance = this;

        _poolDictionary = new Dictionary<EffectType, EffectPool>();
    }

    public static void SpawnEffect(EffectType effect, Vector3 position)
    {
        _instance.InternalSpawnEffect(effect, position);
    }
    private void InternalSpawnEffect(EffectType effectType, Vector3 position)
    {
        // Find effect set that matches effect
        if (_poolDictionary.ContainsKey(effectType) == false)
        {
            _poolDictionary.Add(effectType, new EffectPool(this, effectType));
        }

        EffectPool pool = _poolDictionary[effectType];

        EffectInstance retrievedEffect = pool.TakeFromPool();

        retrievedEffect.transform.position = position;
    }

    
}

public class EffectPool
{
    private EffectManager _manager;
    private EffectType _type;

    private Dictionary<EffectInstance, bool> _effectPool;

    public EffectPool(EffectManager manager, EffectType type)
    {
        _manager = manager;
        _type = type;
        
        _effectPool = new Dictionary<EffectInstance, bool>();
    }

    public EffectInstance TakeFromPool()
    {
        // Find object from pool or instaniate it
        foreach (EffectInstance effect in _effectPool.Keys)
        {
            if (_effectPool[effect])
            {
                _effectPool[effect] = false;
                effect.gameObject.SetActive(true);
                effect.TakenFromPool();
                return effect;
            }
        }
        
        // No available ones found, take a new one from the pool:
        GameObject newObj = Object.Instantiate(_type.prefab, _manager.transform);

        // Initialise the new effect so it knows it can let this pool know once it has finished
        EffectInstance newEffect = newObj.GetComponent<EffectInstance>();
        newEffect.Initialise(this);

        // Add to the pool
        _effectPool.Add(newEffect, false);

        return newEffect;
    }

    public void ReturnToPool(EffectInstance toReturn)
    {
        _effectPool[toReturn] = true;

        toReturn.gameObject.SetActive(false);
    }
    


}

}