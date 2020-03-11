using System;
using System.Xml;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Net net2 = new Net();
            net2.BuildFormNet(1,2,3,1,2,3,3,4,3,4);
            Net net = new Net(2,1, 10, 1, 10);
            EQuaTion eq = new EQuaTion(net, 10, 5, 1, 1e-15);
            //eq.AddFirst(new List<int> {0, 1, 2, 3, 5, 6, 7, 8});
            eq.BuildMatrix();
            //   eq.TestShit();
            eq.PrintA();
            eq.GaussSeidel();
            for (int i = 0; i < eq.u.Count; i++)
            {
                if (eq.net.nodes[i][2]>=0)
                {
                    Console.WriteLine($"{i} {eq.u[i]:e15} {eq.Y(eq.net.nodes[i][0], eq.net.nodes[i][1]):e15}");
                }
            }
        }
    }
}