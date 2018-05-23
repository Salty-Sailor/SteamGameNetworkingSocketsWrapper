using System.Threading.Tasks;
using System.Threading;
using SteamWrapper.Test;

namespace SteamWrapper
{
    public static class Program
    {
        static void Main()
        {
            Task.Run( () =>{
                TestSteam.TestServer();
            });
            
            Thread.Sleep( 100 );

            TestSteam.TestClient();
        }
    }
}
