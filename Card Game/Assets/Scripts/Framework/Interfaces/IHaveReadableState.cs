public interface ICanBeCardViewed
{
    IHaveReadableStats ReadableStats { get; }
    Card AsCard { get; }
}
