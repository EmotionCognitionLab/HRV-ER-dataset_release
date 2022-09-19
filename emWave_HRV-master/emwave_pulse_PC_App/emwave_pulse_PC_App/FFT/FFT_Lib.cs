using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;


namespace emwave_pulse_PC_App
{
    

    class FFT_Lib
    {

        public static int BitReverse(int n, int bits){
            int reversedN = n;
            int count = bits - 1;

            n >>= 1;
            while (n > 0) {
                reversedN = (reversedN << 1) | (n & 1);
                count--;
                n >>= 1;
            }

            return ((reversedN << count) & ((1 << bits) - 1));
        }

        public static void FFT(Complex[] buffer) {
            
            int bits = (int)Math.Log(buffer.Length, 2);

            for (int j = 1; j < buffer.Length / 2; j++) {
                int swapPos = BitReverse(j, bits);
                var temp = buffer[j];
                buffer[j] = buffer[swapPos];
                buffer[swapPos] = temp;
            }

            for (int N = 2; N <= buffer.Length; N <<= 1) {
                for (int i = 0; i < buffer.Length; i += N) {
                    for (int k = 0; k < N / 2; k++) {
                        int evenIndex = i + k;
                        int oddIndex = i + k + (N / 2);

                        var even = buffer[evenIndex];
                        var odd = buffer[oddIndex];

                        double term = -2 * Math.PI * k / (double)N;
                        //Complex exp = new Complex( Math.Cos(term), Math.Sin(term) ) * odd;

                        //buffer[evenIndex] = even + exp;
                        //buffer[oddIndex] = even - exp;
                      
                    }
                }
            }

        }

    }

    /*
     class FFTElement
        {
            public double re = 0.0;
            public double im = 0.0;
            public FFTElement next;
            public uint revTgt;
        }

        private uint m_logN = 0;
        private uint m_N = 0;
        private FFTElement[] m_X;

        public void init(uint logN) {
            m_logN = logN;
            m_N = (uint) (1 << (int)m_logN);

            // Alocate elements for linked list of complex numbers
            m_X = new FFTElement[m_N];

            for (uint k = 0; k < m_N; k++)
                m_X[k] = new FFTElement();

            // Set up next pointer
            for (uint k = 0; k < m_N - 1; k++)
                m_X[k].next = m_X[k + 1];

            // Bit reversal
            for (uint k = 0; k < m_N - 1; k++)
                m_X[k].revTgt = BitReverse(k, logN);
        }

        private uint BitReverse(uint x, uint numBits) {

            uint y = 0;
            for (uint i = 0; i < numBits; i++)
            {
                y <<= 1;
                y |= x & 0x0001;
                x >>= 1;
            }

            return y;
        }

        public void run (double[] xRe, double[] xIm, bool inverse = false) {

            uint numFiles = m_N >> 1;
            uint span = m_N >> 1;
            uint spacing = m_N;
            uint wIndexStep = 1;

            FFTElement x = m_X[0];
            uint k = 0;
            double scale = inverse ? (1.0 / m_N) : 1.0;

            while (x != null) { 
                x.re = scale * xRe[k];
                x.im = scale * xIm[k];
                x = x.next;
                k++;
            }

            for (uint stage = 0; stage < m_logN; stage++ )
            {
                double wAngleInc = wIndexStep * 2.0 * Math.PI / m_N;
                if (inverse == false)
                    wAngleInc *= -1;

                double wMulRe = Math.Cos(wAngleInc);
                double wMulIm = Math.Sin(wAngleInc);

                for (uint start = 0; start < m_N; start += spacing) { 
                    FFTElement xTop = m_X[start];
                    FFTElement xBot = m_X[start+span];

                    double wRe = 1.0;
                    double wIm = 0.0;

                    for (uint flyCount = 0; flyCount < numFiles; ++flyCount) {

                        double xTopRe = xTop.re;
                        double xTopIm = xTop.im;
                        double xBotRe = xBot.re;
                        double xBotIm = xBot.im;

                        xTop.re = xTopRe + xBotRe;
                        xTop.im = xTopIm + xBotIm;
                        xBotRe = xTopRe - xBotRe;
                        xBotIm = xTopIm - xBotIm;

                        xBot.re = xBotRe * wRe - xBotIm * wIm;
                        xBot.im = xBotRe * wIm + xBotIm * wRe;

                        xTop = xTop.next;
                        xBot = xBot.next;

                        double tRe = wRe;
                        wRe = wRe * wMulRe - wIm * wMulIm;
                        wIm = tRe * wMulIm - wIm * wMulRe;

                    }

                }

                numFiles >>= 1;
                span >>= 1;
                spacing >>= 1;
                wIndexStep <<= 1;

            }

            x = m_X[0];
            while (x != null) {
                uint target = x.revTgt;
                xRe[target] = x.re;
                xIm[target] = x.im;
                x = x.next;
            }

        }
     
     
     */
}
