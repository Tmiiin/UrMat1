using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    enum Direction
    {
        Top,
        Bottom,
        Left,
        Right
    }
    class Net
    {
        public List<List<double>> nodes = new List<List<double>>();
        List<int> edges = new List<int>();
        public int kx, ky;
        public int n;
        public List<int> firstCondi = new List<int>();
        public List<List<int>> secondCondi=new List<List<int>>(); //{node, fieldNum, Direction}
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
            this.n = kx * ky;
            double x = xmin, y = ymin;
            int field = 0;
            for (int i = 0; i < ky; i++)
            {
                for (int j = 0; j < kx; j++)
                {
                    nodes.Add(new List<double> {x, y, 1});
                    if (i == 0 || i == ky - 1 || j == 0 || j == kx - 1) //проверка находится ли нод на границе
                    {
                        // if (i==ky-1)
                        // {
                        //     secondCondi.Add(new List<int>
                        //     {
                        //         j + i * kx,
                        //         1,
                        //         (int)Direction.Top
                        //     });
                        // }
                        // else
                        // {
                        //     firstCondi.Add(j+i*kx);
                        // }
                        nodes.Last().Add(0);
                        firstCondi.Add(j+i*kx);
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
            kx = nx1 + nx2+1;
            ky = ny1 + ny2+1;
            n = kx * ky;
            double x = x1;
            double y = y1;
            int k = 0;
            for (int i = 0; i < ky; i++)
            {
                for (int j = 0; j < kx; j++)
                {
                    List<double> node = new List<double> {x, y};
                    if (j <= nx1)
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
                        else node.Add(0);

                        x += hx2;
                    }

                    if (i == 0 || i==ky-1 ||j==0||j==nx1&&i<=ny1||i==ny1&&j>nx1||j==kx-1)
                    {
                        // if (i==ky-1)
                        // {
                        //     secondCondi.Add(new List<int>
                        //     {
                        //         j + i * kx,
                        //         1,
                        //         (int)Direction.Top
                        //     });
                        // }
                        // else
                        // {
                        //     firstCondi.Add(j+i*kx);
                        // }
                        if (node[2]>=0)
                        {
                            firstCondi.Add(j+i*kx);
                        }
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
}