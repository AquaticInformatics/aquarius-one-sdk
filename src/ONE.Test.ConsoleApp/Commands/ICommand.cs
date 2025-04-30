using System.Threading.Tasks;

namespace ONE.Test.ConsoleApp.Commands
{
    public interface ICommand
    {
        Task<int> Execute(ClientSDK.OneApi clientSdk);
    }
}
