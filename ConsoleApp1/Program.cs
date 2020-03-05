using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Matrix
    {
        
    }

    class Net
    {
        public List<KeyValuePair<double,double>> nodes =new List<KeyValuePair<double, double>>();
        public double hx,hy;
        public int n;
        
        public Net(int n,double xmin, double xmax, double ymin, double ymax)
        {
            this.n = n;
            hx = (xmax - xmin) / n;
            hy = (ymax - ymin) / n;
            double x = xmin, y = ymin;
            for (int j = 0; j <= n; j++)
            {
                for (int i = 0; i <= n; i++)
                {
                    nodes.Add(new KeyValuePair<double, double>(x, y));
                    x += hx;
                }
                x = xmin;
                y += hy;
            }
        }
    }

    class EQuaTion
    {
        private List<double> dMid, dTop, dBot, dSecondBot, dSecondTop, b;
        public Net net;
        double lambda, gamma, w, residual = Double.PositiveInfinity, eps;
        public List<double> u;
        private int n;
        int iterations = 0, maxIterations = 10000;

        public EQuaTion(Net net, double lambda, double gamma, double w, double eps)
        {
            this.net = net;
            this.lambda = lambda;
            this.gamma = gamma;
            this.w = w;
            this.eps = eps;
            n = net.nodes.Count;
            dMid = new List<double>();
            dTop = new List<double>();
            dBot = new List<double>();
            dSecondBot = new List<double>();
            dSecondTop = new List<double>();
            b = new List<double>();
            u = new List<double>(new double[n]);
        }

        public double y(double x, double y)
        {
            return 2*x+3*y+1;
        }

        double function(double x, double y)
        {
            return 10*x+15*y+5;
        }

        double Step()
        {
            for (int i = 0; i < n; i++)
            {
                double sum = Sum(i);
                u[i] = u[i] + w / dMid[i] * (b[i] - sum);
            }

            residual = Res() / Norm();
            return residual;
        }

        double Norm()
        {
            double result = 0;
            for (int i = 0; i < n; i++)
            {
                result += u[i] * u[i];
            }
            return result;

        }

        double Sum(int i)
        {
            double sum = 0;
            int shift = net.n+1;
            sum += dMid[i]*u[i];
            if (i < n - 1) sum+= dTop[i] * u[i+1];
            if (i > 0) sum += dBot[i - 1] * u[i-1];
            if (i < n - shift) sum+=dSecondTop[i] * u[i+shift];
            if (i >= shift) sum+=dSecondBot[i - shift] * u[i-shift];
            return sum;
        }

        double Res()
        {
            double result = 0;
            for (int i = 0; i < n; i++)
            {
                double sum = b[i] - Sum(i);
                result += sum * sum;
            }
            return result;

        }

        public void GaussSeidel()
        {
            while (residual >= eps && iterations < maxIterations)
            {
                Step();
                iterations += 1;
            }
        }

        public void BuildMatrix()
        {
            int i = 0;
            int shift = net.n+1;
            foreach (var item in net.nodes)
            {
                    dMid.Add(-2.0 / (net.hx * net.hx) - 2.0 / (net.hy * net.hy) + gamma);
                    if (i < n - 1) dTop.Add(1.0 / net.hx / net.hx);
                    if (i > 0) dBot.Add(1.0 / net.hx / net.hx);
                    if (i < n - shift) dSecondTop.Add(1.0 / net.hy / net.hy);
                    if (i >= shift) dSecondBot.Add(1.0 / net.hy / net.hy);
                    b.Add(function(item.Key, item.Value));
                    i++;
            }
            
        }

        public void TestShit()
        {
            for(int i =0 ; i<= 8; i++)
                if (i != 4)
                {
                    dMid[i] = 1;
                    b[i] = 10;
                }
        }
    }
    

    class Program
    {
        static void Main(string[] args)
        {
            Net net = new Net(2,1,10,1,10);
            EQuaTion eq = new EQuaTion(net,10,5,1,1e-5);
            eq.BuildMatrix();
         //   eq.TestShit();
            eq.GaussSeidel();
            for (int i = 0; i < eq.u.Count; i++)
            {
                Console.WriteLine($"{eq.u[i]:e2} {eq.y(eq.net.nodes[i].Key,eq.net.nodes[i].Value):e2}");
            }
        }
    }
}