using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Filter;

namespace Filter
{
    internal class FirFilter
    {
        public static double trapWaveEdge = 15;//陷波器边沿频率

        private ArrayList filterDataArrayList;
        private int channelNum = 62;
        private int filterQueueDataNum = 501; //滤波数据点数(阶次) 151 默认501
        private const double PI = Math.PI;//PI = 3.1415926;
        private double[] h;
        private double sum_h;//h的值之和

        //以下四个参数存储起来是为了与上次滤波配置参数，若参数变化，需重新计算系统函数h的值
        private bool is50HzFiltered = false;

        private bool isBandpassFiltered = false;
        private double low = 0;
        private double high = 200;
        private double low_limit = 0;
        private double high_limit = 250;

        //RC高通滤波
        private double[] lastOriginalData;//x[n-1]

        private double[] lastFilteredData;//y[n-1]
        private bool[] isRCFilteredStarted;
        private double[] lastOriginalData2;//x[n-2]
        private double[] lastFilteredData2;//y[n-2]
        private int[] numOfRCFilterData;

        //低通滤波
        private double[] lastOriginalDataLowPass;//x[n-1]

        private double[] lastFilteredDataLowPass;//y[n-1]
        private bool[] isRCFilteredStartedLowPass;
        private double[] lastOriginalData2LowPass;//x[n-2]
        private double[] lastFilteredData2LowPass;//y[n-2]
        private int[] numOfRCFilterDataLowPass;

        //RC带通滤波
        private double[] lastOriginalDataBP;//x[n-1]

        private double[] lastFilteredDataBP;//y[n-1]
        private bool[] isRCFilteredStartedBP;
        private double[] lastOriginalData2BP;//x[n-2]
        private double[] lastFilteredData2BP;//y[n-2]
        private int[] numOfRCFilterDataBP;

        //方波均值滤波
        private double[] maxMean;//最大限波值

        private double[] minMean;//最小限波值
        private int numofMean = 100;//求滑动平均的数据个数
        private double[] zeroMean;//数据均值
        private ArrayList meanFilterDataArrayList;
        private int meanFilterQueueDataNum = 1000;//均值滤波数据点数（适用于大于0.4Hz的波形）

        private bool[] isDataPrepared;//该参数用于指示滤波用的数据是否已经储存完毕

        public FirFilter()
        {
            filterDataArrayList = new ArrayList();
            //定义25个队列，对应25个通道，每个队列存放filterQueueDataNum个数据，用于滤波分析，每次采集n个数据时，放于队列最后n个位置，队列前n个位置数据删除
            for (int i = 0; i < channelNum; i++)
            {
                Queue dataQueue = Queue.Synchronized(new Queue());
                filterDataArrayList.Add(dataQueue);
            }
            //暂时替换成滤波阶次
            h = new double[filterQueueDataNum];
            //均值限波
            meanFilterDataArrayList = new ArrayList();//定义25个队列，对应25个通道，每个队列存放filterQueueDataNum个数据，用于滤波分析，每次采集n个数据时，放于队列最后n个位置，队列前n个位置数据删除
            for (int i = 0; i < channelNum; i++)
            {
                Queue dataQueue = Queue.Synchronized(new Queue());
                meanFilterDataArrayList.Add(dataQueue);
            }
            lastOriginalData = new double[channelNum];
            lastFilteredData = new double[channelNum];
            lastOriginalData2 = new double[channelNum];
            lastFilteredData2 = new double[channelNum];
            isRCFilteredStarted = new bool[channelNum];
            isDataPrepared = new bool[channelNum];
            numOfRCFilterData = new int[channelNum];
            lastOriginalDataLowPass = new double[channelNum];
            lastFilteredDataLowPass = new double[channelNum];
            lastOriginalData2LowPass = new double[channelNum];
            lastFilteredData2LowPass = new double[channelNum];
            isRCFilteredStartedLowPass = new bool[channelNum];
            numOfRCFilterDataLowPass = new int[channelNum];
            lastOriginalDataBP = new double[channelNum];
            lastFilteredDataBP = new double[channelNum];
            lastOriginalData2BP = new double[channelNum];
            lastFilteredData2BP = new double[channelNum];
            isRCFilteredStartedBP = new bool[channelNum];
            numOfRCFilterDataBP = new int[channelNum];
            for (int i = 0; i < channelNum; i++)
            {
                lastOriginalData[i] = 0.0;
                lastFilteredData[i] = 0.0;
                lastOriginalData2[i] = 0.0;
                lastFilteredData2[i] = 0.0;
                isRCFilteredStarted[i] = false;
                isDataPrepared[i] = false;
                numOfRCFilterData[i] = 0;
                lastOriginalDataLowPass[i] = 0.0;
                lastFilteredDataLowPass[i] = 0.0;
                lastOriginalData2LowPass[i] = 0.0;
                lastFilteredData2LowPass[i] = 0.0;
                isRCFilteredStartedLowPass[i] = false;
                numOfRCFilterDataLowPass[i] = 0;
                lastOriginalDataBP[i] = 0.0;
                lastFilteredDataBP[i] = 0.0;
                lastOriginalData2BP[i] = 0.0;
                lastFilteredData2BP[i] = 0.0;
                isRCFilteredStartedBP[i] = false;
                numOfRCFilterDataBP[i] = 0;
            }
        }

        /// <summary>
        /// 单位脉冲响应
        /// </summary>
        /// <param name="n">阶次</param>
        /// <param name="band"></param>
        /// <param name="fln">低频</param>
        /// <param name="fhn">高频</param>
        /// <param name="wn">窗函数类型</param>
        /// <param name="h"></param>
        private double ImpulseResponse(int n, int band, double fln, double fhn, bool isBandpassFiltered, bool is50HzFiltered, int wn, double rateOfSample, params double[] h)//求频率响应函数
        {
            int n2, mid;
            double delay, s, beta;
            beta = 0.0;
            if (wn == 7)
            {
                //Debug.WriteLine("Input beta parameter of Kaiser window (3<beta<10)\n");
                //beta = Int32.Parse(Console.ReadLine());
                beta = 4;
            }
            if (n % 2 == 0)
            {
                n2 = n / 2 - 1;
                mid = 1;
            }
            else
            {
                n2 = n / 2;
                mid = 0;
            }
            delay = n / 2;
            double wc1 = 2 * PI * fln / rateOfSample;
            double wc2 = 2 * PI * fhn / rateOfSample;
            //double bandstop1 = 2 * PI * (50 - trapWaveEdge) / rateOfSample;//阻带频率0.3Hz
            //double bandstop2 = 2 * PI * (50 + trapWaveEdge) / rateOfSample;//阻带频率0.3Hz
            double wcBandstop1 = 2 * PI * (50 - trapWaveEdge) / rateOfSample;
            double wcBandstop2 = 2 * PI * (50 + trapWaveEdge) / rateOfSample;

            double w;
            //陷波程序仅修改了case 3带通滤波情况，其他情况若需修改，请参照case 3
            switch (band)
            {
                case 1:
                    {
                        for (int i = 0; i <= n2; i++)
                        {
                            s = i - delay;
                            Window(wn, n + 1, i, beta, out w);
                            h[i] = (Math.Sin(wc1 * s) / (PI * s)) * w;
                            h[n - 1 - i] = h[i];
                        }
                        if (mid == 0)
                            h[n / 2] = wc1 / PI;
                        break;
                    }
                case 2:
                    {
                        for (int i = 0; i <= n2; i++)
                        {
                            s = i - delay;
                            Window(wn, n + 1, i, beta, out w);
                            h[i] = (Math.Sin(PI * s) - Math.Sin(wc1 * s)) / (PI * s);
                            h[i] = h[i] * w;
                            h[n - 1 - i] = h[i];
                        }
                        if (mid == 0)
                            h[n / 2] = 1.0 - wc1 / PI;
                        break;
                    }
                case 3:
                    {
                        for (int i = 0; i <= n2; i++)
                        {
                            s = i - delay;
                            Window(wn, n + 1, i, beta, out w);
                            if (is50HzFiltered && isBandpassFiltered && fln <= 50.0 - trapWaveEdge && fhn > 50.0 + trapWaveEdge && fhn <= this.high_limit)
                            {
                                //软件陷波+带通滤波范围包含陷波范围
                                h[i] = (Math.Sin(wc2 * s) - Math.Sin(wcBandstop2 * s) + Math.Sin(wcBandstop1 * s) - Math.Sin(wc1 * s)) / (PI * s);
                            }
                            else if (is50HzFiltered && (isBandpassFiltered == false))
                            {
                                //软件陷波+无带通滤波
                                fln = 0; fhn = high_limit;
                                wc1 = 2 * PI * fln / rateOfSample;
                                wc2 = 2 * PI * fhn / rateOfSample;
                                //h[i] = (Math.Sin(wc2 * s) - Math.Sin(wcBandstop2 * s) + Math.Sin(wcBandstop1 * s)  - Math.Sin(wc1 * s)) / (PI * s);//仅50Hz陷波
                                h[i] = -Math.Sin(wcBandstop2 * s) + Math.Sin(wcBandstop1 * s);//仅50Hz陷波
                            }
                            else if (isBandpassFiltered == true && is50HzFiltered == true && fhn < 50.0 - trapWaveEdge)
                            {
                                //软件陷波+带通滤波
                                h[i] = (Math.Sin(wc2 * s) - Math.Sin(wc1 * s)) / (PI * s);
                            }
                            else if (is50HzFiltered && isBandpassFiltered && fln < 50.0 - trapWaveEdge && fhn > 50.0 - trapWaveEdge && fhn <= 50.0 + trapWaveEdge)
                            {
                                //软件陷波+带通滤波，带通下限<50-trapWaveEdge,带通上限大于50-trapWaveEdge,小于50+trapWaveEdge
                                h[i] = (Math.Sin(wcBandstop1 * s) - Math.Sin(wc1 * s)) / (PI * s);
                            }
                            else if (is50HzFiltered && isBandpassFiltered && fln > 50.0 - trapWaveEdge && fln < 50.0 + trapWaveEdge && fhn > 50.0 + trapWaveEdge)
                            {
                                //软件陷波+带通滤波，带通下限大于50-trapWaveEdge,小于50+trapWaveEdge,带通上限大于50+trapWaveEdge
                                h[i] = (Math.Sin(wc2 * s) - Math.Sin(wcBandstop2 * s)) / (PI * s);
                            }
                            else if (is50HzFiltered && isBandpassFiltered && fln >= 50.0 - trapWaveEdge && fln < 50.0 + trapWaveEdge && fhn > 50.0 - trapWaveEdge && fhn <= 50.0 + trapWaveEdge)
                            {
                                //软件陷波+带通滤波，带通下限大于50-trapWaveEdge,小于50+trapWaveEdge,带通上限大于50-trapWaveEdge,小于50+trapWaveEdge,此时输出波形数据为0
                                h[i] = 0;
                            }
                            else
                            {
                                //无陷波+带通
                                h[i] = (Math.Sin(wc2 * s) - Math.Sin(wc1 * s)) / (PI * s);
                            }
                            h[i] = h[i] * w;
                            h[n - 1 - i] = h[i];
                        }
                        if (mid == 0)
                        {
                            if (is50HzFiltered && isBandpassFiltered && fln <= 50.0 - trapWaveEdge && fhn > 50.0 + trapWaveEdge && fhn <= this.high_limit)
                            {
                                //软件陷波+带通滤波范围包含陷波范围
                                h[n / 2] = (wc2 - wcBandstop2 + wcBandstop1 - wc1) / PI;
                            }
                            else if (is50HzFiltered && (isBandpassFiltered == false))
                            {
                                //软件陷波+无带通滤波
                                fln = 0; fhn = high_limit;
                                wc1 = 2 * PI * fln / rateOfSample;
                                wc2 = 2 * PI * fhn / rateOfSample;
                                h[n / 2] = (wc2 - wcBandstop2 + wcBandstop1 - wc1) / PI;//仅50Hz陷波
                            }
                            else if (isBandpassFiltered == true && is50HzFiltered == true && fhn < 50.0 - trapWaveEdge)
                            {
                                //软件陷波（无效）+带通滤波
                                h[n / 2] = (wc2 - wc1) / PI;
                            }
                            else if (is50HzFiltered && isBandpassFiltered && fln < 50.0 - trapWaveEdge && fhn > 50.0 - trapWaveEdge && fhn <= 50.0 + trapWaveEdge)
                            {
                                //软件陷波+带通滤波，带通下限<50-trapWaveEdge,带通上限大于50-trapWaveEdge,小于50+trapWaveEdge
                                h[n / 2] = (wcBandstop1 - wc1) / PI;
                            }
                            else if (is50HzFiltered && isBandpassFiltered && fln > 50.0 - trapWaveEdge && fln < 50.0 + trapWaveEdge && fhn > 50.0 + trapWaveEdge)
                            {
                                //软件陷波+带通滤波，带通下限大于50-trapWaveEdge,小于50+trapWaveEdge,带通上限大于50+trapWaveEdge
                                h[n / 2] = (wc2 - wcBandstop2) / PI;
                            }
                            else if (is50HzFiltered && isBandpassFiltered && fln >= 50.0 - trapWaveEdge && fln < 50.0 + trapWaveEdge && fhn > 50.0 - trapWaveEdge && fhn <= 50.0 + trapWaveEdge)
                            {
                                //软件陷波+带通滤波，带通下限大于50-trapWaveEdge,小于50+trapWaveEdge,带通上限大于50-trapWaveEdge,小于50+trapWaveEdge,此时输出波形数据为0
                                h[n / 2] = 0;
                            }
                            else
                            {
                                //无陷波+带通
                                h[n / 2] = (wc2 - wc1) / PI;
                            }
                        }
                        break;
                    }
                case 4:
                    {
                        for (int i = 0; i <= n - 1; i++)
                        {
                            s = i - delay;
                            Window(wn, n + 1, i, beta, out w);
                            h[i] = (Math.Sin(wc1 * s) + Math.Sin(PI * s) - Math.Sin(wc2 * s)) / (PI * s);
                            h[i] = h[i] * w;
                            h[n - 1 - i] = h[i];
                        }
                        if (mid == 0)
                            h[n / 2] = (wc1 + PI - wc2) / PI;
                        break;
                    }
            }
            sum_h = 0;
            for (int i = 0; i < h.Length; i++)
                sum_h = sum_h + h[i];
            return sum_h;
        }

        private void Window(int type, int n, int i, double beta, out double w)//窗函数
        {
            int k;
            w = 1.0;
            switch (type)
            {
                case 1:
                    {
                        w = 1.0;
                        break;
                    }
                case 2:
                    {
                        k = (n - 2) / 10;
                        if (i <= k)
                            w = 0.5 * (1.0 - Math.Cos(i * PI / (k + 1)));
                        if (i > n - k - 2)
                            w = 0.5 * (1.0 - Math.Cos((n - i - 1) * PI / (k + 1)));
                        break;
                    }
                case 3:
                    {
                        w = 1.0 - Math.Abs(1.0 - 2 * i / (n - 1.0));
                        break;
                    }
                case 4:
                    {
                        w = 0.5 * (1.0 - Math.Cos(2 * i * PI / (n - 1)));
                        break;
                    }
                case 5:
                    {
                        w = 0.54 - 0.46 * Math.Cos(2 * i * PI / (n - 1));
                        break;
                    }
                case 6:
                    {
                        w = 0.42 - 0.5 * Math.Cos(2 * i * PI / (n - 1)) + 0.08 * Math.Cos(4 * i * PI / (n - 1));
                        break;
                    }
                case 7:
                    {
                        kaiser(i, n, beta, out w);
                        break;
                    }
            }
        }

        private void kaiser(int i, int n, double beta, out double w)//kaiser窗函数
        {
            double a, a2, b1, b2, beta1;
            besse10(beta, out b1);
            a = 2.0 * i / (double)(n - 1) - 1.0;
            a2 = a * a;
            beta1 = beta * Math.Sqrt(1.0 - a2);
            besse10(beta1, out b2);
            w = b2 / b1;
        }

        private void besse10(double x, out double sum)
        {
            int i;
            double d, y, d2;
            y = x / 2.0;
            d = 1.0;
            sum = 1.0;
            for (i = 1; i <= 25; i++)
            {
                d = d * y / i;
                d2 = d * d;
                sum = sum + d2;
                if (d2 < sum * (1.0e-8))
                    break;
            }
        }

        /// <summary>
        /// 卷积计算
        /// </summary>
        /// <param name="x">输入信号</param>
        /// <param name="h">单位脉冲响应</param>
        /// <returns></returns>
        private double[] xcorr(double[] x, double[] h)
        {
            if (x.Length > h.Length)
            {
                double[] r = new double[x.Length - h.Length];
                double sum_Input = 0;//N阶输入信号和
                //double sum_h = 0;//系统函数和
                double average_Input = 0.0;//N阶输入信号均值
                //计算系统函数h各元素之和
                //for (int m = 0; m < h.Length; m++)
                //    sum_h = sum_h + h[m];
                //滤除直流偏置后求卷积
                for (int i = 0; i < x.Length - h.Length; i++)
                {
                    sum_Input = 0;

                    for (int j = 0; j < h.Length / 2; j++)
                    {
                        r[i] = r[i] + (x[i + j] + x[i + h.Length - 1 - j]) * h[h.Length - j - 1];
                        sum_Input = sum_Input + x[i + j] + x[i + h.Length - 1 - j];
                    }
                    if (h.Length % 2 == 1)
                    {
                        r[i] = r[i] + x[i + h.Length / 2] * h[h.Length / 2];
                        sum_Input = sum_Input + x[i + h.Length / 2];
                    }
                    /*
                    for (int j = 0; j < h.Length ; j++)
                    {
                        r[i] = r[i] + x[i + j] * h[h.Length - j - 1];
                        sum_Input = sum_Input + x[i + j];
                    }*/
                    average_Input = sum_Input / h.Length;
                    r[i] = r[i] - average_Input * sum_h;
                    //r[i] = r[i] - average_Input;
                }
                for (int m = 0; m < h.Length; m++)
                {
                    if (h[m] > 1)
                        sum_h = 0;
                }
                return r;
            }
            return x;
        }

        private void xcorr(double[] x, double[] y, int N, double[] r)
        {
            double sxy;
            int delay, i, j;

            for (delay = -N + 1; delay < N; delay++)
            {
                //Calculate the numerator
                sxy = 0;
                for (i = 0; i < N; i++)
                {
                    j = i + delay;
                    if ((j < 0) || (j >= 5000)) //The series are no wrapped,so the value is ignored
                        continue;
                    else
                        sxy += (x[i] * y[j]);
                }
                r[delay + N - 1] = sxy;
            }
        }

        /// <summary>
        /// 对某一信道进行带通滤波   --by zt
        /// </summary>
        /// <param name="channelIndex"></param>
        /// <param name="data"></param>
        /// <param name="is50HzFiltered"></param>
        /// <param name="isBandpassFiltered"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <param name="rateOfSample"></param>
        /// <returns></returns>
        public double[] BandpassFilter(int channelIndex, double[] data, bool is50HzFiltered, bool isBandpassFiltered, double low, double high, int rateOfSample, bool isFilterParaUpdated)
        {
            //如果不进行50Hz滤波,和带通滤波，则直接返回原始数据
            if (is50HzFiltered == false && isBandpassFiltered == false)
            {
                if (isFilterParaUpdated)
                {
                    this.is50HzFiltered = false;
                    this.isBandpassFiltered = false;
                }
                return data;
            }
            Queue dataQueue = Queue.Synchronized(new Queue());
            dataQueue = (Queue)filterDataArrayList[channelIndex];
            //存储采集数据
            for (int i = 0; i < data.Length; i++)
            {
                //新来的数据全部入列
                dataQueue.Enqueue(data[i]);
            }
            //获取队列数据个数
            int queueCount = dataQueue.Count;
            //待滤波数据量小于滤波阶次时，不进行滤波
            if (queueCount < filterQueueDataNum)
                return data;
            isDataPrepared[channelIndex] = true;

            double[] filterSrcData = new double[queueCount];
            //读取待滤波序列，不改变队列数据存储格式
            for (int i = 0; i < queueCount; i++)
            {
                filterSrcData[i] = (double)dataQueue.Dequeue();
                //保持滤波初始队列数据量为滤波阶次
                if (i >= queueCount - filterQueueDataNum)
                    dataQueue.Enqueue(filterSrcData[i]);
            }

            int n, band, wn;
            double fl, fh, fs;
            band = 3;//bandpass
            n = filterQueueDataNum;//滤波阶次
            fs = rateOfSample;
            wn = 5;//hamming
            //wn = 7;//kaiser
            //fl = low / fs;
            //fh = high / fs;
            //若滤波参数修改，则更新系统函数h
            if (this.is50HzFiltered != is50HzFiltered || this.isBandpassFiltered != isBandpassFiltered || this.low != low || this.high != high)
            {
                sum_h = ImpulseResponse(n, band, low, high, isBandpassFiltered, is50HzFiltered, wn, rateOfSample, this.h);//若有带通滤波
                this.is50HzFiltered = is50HzFiltered;
                this.isBandpassFiltered = isBandpassFiltered;
                this.low = low;
                this.high = high;
            }
            double[] filterData = new double[filterSrcData.Length];
            //double sum = 0;
            //for (int k = 0; k < h.Length; k++)
            //{
            //    sum = sum + h[k];
            //}
            //double[] reverse = new double[n];
            //double[] variable = new double[n];
            //for (int i = 0; i < n;i++ ){
            //    reverse[i] = filterData[i + n];
            //    filterData[i] = reverse[i];

            //}
            bool IIR_needed = is50HzFiltered && (isBandpassFiltered == false | (isBandpassFiltered == true && low < 50 && high > 50));

            filterSrcData = deleteDC(filterSrcData);
            filterSrcData = NotchFilter(filterSrcData, IIR_needed, rateOfSample);//50Hz陷波

            filterData = xcorr(filterSrcData, this.h);
            //if (is50HzFiltered == true && isBandpassFiltered == false)
            //{
            //    for (int k = 0;  k < filterData.Length; k++)
            //    {
            //        filterData[k] = data[k] - filterData[k];
            //    }
            //}
            ///返回滤波后的数据
            return filterData;
        }

        /// <summary>
        /// 回放模式下滤波
        /// </summary>
        /// <param name="channelIndex"></param>
        /// <param name="data"></param>
        /// <param name="is50HzFiltered"></param>
        /// <param name="isBandpassFiltered"></param>
        /// <param name="low"></param>
        /// <param name="high"></param>
        /// <param name="rateOfSample"></param>
        /// <returns></returns>
        public double[] BandpassFilterReplay(int channelIndex, double[] data, bool is50HzFiltered, bool isBandpassFiltered, double low, double high, int rateOfSample)
        {
            //如果不进行50Hz滤波,和带通滤波，则直接返回原始数据
            if (is50HzFiltered == false && isBandpassFiltered == false)
            {
                this.is50HzFiltered = is50HzFiltered;
                this.isBandpassFiltered = isBandpassFiltered;
                return data;
            }

            isDataPrepared[channelIndex] = true;

            double[] filterSrcData = data;

            int n, band, wn;
            double fl, fh, fs;
            band = 3;//bandpass
            n = filterQueueDataNum;//滤波阶次
            fs = rateOfSample;
            wn = 5;//hamming
            //wn = 7;//kaiser
            //fl = low / fs;
            //fh = high / fs;
            //若滤波参数修改，则更新系统函数h
            if (this.is50HzFiltered != is50HzFiltered || this.isBandpassFiltered != isBandpassFiltered || this.low != low || this.high != high)
            {
                sum_h = ImpulseResponse(n, band, low, high, isBandpassFiltered, is50HzFiltered, wn, rateOfSample, this.h);//若有带通滤波

                if (double.IsNaN(sum_h))
                {
                    var a = sum_h;
                }

                this.is50HzFiltered = is50HzFiltered;
                this.isBandpassFiltered = isBandpassFiltered;
                this.low = low;
                this.high = high;
            }
            double[] filterData = new double[filterSrcData.Length];

            bool IIR_needed = is50HzFiltered && (isBandpassFiltered == false | (isBandpassFiltered == true && low < 50 && high > 50));

            //        MWNumericArray arg0 = (MWNumericArray)filterSrcData;
            //        MWNumericArray arg1 = (MWNumericArray)this.h;
            //         MWNumericArray order = filterQueueDataNum ;
            //       MWArray[] inPSD = new MWArray[] { arg0, arg1, order };
            //         MWArray[] outPSD = new MWArray[1];

            //        neurotechFilter bandpassClass = new neurotechFilter();
            //        bandpassClass.neurotech_bandpass(1,ref outPSD,inPSD);
            //filterData = (double[])(((MWNumericArray)outPSD).ToVector());

            //      MWNumericArray mwarray = (MWNumericArray)outPSD[0].Clone();
            //       Array array = mwarray.ToVector(MWArrayComponent.Real);
            //       filterData = (double[])array;

            filterData = deleteDC(filterSrcData);
            filterData = NotchFilter(filterData, IIR_needed, rateOfSample);//50Hz陷波

            filterData = xcorr(filterData, this.h);

            return filterData;
        }

        private double[] deleteDC(double[] eegData)
        {
            double meanValue = 0;
            for (int i = 0; i < eegData.Length; i++)
            {
                meanValue = meanValue + eegData[i] / eegData.Length;
            }
            for (int j = 0; j < eegData.Length; j++)
            {
                eegData[j] = eegData[j] - meanValue;
            }
            return eegData;
        }

        public double[] BandStopFitNotchFilter(double[] filterSrcData, bool IIR_needed, double rateOfSample)
        {
            DateTime dt = DateTime.Now;
            ///返回滤波后的数据
            if (IIR_needed == false)
                return filterSrcData;
            double[] data = new double[filterSrcData.Length];

            double[,] hd_matrix = new double[13, 6]{{1.000000000000000,  -1.620080220369955,   1.000000000000000 ,  1.000000000000000 , -1.555606384120178 ,  0.988595595349427},
                        {1.000000000000000 , -1.620080220369955 ,  1.000000000000000  , 1.000000000000000 , -1.660781546097076 ,  0.989908766697625},
                        {1.000000000000000 , -1.620080220369955  , 1.000000000000000 ,  1.000000000000000 , -1.541983878704310  , 0.966918349724773},
                         {1.000000000000000 , -1.620080220369955 ,  1.000000000000000 ,  1.000000000000000  ,-1.641939797625511  , 0.970485341041992 },
                          { 1.000000000000000,  -1.620080220369955 ,  1.000000000000000 ,  1.000000000000000 , -1.533386330132083  , 0.947838969285140 },
                          {  1.000000000000000  ,-1.620080220369955  , 1.000000000000000  , 1.000000000000000 , -1.622061606705478   ,0.952782598303351},
                          { 1.000000000000000  ,-1.620080220369955 ,  1.000000000000000  , 1.000000000000000 , -1.530129003781416 ,  0.932420819124285},
                          { 1.000000000000000 , -1.620080220369955 ,  1.000000000000000 ,  1.000000000000000,  -1.602043853405186 ,  0.937597117638932},
                          { 1.000000000000000 , -1.620080220369955 ,  1.000000000000000,   1.000000000000000,  -1.532165944956283 ,  0.921413810860788},
                          {1.000000000000000  ,-1.620080220369955 ,  1.000000000000000  , 1.000000000000000,  -1.582808473440971  , 0.925661890637070 },
                          { 1.000000000000000  ,-1.620080220369955 ,  1.000000000000000   ,1.000000000000000,  -1.539139292711514,   0.915244581851580},
                          {  1.000000000000000  ,-1.620080220369955 ,  1.000000000000000  , 1.000000000000000 , -1.565296223568029  , 0.917628712414054},
                          {  1.000000000000000 , -1.620080220369955 ,  1.000000000000000  , 1.000000000000000 , -1.550443455101018 ,  0.914032941834158}
                         };
            double[] hd_Scale = new double[13] { 0.993638782628625, 0.993638782628625, 0.983483021968974, 0.983483021968974, 0.974487854767445, 0.974487854767445, 0.967073377446650, 0.967073377446650, 0.961557936128700, 0.961557936128700, 0.958162482187062, 0.958162482187062, 0.957016470917079 };

            for (int i = 0; i < hd_Scale.Length; i++)
            {
                for (int k = 0; k < data.Length; k++)
                {
                    data[k] = filterSrcData[k];
                }
                filterSrcData[0] = hd_Scale[i] * (hd_matrix[i, 0] * data[0]);
                filterSrcData[1] = hd_Scale[i] * (hd_matrix[i, 0] * data[1] + hd_matrix[i, 1] * data[0]) - hd_matrix[i, 4] * filterSrcData[0];
                for (int j = 2; j < filterSrcData.Length; j++)
                {
                    filterSrcData[j] = hd_Scale[i] * (hd_matrix[i, 0] * data[j] + hd_matrix[i, 1] * data[j - 1] + hd_matrix[i, 2] * data[j - 2]) - hd_matrix[i, 4] * filterSrcData[j - 1] - hd_matrix[i, 5] * filterSrcData[j - 2];
                }
            }
            Console.WriteLine("IIR filter time：{0}ms\n", DateTime.Now.Subtract(dt).Milliseconds);
            return filterSrcData;
        }

        public double[] NotchFilter(double[] filterData, bool IIR_needed, double rateOfSample)
        {
            DateTime dt = DateTime.Now;
            ///返回滤波后的数据
            if (IIR_needed == false)
                return filterData;
            double[] mydata = new double[filterData.Length];
            double[] ifreq = new double[1] { 50 };//陷波器中心频率
            // double[] ifreq = new double[1] { 50 * 500 / rateOfSample };//陷波器中心频率
            double sfreq = rateOfSample;//采样率
            double FreqWidth = 7.5;//
            //brain storm 计算公式
            /* double delta = FreqWidth / 2;
             //% Pole radius
             double  r = 1 - 2 * (delta * Math.PI / sfreq);
             double theta = 2 * Math.PI * ifreq / sfreq;
             double  B0 = Math.Abs(1 - 2 * r * Math.Cos(theta) + r *r) / (2 * Math.Abs(1 - Math.Cos(theta)));
            // double B0 = 1;
             double[] hd_matrix = new double[6] { 1.000000000000000, -2 * Math.Cos(theta),  1, 1.000000000000000, -2 * r * Math.Cos(theta), r*r };*/

            //matlab工具箱陷波器设计
            double Bw = FreqWidth / (sfreq / 2) * Math.PI;
            double Gb = Math.Pow(10, -1.0 / 20);
            double beta = (Math.Sqrt(1.0 - Gb * Gb) / Gb) * Math.Tan(Bw / 2);
            double B0 = 1.0 / (1 + beta);
            double[] w0 = new double[ifreq.Length];
            double[,] hd_matrix = new double[w0.Length, 6];

            for (int i = 0; i < w0.Length; i++)
            {
                w0[i] = ifreq[i] / (sfreq / 2) * Math.PI;

                hd_matrix[i, 0] = 1;
                hd_matrix[i, 1] = -2 * Math.Cos(w0[i]);
                hd_matrix[i, 2] = 1;
                hd_matrix[i, 3] = 1;
                hd_matrix[i, 4] = -2 * B0 * Math.Cos(w0[i]);
                hd_matrix[i, 5] = (2 * B0 - 1);
            }

            //50Hz单独陷波次数设置
            int IIR_times = 2;
            for (int m = 0; m < IIR_times; m++)
            {
                for (int n = 0; n < w0.Length; n++)
                {
                    for (int k = 0; k < mydata.Length; k++)
                    {
                        mydata[k] = filterData[k];
                    }
                    filterData[0] = B0 * (hd_matrix[n, 0] * mydata[0]);
                    filterData[1] = B0 * (hd_matrix[n, 0] * mydata[1] + hd_matrix[n, 1] * mydata[0]) - hd_matrix[n, 4] * filterData[0];
                    for (int j = 2; j < filterData.Length; j++)
                    {
                        filterData[j] = B0 * (hd_matrix[n, 0] * mydata[j] + hd_matrix[n, 1] * mydata[j - 1] + hd_matrix[n, 2] * mydata[j - 2]) - hd_matrix[n, 4] * filterData[j - 1] - hd_matrix[n, 5] * filterData[j - 2];
                    }
                }
            }
            Console.WriteLine("IIR filter time：{0}ms\n", DateTime.Now.Subtract(dt).Milliseconds);
            return filterData;
        }

        public double[] NotchFilter(double[] filterData, bool IIR_needed)
        {
            DateTime dt = DateTime.Now;
            ///返回滤波后的数据
            if (IIR_needed == false)
                return filterData;
            double[] mydata = new double[filterData.Length];
            double[] hd_matrix = new double[6] { 1.000000000000000, -1.618033988749895, 1.000000000000000, 1.000000000000000, -1.294427190999916, 0.640000000000000 };
            //50Hz单独陷波次数设置
            int IIR_times = 3;
            for (int m = 0; m < IIR_times; m++)
            {
                for (int k = 0; k < mydata.Length; k++)
                {
                    mydata[k] = filterData[k];
                }
                filterData[0] = hd_matrix[0] * mydata[0];
                filterData[1] = (hd_matrix[0] * mydata[1] + hd_matrix[1] * mydata[0]) - hd_matrix[4] * filterData[0];
                for (int j = 2; j < filterData.Length; j++)
                {
                    filterData[j] = (hd_matrix[0] * mydata[j] + hd_matrix[1] * mydata[j - 1] + hd_matrix[2] * mydata[j - 2]) - hd_matrix[4] * filterData[j - 1] - hd_matrix[5] * filterData[j - 2];
                }
            }
            Console.WriteLine("IIR filter time：{0}ms\n", DateTime.Now.Subtract(dt).Milliseconds);
            return filterData;
        }

        public double[] RCFilter(int channelIndex, double[] data, double timeConstant, double fs, bool isSecondOrderRC, double lowPassFrequency, bool isSecondOrderRCLowPass)
        {
            double[] RCFilteredData = new double[data.Length];
            if (timeConstant == 0)
                return RCFilter_LowPass(channelIndex, data, lowPassFrequency, fs, isSecondOrderRCLowPass);
            else if (lowPassFrequency == 0)
                return RCFilter_HighPass(channelIndex, data, timeConstant, fs, isSecondOrderRC);
            else
            {
                if (isSecondOrderRC == false)
                {
                    double T = 1 / fs;//采样周期
                    double a1 = timeConstant / T;
                    double RC2 = 1 / (2 * Math.PI * lowPassFrequency);
                    double a2 = RC2 / T;
                    if (isRCFilteredStartedBP[channelIndex] == false)
                    {
                        //初始化RC滤波值
                        isRCFilteredStartedBP[channelIndex] = true;
                        lastOriginalDataBP[channelIndex] = data[0];
                        lastFilteredDataBP[channelIndex] = data[0];
                        RCFilteredData[0] = data[0];
                        for (int i = 1; i < data.Length; i++)
                        {
                            RCFilteredData[i] = 1 / ((1 + a1) * (1 + a2)) * (((1 + a1) * a2 + (1 + a2) * a1) * lastFilteredDataBP[channelIndex] - a1 * a2 * lastFilteredData2BP[channelIndex] + a1 * data[i] - a1 * lastOriginalDataBP[channelIndex]);
                            lastOriginalData2BP[channelIndex] = lastOriginalDataBP[channelIndex];
                            lastFilteredData2BP[channelIndex] = lastFilteredDataBP[channelIndex];
                            lastOriginalDataBP[channelIndex] = data[i];
                            lastFilteredDataBP[channelIndex] = RCFilteredData[i];
                        }
                    }
                    else
                    {
                        //RC滤波
                        for (int i = 0; i < data.Length; i++)
                        {
                            RCFilteredData[i] = 1 / ((1 + a1) * (1 + a2)) * (((1 + a1) * a2 + (1 + a2) * a1) * lastFilteredDataBP[channelIndex] - a1 * a2 * lastFilteredData2BP[channelIndex] + a1 * data[i] - a1 * lastOriginalDataBP[channelIndex]);
                            lastOriginalData2BP[channelIndex] = lastOriginalDataBP[channelIndex];
                            lastFilteredData2BP[channelIndex] = lastFilteredDataBP[channelIndex];
                            lastOriginalDataBP[channelIndex] = data[i];
                            lastFilteredDataBP[channelIndex] = RCFilteredData[i];
                        }
                    }
                }
                else
                {
                    double T = 1 / fs;
                    double a = timeConstant / T;
                    double xi = 1 / (1 + 3 * a + a * a);
                    if (isRCFilteredStarted[channelIndex] == false)
                    {
                        //初始化RC滤波值
                        isRCFilteredStarted[channelIndex] = true;
                        lastOriginalData2[channelIndex] = data[0];
                        lastFilteredData2[channelIndex] = data[0];
                        lastOriginalData[channelIndex] = data[1];
                        lastFilteredData[channelIndex] = data[1];
                        RCFilteredData[0] = data[1];
                        for (int i = 2; i < data.Length; i++)
                        {
                            RCFilteredData[i] = xi * ((3 * a + 2 * a * a) * lastFilteredData[channelIndex] - (a * a) * lastFilteredData2[channelIndex] + (a * a) * (data[i] - 2 * lastOriginalData[channelIndex] + lastOriginalData2[channelIndex]));
                            lastOriginalData2[channelIndex] = lastOriginalData[channelIndex];
                            lastFilteredData2[channelIndex] = lastFilteredData[channelIndex];
                            lastOriginalData[channelIndex] = data[i];
                            lastFilteredData[channelIndex] = RCFilteredData[i];
                        }
                    }
                    else
                    {
                        //RC滤波
                        for (int i = 0; i < data.Length; i++)
                        {
                            RCFilteredData[i] = xi * ((3 * a + 2 * a * a) * lastFilteredData[channelIndex] - (a * a) * lastFilteredData2[channelIndex] + (a * a) * (data[i] - 2 * lastOriginalData[channelIndex] + lastOriginalData2[channelIndex]));
                            lastOriginalData2[channelIndex] = lastOriginalData[channelIndex];
                            lastFilteredData2[channelIndex] = lastFilteredData[channelIndex];
                            lastOriginalData[channelIndex] = data[i];
                            lastFilteredData[channelIndex] = RCFilteredData[i];
                        }
                    }
                }
            }
            return RCFilteredData;
        }

        /// <summary>
        /// RC滤波器
        /// </summary>
        /// <param name="channelIndex">通道号</param>
        /// <param name="data">输入数据</param>
        /// <param name="timeConstant">时间常数</param>
        /// <param name="fs">采样率</param>
        /// <param name="isSecondOrderRC">是否进行高通二阶滤波</param>
        /// <returns></returns>
        public double[] RCFilter_HighPass(int channelIndex, double[] data, double timeConstant, double fs, bool isSecondOrderRC)
        {
            double[] RCFilteredData = new double[data.Length];
            /*********************************************
             data为输入数据，RCFilteredData为滤波后输出值
             RCFilteredData[n]=a*(RCFilteredData[n-1]+data[n]-data[n-1])
             a为与RC值相关的一个参数，其值决定新采样值在本次滤波结果中所占的权重
             其值通常远小于1，a=RC/(RC+T),T为采样周期
             截止频率fl=1/(2*pi*RC)
             *********************************************/
            if (timeConstant == 0)
                return data;

            if (isSecondOrderRC == false)
            {
                double T = 1 / fs;//采样周期
                double a = timeConstant / (timeConstant + T);
                if (isRCFilteredStarted[channelIndex] == false)
                {
                    //初始化RC滤波值
                    isRCFilteredStarted[channelIndex] = true;
                    lastOriginalData[channelIndex] = data[0];
                    lastFilteredData[channelIndex] = data[0];
                    RCFilteredData[0] = data[0];
                    for (int i = 1; i < data.Length; i++)
                    {
                        RCFilteredData[i] = a * (lastFilteredData[channelIndex] + data[i] - lastOriginalData[channelIndex]);
                        lastOriginalData[channelIndex] = data[i];
                        lastFilteredData[channelIndex] = RCFilteredData[i];
                    }
                }
                else
                {
                    //RC滤波
                    for (int i = 0; i < data.Length; i++)
                    {
                        RCFilteredData[i] = a * (lastFilteredData[channelIndex] + data[i] - lastOriginalData[channelIndex]);
                        lastOriginalData[channelIndex] = data[i];
                        lastFilteredData[channelIndex] = RCFilteredData[i];
                    }
                }
            }
            else
            {
                double T = 1 / fs;
                double a = timeConstant / T;
                double xi = 1 / (1 + 3 * a + a * a);
                if (isRCFilteredStarted[channelIndex] == false)
                {
                    //初始化RC滤波值
                    isRCFilteredStarted[channelIndex] = true;
                    lastOriginalData2[channelIndex] = data[0];
                    lastFilteredData2[channelIndex] = data[0];
                    lastOriginalData[channelIndex] = data[1];
                    lastFilteredData[channelIndex] = data[1];
                    RCFilteredData[0] = data[1];
                    for (int i = 2; i < data.Length; i++)
                    {
                        RCFilteredData[i] = xi * ((3 * a + 2 * a * a) * lastFilteredData[channelIndex] - (a * a) * lastFilteredData2[channelIndex] + (a * a) * (data[i] - 2 * lastOriginalData[channelIndex] + lastOriginalData2[channelIndex]));
                        lastOriginalData2[channelIndex] = lastOriginalData[channelIndex];
                        lastFilteredData2[channelIndex] = lastFilteredData[channelIndex];
                        lastOriginalData[channelIndex] = data[i];
                        lastFilteredData[channelIndex] = RCFilteredData[i];
                    }
                }
                else
                {
                    //RC滤波
                    for (int i = 0; i < data.Length; i++)
                    {
                        RCFilteredData[i] = xi * ((3 * a + 2 * a * a) * lastFilteredData[channelIndex] - (a * a) * lastFilteredData2[channelIndex] + (a * a) * (data[i] - 2 * lastOriginalData[channelIndex] + lastOriginalData2[channelIndex]));
                        lastOriginalData2[channelIndex] = lastOriginalData[channelIndex];
                        lastFilteredData2[channelIndex] = lastFilteredData[channelIndex];
                        lastOriginalData[channelIndex] = data[i];
                        lastFilteredData[channelIndex] = RCFilteredData[i];
                    }
                }
            }
            return RCFilteredData;
        }

        /// <summary>
        /// RC低通滤波器
        /// </summary>
        /// <param name="channelIndex">通道号</param>
        /// <param name="data">输入数据</param>
        /// <param name="lowPassFrequency">低通滤波截止频率</param>
        /// <param name="fs">采样率</param>
        /// <param name="isSecondOrderRC">而否进行二阶低通</param>
        /// <returns></returns>
        public double[] RCFilter_LowPass(int channelIndex, double[] data, double lowPassFrequency, double fs, bool isSecondOrderRC)
        {
            double[] RCFilteredData = new double[data.Length];
            /*********************************************
             data为输入数据，RCFilteredData为滤波后输出值
             RCFilteredData[n]=1/(1+a)*(a*RCFilteredData[n-1]+data[n])
             a为与RC值相关的一个参数，其值决定新采样值在本次滤波结果中所占的权重
             其值通常远小于1，a=RC/T,T为采样周期
             截止频率fh=1/(2*pi*RC)，因此，RC=1/(2*pi*fh)
             *********************************************/
            if (lowPassFrequency == 0)
                return data;
            double RC = 1 / (2 * Math.PI * lowPassFrequency);
            double T = 1 / fs;//采样周期
            double a = RC / T;
            if (isSecondOrderRC == false)
            {
                if (isRCFilteredStartedLowPass[channelIndex] == false)
                {
                    //初始化RC滤波值
                    isRCFilteredStartedLowPass[channelIndex] = true;
                    lastFilteredDataLowPass[channelIndex] = data[0];
                    RCFilteredData[0] = data[0];
                    for (int i = 1; i < data.Length; i++)
                    {
                        RCFilteredData[i] = 1 / (1 + a) * (a * lastFilteredDataLowPass[channelIndex] + data[i]);
                        lastFilteredDataLowPass[channelIndex] = RCFilteredData[i];
                    }
                }
                else
                {
                    //RC滤波
                    for (int i = 0; i < data.Length; i++)
                    {
                        RCFilteredData[i] = 1 / (1 + a) * (a * lastFilteredDataLowPass[channelIndex] + data[i]);
                        lastFilteredDataLowPass[channelIndex] = RCFilteredData[i];
                    }
                }
            }
            else
            {
                double xi = 1 / (1 + 3 * a + a * a);
                if (isRCFilteredStartedLowPass[channelIndex] == false)
                {
                    //初始化RC滤波值
                    isRCFilteredStartedLowPass[channelIndex] = true;
                    lastOriginalData2LowPass[channelIndex] = data[0];
                    lastFilteredData2LowPass[channelIndex] = data[0];
                    lastOriginalDataLowPass[channelIndex] = data[1];
                    lastFilteredDataLowPass[channelIndex] = data[1];
                    RCFilteredData[0] = data[1];
                    for (int i = 2; i < data.Length; i++)
                    {
                        RCFilteredData[i] = xi * ((3 * a + 2 * a * a) * lastFilteredDataLowPass[channelIndex] - (a * a) * lastFilteredData2LowPass[channelIndex] + data[i]);
                        lastOriginalData2LowPass[channelIndex] = lastOriginalDataLowPass[channelIndex];
                        lastFilteredData2LowPass[channelIndex] = lastFilteredDataLowPass[channelIndex];
                        lastOriginalDataLowPass[channelIndex] = data[i];
                        lastFilteredDataLowPass[channelIndex] = RCFilteredData[i];
                    }
                }
                else
                {
                    //RC滤波
                    for (int i = 0; i < data.Length; i++)
                    {
                        RCFilteredData[i] = xi * ((3 * a + 2 * a * a) * lastFilteredDataLowPass[channelIndex] - (a * a) * lastFilteredData2LowPass[channelIndex] + data[i]);
                        lastOriginalData2LowPass[channelIndex] = lastOriginalDataLowPass[channelIndex];
                        lastFilteredData2LowPass[channelIndex] = lastFilteredDataLowPass[channelIndex];
                        lastOriginalDataLowPass[channelIndex] = data[i];
                        lastFilteredDataLowPass[channelIndex] = RCFilteredData[i];
                    }
                }
            }
            return RCFilteredData;
        }

        public double[] MeanFilter(int channelIndex, double[] data)
        {
            double[] meanData = new double[data.Length];
            double meanValue = 0;
            double maxMeanValue = 0;
            double maxValueNum = 0;
            double minMeanValue = 0;
            double minValueNum = 0;
            bool isMaxValue = true;
            double isSquareNum = 0;
            double vpp = 0;
            Queue dataQueue = Queue.Synchronized(new Queue());
            dataQueue = (Queue)filterDataArrayList[channelIndex];
            //存储采集数据
            for (int i = 0; i < data.Length; i++)
            {
                //新来的数据全部入列
                dataQueue.Enqueue(data[i]);
            }
            //获取队列数据个数
            int queueCount = dataQueue.Count;
            //待滤波数据量小于滤波阶次时，不进行滤波
            if (queueCount < filterQueueDataNum)
                return data;
            double[] filterSrcData = new double[queueCount];
            //读取待滤波序列，不改变队列数据存储格式
            for (int i = 0; i < queueCount; i++)
            {
                filterSrcData[i] = (double)dataQueue.Dequeue();
                //保持滤波初始队列数据量为滤波阶次
                if (i >= queueCount - filterQueueDataNum)
                    dataQueue.Enqueue(filterSrcData[i]);
            }
            for (int i = 0; i < filterSrcData.Length; i++)
            {
                meanValue = meanValue + filterSrcData[i] / filterSrcData.Length;
            }
            for (int i = 0; i < filterSrcData.Length; i++)
            {
                if (filterSrcData[i] > meanValue)
                {
                    maxMeanValue = maxMeanValue + filterSrcData[i];
                    maxValueNum++;
                }
                else
                {
                    minMeanValue = minMeanValue + filterSrcData[i];
                    minValueNum++;
                }
            }
            if (maxValueNum > 0)
                maxMeanValue = maxMeanValue / maxValueNum;
            if (minValueNum > 0)
                minMeanValue = minMeanValue / minValueNum;
            vpp = (maxMeanValue - minMeanValue) / 2;
            for (int i = 0; i < filterSrcData.Length; i++)
            {
                if (Math.Abs(filterSrcData[i] - maxMeanValue) < vpp * 0.3 || Math.Abs(filterSrcData[i] - minMeanValue) < vpp * 0.3)
                {
                    isSquareNum++;
                }
                if (filterSrcData[i] > meanValue)
                {
                    if (Math.Abs(filterSrcData[i] - maxMeanValue) > vpp * 0.01)
                    {
                        filterSrcData[i] = maxMeanValue + (filterSrcData[i] - maxMeanValue) * 0.01;
                    }
                }
                else
                {
                    if (Math.Abs(filterSrcData[i] - minMeanValue) > vpp * 0.01)
                    {
                        filterSrcData[i] = minMeanValue + (filterSrcData[i] - minMeanValue) * 0.01;
                    }
                }
                filterSrcData[i] = filterSrcData[i] - (maxMeanValue - vpp);
            }
            for (int i = 0; i < data.Length; i++)
            {
                meanData[i] = filterSrcData[filterSrcData.Length - data.Length + i];
            }
            if (isSquareNum / filterSrcData.Length < 0.8)
            {
                return data;
            }
            else
            {
                return meanData;
            }
        }

        /// <summary>
        /// 初始化滤波参数
        /// </summary>
        public void clearFilter()
        {
            filterDataArrayList.Clear();
            //定义25个队列，对应25个通道，每个队列存放filterQueueDataNum个数据，用于滤波分析，每次采集n个数据时，放于队列最后n个位置，队列前n个位置数据删除
            for (int i = 0; i < channelNum; i++)
            {
                Queue dataQueue = Queue.Synchronized(new Queue());
                filterDataArrayList.Add(dataQueue);
            }
            h = new double[filterQueueDataNum];
            //均值限波
            meanFilterDataArrayList.Clear();//定义25个队列，对应25个通道，每个队列存放filterQueueDataNum个数据，用于滤波分析，每次采集n个数据时，放于队列最后n个位置，队列前n个位置数据删除
            for (int i = 0; i < channelNum; i++)
            {
                Queue dataQueue = Queue.Synchronized(new Queue());
                meanFilterDataArrayList.Add(dataQueue);
            }
            lastOriginalData = new double[channelNum];
            lastFilteredData = new double[channelNum];
            lastOriginalData2 = new double[channelNum];
            lastFilteredData2 = new double[channelNum];
            isRCFilteredStarted = new bool[channelNum];
            isDataPrepared = new bool[channelNum];
            numOfRCFilterData = new int[channelNum];
            for (int i = 0; i < channelNum; i++)
            {
                lastOriginalData[i] = 0.0;
                lastFilteredData[i] = 0.0;
                lastOriginalData2[i] = 0.0;
                lastFilteredData2[i] = 0.0;
                isRCFilteredStarted[i] = false;
                isDataPrepared[i] = false;
                numOfRCFilterData[i] = 0;
            }
        }

        /// <summary>
        /// 获取各通道数据准备是否完成的指示
        /// </summary>
        /// <param name="channelNum"></param>
        /// <returns></returns>
        public bool getIsDataPrepared(int channelNum)
        {
            return isDataPrepared[channelNum];
        }
    }
}