namespace BDMultiTool.Engines
{
    public interface IEngine
    {
        void Start();
        void Stop();

        bool Running { get; }
    }
}