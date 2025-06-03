using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A serializable container for “all the enemies in Cart #X.”
/// We will simply hold references to pre-placed enemy GameObjects and
/// enable them when the player first enters the cart.
/// </summary>
[Serializable]
public class CartData
{
    [Tooltip("A friendly name for debugging (e.g. “Cart 0”).")]
    public string cartName;

    [Tooltip("All existing enemy GameObjects under this cart (initially disabled).")]
    public List<GameObject> enemies = new List<GameObject>();

    [HideInInspector]
    public bool hasActivated = false;
}