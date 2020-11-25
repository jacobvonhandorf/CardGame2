using UnityEngine;

[CreateAssetMenu(fileName = "New Counter Type", menuName = "Counter Type")]
public class CounterType : ScriptableObject
{
    public int test;
    [SerializeField] public Color borderColor { get; }
}
