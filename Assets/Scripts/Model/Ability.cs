using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public interface IAbility
{
    string Name { get; }
    int Cost { get; }
    int Cooldown { get; }
    int CurrentCooldown { get; set; }
    bool IsOnCooldown { get; }
    void Execute(List<Character> targets);
    void ResetCooldown();
}
