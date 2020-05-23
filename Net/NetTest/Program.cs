
namespace NetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            InputHandle.Start();

            //TCP.TCPTest.Run();

            UDPTest.Run();

            System.Console.ReadLine();
        }
    }
}
