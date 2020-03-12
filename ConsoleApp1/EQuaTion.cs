using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class EQuaTion
    {
        private List<double> dMid, dTop, dBot, dSecondBot, dSecondTop, b;
        public Net net;
        private int shift;
        double lambda, gamma, w, residual = Double.PositiveInfinity, eps;
        public List<double> u;
        private int n;
        int iterations = 0, maxIterations = 10000;

        double Tetta(List<double> node, int field)
        {
            return 2 * node[1];
        }
        double Ug(List<double> node, int field)
        {
            return node[1]*node[1];
        }

        public double Y(double x, double y)
        {
            return y*y;
        }

        double function(List<double> node)
        {
            double x = node[0];
            double y = node[1];
            return -2+gamma*y*y;
        }

        public void AddFirst()
        {
            foreach (var node in  net.firstCondi)
            {
                dMid[node] = 1;
                b[node] = Ug(net.nodes[node],1);
            }
        }

        public void AddSecond()
        {
            double h;
            int node;
            foreach (var condi in net.secondCondi)
            {
                switch (condi[2])
                {
                    case (int)Direction.Top:
                        node = condi[0];
                        h = net.nodes[node][1]-net.nodes[node-shift][1];
                        dMid[node] = 1 / h;
                        dSecondBot[node - shift] = -1 / h;
                        b[node] = Tetta(net.nodes[node],condi[1]);
                        break;
                }
            }
        }

        public EQuaTion(Net net, double lambda, double gamma, double w, double eps)
        {
            this.net = net;
            shift = net.kx;
            this.lambda = lambda;
            this.gamma = gamma;
            this.w = w;
            this.eps = eps;
            n = net.nodes.Count;
            dMid = new List<double>(new double[n]);
            dTop = new List<double>(new double[n - 1]);
            dBot = new List<double>(new double [n - 1]);
            dSecondBot = new List<double>(new double[n - net.kx]);
            dSecondTop = new List<double>(new double[n - net.kx]);
            b = new List<double>(new double[n]);
            u = new List<double>(new double[n]);
        }


        double Step()
        {
            for (int i = 0; i < n; i++)
            {
                double sum = Sum(i);
                u[i] = u[i] + w / dMid[i] * (b[i] - sum);
            }
            double res = Res(), norm = Norm();
            residual = Res() / Norm();
            return residual;
        }

        double Norm()
        {
            double result = 0;
            for (int i = 0; i < n; i++)
            {
                result += b[i] * b[i];
            }

            return result;
        }

        double Sum(int i)
        {
            double sum = 0;
            int shift = net.kx;
            sum += dMid[i] * u[i];
            if (i < n - 1) sum += dTop[i] * u[i + 1];
            if (i > 0) sum += dBot[i - 1] * u[i - 1];
            if (i < n - shift) sum += dSecondTop[i] * u[i + shift];
            if (i >= shift) sum += dSecondBot[i - shift] * u[i - shift];
            return sum;
        }

        public void PrintA()
        {
            for (int i = 0; i < net.n; i++)
            {
                PrintStr(i);
            }
        }
        void PrintStr(int i)
        {
            int shift = net.kx;
            for (int j = 0; j < net.n; j++)
            {
                if (j == i - shift)
                {
                    Console.Write($"{dSecondBot[i - shift]:e4} ");
                    continue;
                }
                if (j == i + shift)
                {
                    Console.Write($"{dSecondTop[i]:e4} ");
                    continue;
                }
                if (j == i - 1)
                {
                    Console.Write($"{dBot[i-1]:e4} ");
                    continue;
                }
                if (j == i + 1)
                {
                    Console.Write($"{dTop[i]:e4} ");
                    continue;
                }
                if (j == i)
                {
                    Console.Write($"{dMid[i]:e4} ");
                    continue;
                }

                Console.Write($"{0:e4} ");
            }
            Console.WriteLine();
        }

        double Res()
        {
            double result = 0;
            for (int i = 0; i < n; i++)
            {
                double sum1 = Sum(i);
                double sum = b[i] - sum1;
                result += sum * sum;
            }

            return result;
        }

        public void GaussSeidel()
        {
            while (residual >= eps*eps && iterations < maxIterations)
            {
                Step();
                iterations += 1;
            }
        }

        // public void BuildMatrixEz()
        // {
        //     int i = 0;
        //     int shift = net.n+1;
        //     foreach (var item in net.nodes)
        //     {
        //         if (!firstCondi.Contains(i))
        //         {
        //             dMid.Add(-2.0 / (net.hx * net.hx) - 2.0 / (net.hy * net.hy) + gamma);
        //             if (i < n - 1) dTop.Add(1.0 / net.hx / net.hx);
        //             if (i > 0) dBot.Add(1.0 / net.hx / net.hx);
        //             if (i < n - shift) dSecondTop.Add(1.0 / net.hy / net.hy);
        //             if (i >= shift) dSecondBot.Add(1.0 / net.hy / net.hy);
        //             b.Add(function(item.Key, item.Value));
        //         }
        //         else
        //         {
        //             dMid.Add(1);
        //             if (i < n - 1) dTop.Add(0);
        //             if (i > 0) dBot.Add(0);
        //             if (i < n - shift) dSecondTop.Add(0);
        //             if (i >= shift) dSecondBot.Add(0);
        //             b.Add(Ug(item.Key, item.Value));
        //         }
        //
        //             i++;
        //     }
        //}

        public void BuildMatrix()
        {
            for (int i = 0; i < net.ky; i++)
            {
                for (int j = 0; j < net.kx; j++)
                {
                    int k = j + i * net.kx;
                    if (net.nodes[k][2]<0||net.nodes[k][3]==0)
                    {
                        dMid[k] = 1;
                        continue;
                    }
                    double hxNext = net.nodes[k + 1][0] - net.nodes[k][0];
                    double hxPrev = net.nodes[k][0] - net.nodes[k - 1][0];
                    double hyNext = net.nodes[k + net.kx][1] - net.nodes[k][1];
                    double hyPrev = net.nodes[k][1] - net.nodes[k - net.kx][1];
                    int shift = net.kx;
                    dMid[k] = 2.0 / (hxNext * hxPrev) + 2.0 / (hyNext * hyPrev)+gamma;
                    if (k < n - 1) dTop[k] = -2.0 / (hxNext * (hxNext + hxPrev));
                    if (k > 0) dBot[k - 1] = -2.0 / (hxPrev * (hxNext + hxPrev));
                    if (k < n - shift) dSecondTop[k] = -2.0 / (hyNext * (hyNext + hyPrev));
                    if (k >= shift) dSecondBot[k - shift] = -2.0 / (hyPrev * (hyNext + hyPrev));
                    b[k] = function(net.nodes[k]);
                }
            }
           // PrintA();
            Console.WriteLine();
            AddFirst();
            PrintA();
            Console.WriteLine();
           // AddSecond();
           //PrintA();
            Console.WriteLine();

        }

        public void TestShit()
        {
            for (int i = 0; i <= 8; i++)
                if (i != 4)
                {
                    dMid[i] = 1;
                    b[i] = 10;
                }
        }
    }
}