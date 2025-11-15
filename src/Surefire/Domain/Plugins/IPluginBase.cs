public interface IPluginBase
{
    string Name { get; }
    bool IsActive { get; set; }
}