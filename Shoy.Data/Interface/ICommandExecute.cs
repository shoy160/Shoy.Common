namespace Shoy.Data
{
    public interface ICommandExecute
    {
        int Execute(IConnectionContext cc);
    }
}
