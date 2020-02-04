using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListChangeController
{
    public static ListChangeController Instance;

    public ListChangeController()
    {
        Instance = this;
    }

    public event Action IsWalkableChengedToTrue;
    public event Action IsWalkableChengedToFalse;
    public void AddIsWalkableToList()
    {
        if (IsWalkableChengedToTrue != null)
        {
            IsWalkableChengedToTrue();
        }
    }
    public void RemoveIsWalkableFromList()
    {
        if (IsWalkableChengedToFalse != null)
        {
            IsWalkableChengedToFalse();
        }
    }
}
