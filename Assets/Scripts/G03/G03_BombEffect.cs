using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class G03_BombEffect : MonoBehaviour
{
    [SerializeField] private string _effectName = "Default Name";
    [SerializeField] private bool _isOneTime = false;
    [SerializeField] private float _effectLifetime = 1f;
    [SerializeField] private Color _effectColor;

    public string GetEffectName { get { return _effectName; } }
    public bool GetIsOneTime { get { return _isOneTime; } }
    public float GetEffectLifetime { get { return _effectLifetime; } }
    public Color GetEffectColor { get { return _effectColor; } }

    public abstract bool SetEffectPower (int power);
    public abstract bool ApplyEffect(G03_NPC npc);
    public abstract bool RemoveEffect(G03_NPC npc); // change to interface since all can't be removed

}
