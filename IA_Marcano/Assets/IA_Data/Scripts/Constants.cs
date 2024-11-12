using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants
{
    private static PlayerController _player;
    public static PlayerController Player => _player;
    public const float nearNodeDistance = 3;
    public static LayerMask obsMask = LayerMask.GetMask("Wall");
    public static LayerMask nodeMask = LayerMask.GetMask("Node");


    public static void SetPlayer(PlayerController player)
    {
        _player = player;
        //Debug.Log($"Player {_player}");
    }
}
