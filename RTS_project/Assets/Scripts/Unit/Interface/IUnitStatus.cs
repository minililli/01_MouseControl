using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitStatus
{
    float HP { get; set; }
    float MP { get; set; }
    float Power { get; set; }
    float AttackSpeed { get; set; }
    float Defence { get; set; }
    float MoveSpeed { get; set; }
}
