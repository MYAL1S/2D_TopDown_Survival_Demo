using System;
using UnityEngine;

[DisallowMultipleComponent]
public class StatusEffectTickSystem : MonoBehaviour
{
    public static event Action<float> OnGlobalTick;

    private static StatusEffectTickSystem instance;
    public static StatusEffectTickSystem Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("StatusEffectTickSystem");
                instance = obj.AddComponent<StatusEffectTickSystem>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    [SerializeField]
    [Min(0.01f)]
    private float tickInterval = 0.25f;

    private float elapsedTime;


    private void Update()
    {
        elapsedTime += Time.deltaTime;
        float interval = Mathf.Max(0.01f, tickInterval);
        while (elapsedTime >= interval)
        {
            elapsedTime -= interval;
            OnGlobalTick?.Invoke(interval);
        }
    }
}
