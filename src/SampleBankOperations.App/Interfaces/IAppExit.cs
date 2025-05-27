namespace SampleBankOperations.App.Interfaces
{
    public interface IAppExit
    {
        void Exit(int code);
    }

    public class DefaultAppExit : IAppExit
    {
        public void Exit(int code) => System.Environment.Exit(code);
    }
}
