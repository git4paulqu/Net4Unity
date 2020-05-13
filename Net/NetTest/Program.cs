
namespace NetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            InputHandle.Start();
            //TCP.TCPTest.Run();

            UDPTest.Run();

            //System.Threading.Thread.Sleep(100);

            System.Console.ReadLine();
        }
    }
}
