using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface HeroPower
{
    void activate(Player controller);
    bool canBeActivatedCheck(Player controller);
    string getEffectText();
}
