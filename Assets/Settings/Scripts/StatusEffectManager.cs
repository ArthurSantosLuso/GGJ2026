using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatusEffectManager : MonoBehaviour
{
    private class ActiveEffect
    {
        public Coroutine coroutine;
    }

    private Dictionary<Enemy, Dictionary<StatusType, ActiveEffect>> activeEffects
    = new Dictionary<Enemy, Dictionary<StatusType, ActiveEffect>>();


    public void ApplyFire(Enemy enemy, float dps, float duration)
    {
        StartEffect(enemy, StatusType.Fire,
        FireCoroutine(enemy, dps, duration));
    }

    public void ApplyPoison(Enemy enemy, float dps, float duration)
    {
        StartEffect(enemy, StatusType.Poison,
        PoisonCoroutine(enemy, dps, duration));
    }

    public void ApplySlow(Enemy enemy, float slowMultiplier, float duration)
    {
        StartEffect(enemy, StatusType.Slow,
        SlowCoroutine(enemy, slowMultiplier, duration));
    }


    private void StartEffect(Enemy enemy, StatusType type, IEnumerator routine)
    {
        if (!activeEffects.ContainsKey(enemy))
            activeEffects[enemy] = new Dictionary<StatusType, ActiveEffect>();

        // Refresh effect if already active
        if (activeEffects[enemy].ContainsKey(type))
        {
            StopCoroutine(activeEffects[enemy][type].coroutine);
        }

        Coroutine c = StartCoroutine(routine);

        activeEffects[enemy][type] = new ActiveEffect
        {
            coroutine = c
        };
    }

    private void EndEffect(Enemy enemy, StatusType type)
    {
        if (!activeEffects.ContainsKey(enemy)) return;

        activeEffects[enemy].Remove(type);

        if (activeEffects[enemy].Count == 0)
            activeEffects.Remove(enemy);
    }


    private IEnumerator FireCoroutine(Enemy enemy, float dps, float duration)
    {
        float t = 0f;

        while (t < duration && enemy != null)
        {
            enemy.TakeDamage(dps * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }

        EndEffect(enemy, StatusType.Fire);
    }

    private IEnumerator PoisonCoroutine(Enemy enemy, float dps, float duration)
    {
        float t = 0f;

        while (t < duration && enemy != null)
        {
            enemy.TakeDamage(dps * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }

        EndEffect(enemy, StatusType.Poison);
    }

    private IEnumerator SlowCoroutine(Enemy enemy, float slowMultiplier, float duration)
    {
        float originalSpeed = enemy.moveSpeed;
        enemy.moveSpeed *= slowMultiplier;

        yield return new WaitForSeconds(duration);

        if (enemy != null)
            enemy.moveSpeed = originalSpeed;

        EndEffect(enemy, StatusType.Slow);
    }
}