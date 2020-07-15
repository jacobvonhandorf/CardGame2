using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrapper<T>
{
    public T value;

    public Wrapper(T value)
    {
        this.value = value;
    }
}
