namespace Shoy.Core.Domain.Entities
{
    public interface IHasStatus : IHasStatus<byte> { }
    public interface IHasStatus<TStatusType>
    {
        TStatusType Status { get; set; }
    }
}
