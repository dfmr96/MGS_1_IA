using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    public const float nearNodeDistance = 3;
    public static LayerMask obsMask = LayerMask.GetMask("Wall");
    public static LayerMask nodeMask = LayerMask.GetMask("Node");
}
