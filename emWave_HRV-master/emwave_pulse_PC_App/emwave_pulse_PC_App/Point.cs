using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace emwave_pulse_PC_App
{
    class Point
    {

        private double x;
        private double y;
        
        public double X
        {
            set { x = value; }
            get { return x; }
        }
        
        public double Y
        {
            set { y = value; }
            get { return y;}
        }

        public void Add(double X, double Y) {
            this.x = X;
            this.y = Y;
        }

    }
}
