using ONE;
using System.Threading.Tasks;

namespace Aquarius.ONE.Test.ConsoleApp.Commands
{
    public interface ICommand
    {
        Task<int> Execute(ClientSDK clientSDK);
    }
}
