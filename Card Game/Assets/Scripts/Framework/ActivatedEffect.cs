public class ActivatedEffect
{
    public string Name { get; private set; }
    public EmptyHandler Effect { get; private set; }

    public ActivatedEffect(string name, EmptyHandler effect)
    {
        Name = name;
        Effect = effect;
    }
}
