using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ZedGraph;

namespace emwave_pulse_PC_App
{
    class emWaveData
    {
        public List<RollingPointPairList> IBIPoint = new List<RollingPointPairList>();
        public List<RollingPointPairList> ASPoint = new List<RollingPointPairList>();
        public List<RollingPointPairList> HRPoint = new List<RollingPointPairList>();
        public List<RollingPointPairList> sPoint = new List<RollingPointPairList>();
        public List<RollingPointPairList> EPPoint = new List<RollingPointPairList>();

        const int MAX_NUMBER_MESUREMENTS = 40 * 1000;// ~1000s of data

        public void AddData(int hr, int EP, int AS, int s, double IBI, double time)
        {
            
            isInitializedPoints();

            IBIPoint[0].Add(time, IBI);
            ASPoint[0].Add(time, AS);
            HRPoint[0].Add(time, hr);
            sPoint[0].Add(time, s);
            EPPoint[0].Add(time, EP);
              
        }

        private void isInitializedPoints()
        {
            if (IBIPoint.Count == 0)
            {
                IBIPoint.Add(new RollingPointPairList(MAX_NUMBER_MESUREMENTS));
                IBIPoint.TrimExcess();
            }

            if (ASPoint.Count == 0)
            {
                ASPoint.Add(new RollingPointPairList(MAX_NUMBER_MESUREMENTS));
                ASPoint.TrimExcess();
            }

            if (HRPoint.Count == 0)
            {
                HRPoint.Add(new RollingPointPairList(MAX_NUMBER_MESUREMENTS));
                HRPoint.TrimExcess();
            }

            if (sPoint.Count == 0)
            {
                sPoint.Add(new RollingPointPairList(MAX_NUMBER_MESUREMENTS));
                sPoint.TrimExcess();
            }

            if (EPPoint.Count == 0)
            {
                EPPoint.Add(new RollingPointPairList(MAX_NUMBER_MESUREMENTS));
                EPPoint.TrimExcess();
            }
        }

    }
}
