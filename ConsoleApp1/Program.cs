﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace ConsoleApp1
{
    class Net
    {
        public List<List<double>> nodes = new List<List<double>>();
        List<int> edges = new List<int>();
        public int kx, ky;
        public int n;

        public Net()
        {
        }

        public Net(int n, double xmin, double xmax, double ymin, double ymax)
        {
            this.n = n;
            kx = n+1;
            ky = n+1;
            double hx = (xmax - xmin) / n;
            double hy = (ymax - ymin) / n;
            this.n = kx + ky;
            double x = xmin, y = ymin;
            int field = 0;
            for (int j = 0; j <= n; j++)
            {
                for (int i = 0; i <= n; i++)
                {
                    nodes.Add(new List<double> {x, y, 1});
                    if (i == 0 || i == ky - 1 || j == 0 || j == kx - 1) //проверка находится ли нод на границе
                    {
                        nodes.Last().Add(0);
                    }
                    else
                    {
                        nodes.Last().Add(1);
                    }

                    x += hx;
                }

                x = xmin;
                y += hy;
            }
        }

        public void BuildFormNet(double x1, double x2, double x3, double y1, double y2, double y3, int nx1, int nx2,
            int ny1, int ny2)
        {
            double hx1 = (x2 - x1) / nx1;
            double hx2 = (x3 - x2) / nx2;
            double hy1 = (y3 - y2) / ny1;
            double hy2 = (y3 - y2) / ny2;
            kx = nx1 + nx2;
            ky = ny1 + ny2;
            double x = x1;
            double y = y1;
            for (int i = 0; i < ny1 + ny2; i++)
            {
                for (int j = 0; j < nx1 + nx2; j++)
                {
                    List<double> node = new List<double> {x, y};
                    if (j < nx1)
                    {
                        node.Add(0);
                        x += hx1;
                    }
                    else
                    {
                        if (i < ny1)
                        {
                            node.Add(-1);
                        }

                        x += hx2;
                    }

                    if (j == 0 || j == nx1 - 1 && i < ny1 || i == ny1 + ny2 - 1 || j == nx1 + nx2 - 1 && i >= ny1 - 1)
                    {
                        node.Add(0);
                    }
                    else
                    {
                        node.Add(1);
                    }

                    nodes.Add(node);
                }

                x = x1;
                if (i < ny1)
                {
                    y += hy1;
                }
                else
                {
                    y += hy2;
                }
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
        List<int> firstCondi;
        List<KeyValuePair<int, int>> secondCondi;

        double Ug(double x, double y)
        {
            return 2 * x + 3 * y + 1;
        }

        public void AddFirst(List<int> firstCondi)
        {
            this.firstCondi = firstCondi;
        }

        public void AddSecond(List<KeyValuePair<int, int>> secondCondi)
        {
            this.secondCondi = secondCondi;
        }

        public EQuaTion(Net net, double lambda, double gamma, double w, double eps)
        {
            this.net = net;
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

        public double y(double x, double y)
        {
            return 2 * x + 3 * y + 1;
        }

        double function(double x, double y)
        {
            return 10 * x + 15 * y + 5;
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
            int shift = net.n + 1;
            sum += dMid[i] * u[i];
            if (i < n - 1) sum += dTop[i] * u[i + 1];
            if (i > 0) sum += dBot[i - 1] * u[i - 1];
            if (i < n - shift) sum += dSecondTop[i] * u[i + shift];
            if (i >= shift) sum += dSecondBot[i - shift] * u[i - shift];
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
                    if (net.nodes[k][3] == 0)
                    {
                        dMid[k] = 1;
                        b[k] = Ug(net.nodes[k][0], net.nodes[k][1]);
                        continue;
                    }

                    double hxNext = net.nodes[k + 1][0] - net.nodes[k][0];
                    double hxPrev = net.nodes[k][0] - net.nodes[k - 1][0];
                    double hyNext = net.nodes[k + net.kx][1] - net.nodes[k][1];
                    double hyPrev = net.nodes[k][1] - net.nodes[k - net.kx][1];
                    int shift = net.kx + 1;
                    dMid[k] = -2.0 / (hxNext * hxPrev) - 2.0 / (hyNext * hyPrev)+gamma;
                    if (k < n - 1) dTop[k] = 2.0 / (hxNext * (hxNext + hxPrev));
                    if (k > 0) dBot[k - 1] = 2.0 / (hxPrev * (hxNext + hxPrev));
                    if (k < n - shift) dSecondTop[k] = 2.0 / (hyNext * (hyNext + hyPrev));
                    if (k >= shift) dSecondBot[k - shift] = 2.0 / (hyPrev * (hyNext + hyPrev));
                    b[k] = function(net.nodes[k][0], net.nodes[k][1]);
                }
            }
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


    class Program
    {
        static void Main(string[] args)
        {
            Net net = new Net(2, 1, 10, 1, 10);
            EQuaTion eq = new EQuaTion(net, 10, 5, 0.8, 1e-5);
            eq.AddFirst(new List<int> {0, 1, 2, 3, 5, 6, 7, 8});
            eq.BuildMatrix();
            //   eq.TestShit();
            eq.GaussSeidel();
            for (int i = 0; i < eq.u.Count; i++)
            {
                Console.WriteLine($"{eq.u[i]:e2} {eq.y(eq.net.nodes[i][0], eq.net.nodes[i][1]):e2}");
            }
        }
    }
}