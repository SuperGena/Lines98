using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Color color;

    public enum Color
    {
        Red, Yellow, Green, Blue
    }

    Ball(Color color)
    {
        this.color = color;
    }
}
