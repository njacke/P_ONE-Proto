using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class G05_ItemEffect
{
    public EffectCat EffectCategory { get; protected set; }
    public string EffectName { get; protected set; }
    public int EffectValue { get; protected set; }
    public string EffectValueText { get; protected set; }


    public enum EffectCat {
        None,
        PreRoll,
        PostRoll,
    }

    public abstract void ResolveEffect();

}
