using UnityEngine;
using System.Collections;

public interface StatChangeHandler<T>
{
    void OnValueUpdated(T value);
}
