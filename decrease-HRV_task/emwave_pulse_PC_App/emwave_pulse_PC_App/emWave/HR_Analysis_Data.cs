using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ZedGraph;

namespace emwave_pulse_PC_App
{
    class HR_Analysis_Data
    {

        /// Most recent time
        /// </summary>
        public double MostRecentTime
        { get { return mostRecentTime_; } }
        private double mostRecentTime_ = 0;

        public double FirstTime
        { get { return firstTime_; } }
        private double firstTime_ = 0;

        public int NumOfInstantHR
        { get { return numOfInstantHR_; } }
        private int numOfInstantHR_ = 0;

        const int MAX_NUMBER_MESUREMENTS = 40 * 1000;// ~1000s of data

        /// Analysis data
        /// </summary>
        public List<double> instantHRBuffer = new List<double>();
        public List<double> tempInstantHRBuffer = new List<double>();
        public List<double> localMaxHRBuffer = new List<double>();
        public List<double> dataBuffer = new List<double>();
        public List<double> timeBuffer = new List<double>();

        private Boolean isFirstAveInstantHr = true;
        private Boolean isFirstData = false;
        private Boolean updateOtherParams = false;
        private Boolean localMaxHRFound = false;

        public List<RollingPointPairList> instantHRPoint        = new List<RollingPointPairList>();
        public List<RollingPointPairList> coherencePoint        = new List<RollingPointPairList>();
        public List<RollingPointPairList> antiCoherencePoint    = new List<RollingPointPairList>();
        public List<RollingPointPairList> calmPoint             = new List<RollingPointPairList>();
        public List<RollingPointPairList> EPPoint               = new List<RollingPointPairList>();
        public List<RollingPointPairList> localMaxHRPoint       = new List<RollingPointPairList>();
        public List<RollingPointPairList> ASPoint               = new List<RollingPointPairList>();
        public List<RollingPointPairList> accCalmPoint          = new List<RollingPointPairList>();
        public List<RollingPointPairList> emWaveEPPoint         = new List<RollingPointPairList>();
        public double currentInstantHR_;
        public double localMaxHR_;

        public double coherenceScore, antiCoherenceScore;
        public double calmScore = 0.00, prize, accCalmScore = 0.00;
        public Boolean isNewCalmScoreComing = false;

        private const int timeShiftWindow = 5;
        private const int timeShiftWindowMaxHR = 15;

        public void AddData(double hr, int lowEP, int highEP, int emWaveEP, int AS, double time, Boolean isStartCalEP)
        {
            // Add instant HR data if available
            AddHRData(hr, time);

            int EP = lowEP + highEP;

            // Add other data once we have HR data available
            if (updateOtherParams) {

                isInitializedPoints();
                
                AddEPData(lowEP, highEP, time);
                AddEmWaveEPData(emWaveEP, time);
                AddASData(AS, time);

                if (localMaxHRFound && isStartCalEP)
                {
                    localMaxHRPoint[0].Add(Math.Floor(time), localMaxHR_);
                    AddCoherenceData(EP, time);
                    AddCalmData(antiCoherenceScore, time);
                    AddAccCalmData(calmScore, time);
                    localMaxHRFound = false;

                }
                else {
                    // Dummy 0 or unchanged data over every 5 seconds
                    coherencePoint[0].Add(Math.Floor(time), coherenceScore);
                    antiCoherencePoint[0].Add(Math.Floor(time), antiCoherenceScore);
                    localMaxHRPoint[0].Add(Math.Floor(time), localMaxHR_);
                    calmPoint[0].Add(Math.Floor(time), calmScore);
                    accCalmPoint[0].Add(Math.Floor(time), accCalmScore);
                }

                updateOtherParams = false;
            }

        }

        private void isInitializedPoints() {
            if (coherencePoint.Count == 0)
            {
                coherencePoint.Add(new RollingPointPairList(MAX_NUMBER_MESUREMENTS));
                coherencePoint.TrimExcess();
            }

            if (antiCoherencePoint.Count == 0)
            {
                antiCoherencePoint.Add(new RollingPointPairList(MAX_NUMBER_MESUREMENTS));
                antiCoherencePoint.TrimExcess();
            }

            if (calmPoint.Count == 0)
            {
                calmPoint.Add(new RollingPointPairList(MAX_NUMBER_MESUREMENTS));
                calmPoint.TrimExcess();
            }

            if (accCalmPoint.Count == 0)
            {
                accCalmPoint.Add(new RollingPointPairList(MAX_NUMBER_MESUREMENTS));
                accCalmPoint.TrimExcess();
            }

            if (emWaveEPPoint.Count == 0)
            {
                emWaveEPPoint.Add(new RollingPointPairList(MAX_NUMBER_MESUREMENTS));
                emWaveEPPoint.TrimExcess();
            }
        }

        public void AddCalmData(double measurements, double time)
        {
            
            if (localMaxHR_ - currentInstantHR_ > -0.5)
            {
                prize = 1;
            }
            else {
                prize = -2;
            }

            calmScore = prize + antiCoherenceScore;
            calmPoint[0].Add(Math.Floor(time), calmScore);

            isNewCalmScoreComing = true;

        }

        public void AddAccCalmData(double measurements, double time)
        {
            accCalmScore += measurements;

            accCalmPoint[0].Add(Math.Floor(time), accCalmScore);

        }

        public void AddEPData(int lowEP, int highEP, double time) {
            if (EPPoint.Count == 0)
            {
                EPPoint.Add(new RollingPointPairList(MAX_NUMBER_MESUREMENTS));
                EPPoint.TrimExcess();
            }

            EPPoint[0].Add(Math.Floor(time), lowEP, highEP);

        }

        public void AddEmWaveEPData(int measurements, double time)
        {
            int emwaveEP = measurements;

            if (emWaveEPPoint.Count == 0)
            {
                emWaveEPPoint.Add(new RollingPointPairList(MAX_NUMBER_MESUREMENTS));
                emWaveEPPoint.TrimExcess();
            }

            emWaveEPPoint[0].Add(Math.Floor(time), emwaveEP);

        }

        public void AddASData(int measurements, double time)
        {
            int AS = measurements;

            if (ASPoint.Count == 0)
            {
                ASPoint.Add(new RollingPointPairList(MAX_NUMBER_MESUREMENTS));
                ASPoint.TrimExcess();
            }

            ASPoint[0].Add(Math.Floor(time), AS);

        }

        public void AddCoherenceData(int measurements, double time)
        {
            int EP = measurements;

            coherenceScore = calCoherenceScore(EP);
            coherencePoint[0].Add(Math.Floor(time), coherenceScore);

            antiCoherenceScore = 10 - coherenceScore;
            antiCoherencePoint[0].Add(Math.Floor(time), antiCoherenceScore);

        }

        private double calCoherenceScore(int EP)
        {

            double coherenceScore = 0;
            coherenceScore = Math.Log(EP + 1);
            return coherenceScore;
        }


        public void AddHRData(double measurements, double time)
        {

            if (instantHRPoint.Count == 0)
            {
                instantHRPoint.Add(new RollingPointPairList(MAX_NUMBER_MESUREMENTS));
                instantHRPoint.TrimExcess();
            }

            if (!isFirstData)
            {
                isFirstData = true;
                firstTime_ = time;
            }
                        
            calInstantHR(time);

            // Update Every 15 second
            if (updateOtherParams)
            {
                calLocalMaxHR(time);
            }

            //Add the data to the buffer
            timeBuffer.Add(time);
            dataBuffer.Add(measurements);
            
            mostRecentTime_ = time;
        }

        
        private void calInstantHR(double time) {
            if (isFirstAveInstantHr)
            {
                if (time - Math.Floor(firstTime_) > timeShiftWindow)
                {
                    isFirstAveInstantHr = false;
                    int count = dataBuffer.Count;
                    double temp = 0;

                    for (int i = 0; i < count; i++)
                        temp += dataBuffer.ElementAt(i);

                    currentInstantHR_ = temp / count;
                    instantHRPoint[0].Add(Math.Floor(time), currentInstantHR_);
                    
                    instantHRBuffer.Add(currentInstantHR_);
                    
                    while (Math.Floor(timeBuffer.ElementAt(0)) - Math.Floor(firstTime_) < 1)
                    {
                        if (timeBuffer.Count > 0)
                        {
                            timeBuffer.RemoveAt(0);
                            dataBuffer.RemoveAt(0);
                        }
                    }

                    firstTime_ = timeBuffer.ElementAt(0);
                    updateOtherParams = true;
                }
            }
            else
            {
                if (time - Math.Floor(firstTime_) > timeShiftWindow)
                {
                    int count = dataBuffer.Count;
                    double temp = 0;

                    for (int i = 0; i < count; i++)
                        temp += dataBuffer.ElementAt(i);

                    currentInstantHR_ = temp / count;
                    instantHRPoint[0].Add(Math.Floor(time), currentInstantHR_);
                    instantHRBuffer.Add(currentInstantHR_);
                    //tempInstantHRBuffer.Add(currentInstantHR_);

                    while (Math.Floor(timeBuffer.ElementAt(0)) - Math.Floor(firstTime_) < 1)
                    {
                        if (timeBuffer.Count > 0)
                        {
                            timeBuffer.RemoveAt(0);
                            dataBuffer.RemoveAt(0);
                        }
                    }

                    firstTime_ = timeBuffer.ElementAt(0);
                    updateOtherParams = true;
                }

            }
        }

        private void calLocalMaxHR(double time)
        {
            // Calc every 15 sec, first 15 sec only has 10 points
            if (localMaxHRPoint.Count == 0)
            {
                localMaxHRPoint.Add(new RollingPointPairList(MAX_NUMBER_MESUREMENTS));
                localMaxHRPoint.TrimExcess();
            }

            // First Local Max HR
            if ((numOfInstantHR_ + timeShiftWindow) == timeShiftWindowMaxHR)
            {

                if (tempInstantHRBuffer.Count > 0)
                {
                    localMaxHR_ = tempInstantHRBuffer.Max();
                    localMaxHRBuffer.Add(localMaxHR_);
                }

                localMaxHRFound = true;
            }

            // Moving every 5 second
            if ((numOfInstantHR_ + timeShiftWindow) > timeShiftWindowMaxHR && (numOfInstantHR_ % timeShiftWindow) == 0)
            {
                if (tempInstantHRBuffer.Count > 0)
                {
                    localMaxHR_ = tempInstantHRBuffer.Max();
                    localMaxHRBuffer.Add(localMaxHR_);
                    if (tempInstantHRBuffer.Count > 10) {
                        tempInstantHRBuffer.RemoveRange(0, tempInstantHRBuffer.Count - 10);
                    }
                    
                }
                localMaxHRFound = true;
            }

            numOfInstantHR_ = numOfInstantHR_ + 1;
            //Buffer to calc local Max
            tempInstantHRBuffer.Add(currentInstantHR_);
        }

    }
}
