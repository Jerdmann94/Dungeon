using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectable
{
    public void AddEffect(EffectObj effect);
    public void HandleEffect();
    public void RemoveEffect(EffectObj effect);
}
