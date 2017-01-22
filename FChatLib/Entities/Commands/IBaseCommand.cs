namespace FChatLib.Entities.Commands
{
    public interface IBaseCommand
    {
        string Data { get; }
        string Type { get; set; }
    }
}