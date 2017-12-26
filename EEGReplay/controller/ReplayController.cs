using EDF;
using EEGReplay.model;
using Filter;
using Ini;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EEGReplay
{
    public class ReplayController
    {
        /// <summary>
        /// 默认配置名
        /// </summary>
        public const string DEFAULT_CONFIG_NAME = "default";

        /// <summary>
        /// 回放文件
        /// </summary>
        private EDFFile _replayFile;

        public EDFFile replayFile
        {
            get { return this._replayFile; }
        }

        /// <summary>
        /// 视频回放文件
        /// </summary>
        private String _videoFilePath;

        public String videoFilePath
        {
            set { this._videoFilePath = value; }
            get { return this._videoFilePath; }
        }

        /*
        /// <summary>
        /// 标记信息 key:标记名称 value:标记时间
        /// </summary>
        private Dictionary<String, DateTime> _markInfoDic = new Dictionary<string, DateTime>();

        public Dictionary<String, DateTime> MarkInfoDic
        {
            get
            {
                return this._markInfoDic;
            }
        }

        /// <summary>
        /// 标记信息：标记名称的有序集合
        /// </summary>
        private List<String> _markInfoList = new List<string>();

        public List<String> MarkInfoList
        {
            get
            {
                return this._markInfoList;
            }
        }
        */

        /// <summary>
        /// 标记列表
        /// </summary>
        public List<Mark> MarkList { get; } = new List<Mark>();

        /// <summary>
        /// 滤波数据
        /// </summary>
        private double[][] _filterData;

        public double[][] filterData
        {
            get { return this._filterData; }
        }

        /// <summary>
        /// 滤波器
        /// </summary>
        internal FirFilter firFilter = new FirFilter();

        /// <summary>
        /// 配置文件
        /// </summary>
        private IniFile _configFile;

        public IniFile configFile
        {
            get { return this._configFile; }
        }

        /// <summary>
        /// 当前导联名称
        /// </summary>
        private String _currentLeadConfigName;

        public String currentLeadConfigName
        {
            set
            {
                this._currentLeadConfigName = value;
            }
            get { return this._currentLeadConfigName; }
        }

        /// <summary>
        /// 当前导联配置 FPI-FPj
        /// </summary>
        public List<String> leadConfig
        {
            get { return this._leadConfigDic[_currentLeadConfigName]; }
        }

        /// <summary>
        /// 全部导联配置    格式：导联名称-导联配置
        /// </summary>
        private Dictionary<String, List<String>> _leadConfigDic = new Dictionary<string, List<string>>();

        public Dictionary<String, List<String>> leadConfigDic
        {
            get { return this._leadConfigDic; }
        }

        /// <summary>
        /// 各路信道名称    格式：索引-信道名称
        /// </summary>
        private Dictionary<int, String> _leadSource = new Dictionary<int, string>();

        public Dictionary<int, String> leadSource
        {
            get { return this._leadSource; }
        }

        /// <summary>
        /// 当前灵敏度
        /// </summary>
        private int _sensitivity;

        public int sensitivity
        {
            set
            {
                this._sensitivity = value;
                this.drawEEG(false, false);
            }
            get { return this._sensitivity; }
        }

        /// <summary>
        /// 灵敏度范围
        /// </summary>
        private List<int> _sensitivityList = new List<int>();

        public List<int> sensitivityList
        {
            get { return this._sensitivityList; }
        }

        /// <summary>
        /// 滤波低频
        /// </summary>
        private double _lowFreq;

        public double lowFreq
        {
            get { return this._lowFreq; }
            set
            {
                this._lowFreq = value;
                this.drawEEG(true, false);
            }
        }

        /// <summary>
        /// 滤波高频
        /// </summary>
        private double _highFreq;

        public double highFreq
        {
            get { return this._highFreq; }
            set
            {
                this._highFreq = value;
                this.drawEEG(true, false);
            }
        }

        /// <summary>
        /// 滤波频率-低频范围
        /// </summary>
        private List<double> _lowFreqList = new List<double>();

        public List<double> lowFreqList
        {
            get { return this._lowFreqList; }
        }

        /// <summary>
        /// 滤波频率-高频范围
        /// </summary>
        private List<double> _highFreqList = new List<double>();

        public List<double> highFreqList
        {
            get { return this._highFreqList; }
        }

        /// <summary>
        /// 当前走纸速度 mm/s
        /// </summary>
        private double _speed;

        public double speed
        {
            set
            {
                this._speed = value;
                this.drawEEG(true, true);

                this.formMain.resetScrollBar();
            }
            get { return this._speed; }
        }

        /// <summary>
        /// 走纸速度范围
        /// </summary>
        private List<double> _speedList = new List<double>();

        public List<double> speedList
        {
            get { return this._speedList; }
        }

        /// <summary>
        /// 当前倍速
        /// </summary>
        private int _rate;

        public int rate
        {
            get { return this._rate; }
            set { this._rate = value; }
        }

        /// <summary>
        /// 倍速范围
        /// </summary>
        private List<int> _rateList = new List<int>();

        public List<int> rateList
        {
            get { return this._rateList; }
        }

        /// <summary>
        /// 时间常数
        /// </summary>
        private float _timeConstant;

        public float timeConstant
        {
            get { return this._timeConstant; }
            set
            {
                this._timeConstant = value;
                this.drawEEG(true, false);
            }
        }

        /// <summary>
        /// 时间常数列表
        /// </summary>
        private List<float> _timeConstantList = new List<float>();

        public List<float> timeConstantList
        {
            get { return this._timeConstantList; }
        }

        /// <summary>
        /// 是否进行二阶RC高通滤波
        /// </summary>
        private int _isSecondOrderRC;

        public Boolean isSecondOrderRC
        {
            get { return this._isSecondOrderRC != 0; }
            set
            {
                if (value) this._isSecondOrderRC = 1;
                else this._isSecondOrderRC = 0;
            }
        }

        /// <summary>
        /// 是否开启50Hz陷波
        /// </summary>
        private int _is50Hz;

        public Boolean is50Hz
        {
            get { return this._is50Hz != 0; }
            set
            {
                if (value) this._is50Hz = 1;
                else this._is50Hz = 0;
            }
        }

        /// <summary>
        /// 是否滤波
        /// </summary>
        private int _isFilter;

        public Boolean isFilter
        {
            get { return this._isFilter != 0; }
            set
            {
                if (value) this._isFilter = 1;
                else this._isFilter = 0;
            }
        }

        /// <summary>
        /// 每秒钟有多少个点
        /// </summary>
        public int numOfSamplesPerSecond
        {
            get
            {
                if (this._replayFile == null) return 500;
                return (int)(this._replayFile.Header.Signals[0].NumberOfSamplesPerDataRecord / this._replayFile.Header.DurationOfDataRecordInSeconds);
            }
        }

        /// <summary>
        /// 当前页有多少点
        /// </summary>
        public int numOfSamplesCurrentPage
        {
            get { return (int)(this.numOfSamplesPerSecond * this.secondsPerPage); }
        }

        /// <summary>
        /// 每厘米多少像素
        /// </summary>
        private double _pixelsPerCM = 0;

        public double pixelsPerCM
        {
            set
            {
                if (value <= 0) return;
                this._pixelsPerCM = value;
                this._windowSize = (double)this.formMain.getEEGPanelWidth() / (double)this._pixelsPerCM;
            }
            get { return this._pixelsPerCM; }
        }

        /// <summary>
        /// 每页有多少厘米
        /// </summary>
        private double _windowSize = 30;

        public double windowSize
        {
            get { return this._windowSize; }
        }

        /// <summary>
        /// 每秒有多少像素
        /// </summary>
        public double pixelsPerSecond
        {
            get
            {
                return this._speed * this._pixelsPerCM / 10;
            }
        }

        /*
        /// <summary>
        /// 当前页码
        /// </summary>
        private int _page = 1;

        public int page
        {
            get { return this._page; }
            set
            {
                if (value < 1 || value > this.totalPage) return;
                this._page = value;
                this.drawEEG(true, true);

                this.formMain.resetScrollBar();
            }
        }

        /// <summary>
        /// 总页码
        /// </summary>
        public int totalPage
        {
            get
            {
                if (this._replayFile == null) return 1;
                int tmp = (int)(this.duration / this.secondsPerPage);
                if (this.duration % this.secondsPerPage <= 0.00d)
                    return tmp;
                else
                    return tmp + 1;
            }
        }
        */

        /// <summary>
        /// 当前秒数    asdd by lzy 20170703 去除原有页码概念，改用下一秒/下一页
        /// </summary>
        private int _second = 1;

        public int second
        {
            get { return this._second; }
            set
            {
                // 20171117
                {
                    if (value < 1)
                    {
                        formMain.OpenPrev();
                        return;
                    }

                    if (value > duration)
                    {
                        formMain.OpenNext();
                        return;
                    }
                }

                this._second = value;
                this.drawEEG(true, true);

                this.formMain.resetScrollBar();
            }
        }

        /// <summary>
        /// 每页有多少秒
        /// </summary>
        public double secondsPerPage
        {
            get
            {
                if (this._replayFile == null) return 1;
                return this._windowSize * 10 / this._speed;
            }
        }

        /// <summary>
        /// 总时长秒数
        /// </summary>
        public int duration
        {
            get
            {
                if (this._replayFile == null) return 1;
                return (int)Math.Ceiling(this._replayFile.Header.NumberOfDataRecords * this._replayFile.Header.DurationOfDataRecordInSeconds);
            }
        }

        private FormMain formMain;

        public ReplayController(FormMain formMain)
        {
            this.formMain = formMain;
            this._configFile = new IniFile("config.ini");
            this.init();
        }

        private void init()
        {
            if (this._configFile.configData.Count <= 0) this.defaultInit();

            this._pixelsPerCM = double.Parse(this.configFile.get("pixelsPerCM"));

            this._lowFreq = double.Parse(this.configFile.get("lowFreq"));
            this._highFreq = double.Parse(this.configFile.get("highFreq"));

            this._sensitivity = int.Parse(this.configFile.get("sensitivity"));
            String[] values = this.configFile.get("sensitivityList").Split(new char[] { ',' });
            foreach (String value in values)
            {
                this._sensitivityList.Add(int.Parse(value));
            }

            values = this.configFile.get("lowFreqList").Split(new char[] { ',' });
            foreach (String value in values)
            {
                this._lowFreqList.Add(double.Parse(value));
            }

            values = this.configFile.get("highFreqList").Split(new char[] { ',' });
            foreach (String value in values)
            {
                this._highFreqList.Add(double.Parse(value));
            }

            this._timeConstant = float.Parse(this.configFile.get("timeConstant"));

            values = this.configFile.get("timeConstantList").Split(new char[] { ',' });
            foreach (String value in values)
            {
                this._timeConstantList.Add(float.Parse(value));
            }

            this._isSecondOrderRC = int.Parse(this.configFile.get("isSecondOrderRC"));

            this._is50Hz = int.Parse(this.configFile.get("is50Hz"));

            this._isFilter = int.Parse(this.configFile.get("isFilter"));

            this._speed = double.Parse(this.configFile.get("speed"));
            values = this.configFile.get("speedList").Split(new char[] { ',' });
            foreach (String value in values)
            {
                this._speedList.Add(double.Parse(value));
            }

            this._rate = int.Parse(this.configFile.get("rate"));
            values = this.configFile.get("rateList").Split(new char[] { ',' });
            foreach (String value in values)
            {
                this._rateList.Add(int.Parse(value));
            }

            values = this.configFile.get("leadSource").Split(new char[] { ',' });
            for (int i = 0; i < values.Length; i++)
            {
                this._leadSource.Add(i + 1, values[i]);
            }

            this._currentLeadConfigName = this.configFile.get("currentLeadConfigName");
            values = this.configFile.get("leadConfigNameList").Split(new char[] { ',' });
            foreach (String leadConfigName in values)
            {
                String name = "leadConfig_" + leadConfigName;
                String[] array = this.configFile.get(name).Split(new char[] { ',' });
                List<String> tempList = new List<string>();
                foreach (String v in array)
                {
                    tempList.Add(v);
                }
                this._leadConfigDic.Add(leadConfigName, tempList);
            }
        }

        private void defaultInit()
        {
            this.configFile.configData.Add("pixelsPerCM", "30");
            this.configFile.configData.Add("lowFreq", "0.10");
            this.configFile.configData.Add("highFreq", "15");
            this.configFile.configData.Add("lowFreqList", "0.10,0.30,0.50,1.00,1.50,1.60,2.00,3.00,10.00,15.00");
            this.configFile.configData.Add("highFreqList", "15,35,70,100");
            this.configFile.configData.Add("timeConstant", "0");
            this.configFile.configData.Add("timeConstantList", "0,0.01,0.1,1,2,3");
            this.configFile.configData.Add("isSecondOrderRC", "0");
            this.configFile.configData.Add("is50Hz", "0");
            this.configFile.configData.Add("isFilter", "0");
            this.configFile.configData.Add("sensitivity", "12");
            this.configFile.configData.Add("sensitivityList", "12,25,50,100,200,800,10000,100000");
            this.configFile.configData.Add("speed", "30");
            this.configFile.configData.Add("speedList", "0.5,1,2.5,5,10,15,20,30,60");
            this.configFile.configData.Add("rate", "1");
            this.configFile.configData.Add("rateList", "1,2,4,8,16,32,64,128,256");
            this.configFile.configData.Add("leadSource", "FP1,F7,F3,T3,C3,T5,P3,O1,FP2,F8,F4,T4,C4,T6,P4,O2,Fz,Cz,Pz,Oz,A1,A2,T1,T2");
            this.configFile.configData.Add("currentLeadConfigName", DEFAULT_CONFIG_NAME);
            this.configFile.configData.Add("leadConfigNameList", DEFAULT_CONFIG_NAME);
            this.configFile.configData.Add("leadConfig_" + DEFAULT_CONFIG_NAME, "FP1-FPz,F7-FPz,F3-FPz,T3-FPz,C3-FPz,T5-FPz,P3-FPz,O1-FPz,FP2-FPz,F8-FPz,F4-FPz,T4-FPz,C4-FPz,T6-FPz,P4-FPz,O2-FPz,Fz-FPz,Oz-FPz,Pz-FPz,Oz-FPz");

            this.configFile.save();
        }

        private NeuroEDF.EDF TEDF = new NeuroEDF.EDF();

        /// <summary>
        /// 回放文件初始化
        /// </summary>
        /// <param name="filePath">edf/edfx文件路径</param>
        public void initReplayFile(String filePath)
        {
            var sw = Stopwatch.StartNew();

            /*
            TEDF.ReadFile(filePath);
            TEDF.GetListDataByTime(0, 10);

            Console.WriteLine(sw.ElapsedMilliseconds);
            */

            sw.Restart();

            this._replayFile = null;
            this._replayFile = new EDFFile();
            this._replayFile.readFile(filePath);

            Console.WriteLine(sw.ElapsedMilliseconds);

            this.edfCompatible(this._replayFile);
            this.calcAVG(this._replayFile);

            if (this._replayFile == null)
            {
                throw new Exception("initReplayFile Exception: replayFile is null");
            }

            this.initMarkInfo(filePath);

            this.second = 1;

            this.drawEEG(true, true);
        }

        /// <summary>
        /// 计算AVG
        /// </summary>
        /// <param name="replayFile"></param>
        private void calcAVG(EDFFile edfFile)
        {
            // 1.添加AVG信道
            EDFSignal signalAVG = new EDFSignal();
            signalAVG.Label = "AVG";
            signalAVG.IndexNumber = edfFile.Header.Signals.Count + 1;
            signalAVG.NumberOfSamplesPerDataRecord = edfFile.Header.Signals[0].NumberOfSamplesPerDataRecord;

            edfFile.Header.Signals.Add(signalAVG);

            // 2.添加AVG数据
            for (int i = 0; i < edfFile.MyDataRecords.Count; i++)
            {
                MyEDFDataRecord record = edfFile.MyDataRecords[i];

                float sum = 0f;
                float avg = 0f;
                for (int j = 0; j < record.Count; j++)
                {
                    sum += record[j];
                }
                avg = sum / record.Count;
                record.Add(avg);
            }
        }

        private void edfCompatible(EDFFile edfFile)
        {
            if (edfFile.isEDFXFile()) return;
            for (int i = 0; i < edfFile.Header.Signals.Count; i++)
            {
                EDFSignal signal = edfFile.Header.Signals[i];

                String label = signal.Label;
                if (label.StartsWith("EEG "))
                {
                    label = label.Replace("EEG ", "");
                }
                if (label.StartsWith("ECG "))
                {
                    label = label.Replace("ECG ", "");
                }
                if (label.EndsWith("-REF", StringComparison.InvariantCultureIgnoreCase))
                {
                    label = label.Replace("-REF", "");
                    label = label.Replace("-Ref", "");
                }

                switch (label)
                {
                    case "EKG":
                        signal.Label = "X1-Y1";
                        continue;
                    case "EMG-LS":
                        signal.Label = "X5-Y5";
                        continue;
                    case "EMG-RS":
                        signal.Label = "X6-Y6";
                        continue;
                    case "PG1":
                    case "FT9":
                        signal.Label = "T1";
                        continue;
                    case "PG2":
                    case "FT10":
                        signal.Label = "T2";
                        continue;
                    case "X1-Y1":
                    case "X5-Y5":
                    case "X6-Y6":
                        continue;
                }

                /*
                if (label.Equals("PG1") || label.Equals("FT9"))
                {
                    label = "T1";
                }
                if (label.Equals("PG2") || label.Equals("FT10"))
                {
                    label = "T2";
                }

                if (label.Equals("X1-Y1"))
                {
                    label = "EKG";
                }
                if (label.Equals("X5-Y5"))
                {
                    label = "EMG-LS";
                }
                if (label.Equals("X6-Y6"))
                {
                    label = "EMG-RS";
                }

                if (label.Equals("T1") || label.Equals("T2") || label.Equals("EKG") || label.Equals("EMG-LS") || label.Equals("EMG-RS"))
                {
                    signal.Label = label;
                    continue;
                }
                */

                Boolean isHave = false;
                string value = null;
                Dictionary<int, string>.Enumerator e = this.leadSource.GetEnumerator();
                while (e.MoveNext())
                {
                    KeyValuePair<int, string> kv = e.Current;
                    if (kv.Value.ToUpper().Equals(label.ToUpper()))
                    {
                        isHave = true;
                        value = kv.Value;
                        break;
                    }
                }

                if (isHave)
                {
                    signal.Label = value;
                    continue;
                }

                signal.Label = "ZZZ" + i;
            }
        }

        /// <summary>
        /// 初始化标记信息
        /// </summary>
        /// <param name="edfFilePath">edf/edfx文件路径</param>
        private void initMarkInfo(String edfFilePath)
        {
            /*
            this._markInfoList.Clear();
            this._markInfoDic.Clear();
            */
            this.MarkList.Clear();

            String fileNameWithoutExtension = Path.GetFileNameWithoutExtension(edfFilePath) + "_mark";
            String filePath = Path.GetDirectoryName(edfFilePath) + @"\" + fileNameWithoutExtension + ".txt";

            // 初始化保存标记方法
            SaveMark = () =>
            {
                using (var fs = new FileStream(filePath, FileMode.Create))
                using (var sw = new StreamWriter(fs))
                {
                    foreach (var mark in MarkList)
                    {
                        // 计算标记名称长度
                        var length = 30 - (Encoding.Default.GetBytes(mark.Name).Length - mark.Name.Length);

                        // 写入标记信息
                        sw.WriteLine(mark.Name.PadRight(length) + mark.DateTime.ToString("HH:mm:ss.fff  MM/dd/yyyy"));
                    }
                }
            };

            // 打开标记文件
            FileInfo file = new FileInfo(filePath);
            if (!file.Exists)
            {
                this.formMain.refreshMarkListBox();
                return;
            }

            FileStream fileStream = null;
            StreamReader reader = null;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                // reader = new StreamReader(fileStream, Encoding.UTF8);
                reader = new StreamReader(fileStream, true);
                String line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.TrimEnd();

                    // 分解成字节
                    var bytes = Encoding.Default.GetBytes(line);

                    // 前30个字节为名称
                    var mark = Encoding.Default.GetString(bytes, 0, 30).Trim();

                    // 后面为日期
                    var time = Encoding.Default.GetString(bytes, 30, bytes.Length - 30).Trim();

                    // 转换成DateTime
                    DateTime markTime;
                    {
                        // BIO[12:34:56  01/01/2017]长度为20
                        if (time.Length == 20)
                        {
                            markTime = DateTime.ParseExact(time, "HH:mm:ss  MM/dd/yyyy", CultureInfo.CurrentCulture);
                        }
                        // HM[12:34:56.789  01/01/2017]长度为24
                        else if (time.Length == 24)
                        {
                            markTime = DateTime.ParseExact(time, "HH:mm:ss.fff  MM/dd/yyyy", CultureInfo.CurrentCulture);
                        }
                        // 尝试转换
                        else
                        {
                            markTime = DateTime.Parse(time);
                        }
                    }
                    /*
                    String[] lines = line.Split('|');

                    String mark = null; // 标记信息
                    DateTime markTime;  // 标记相对时间

                    if (lines.Length == 2)
                    {
                        mark = lines[0];
                        markTime = Convert.ToDateTime(lines[1]);
                    }
                    else if (line.Length > 20) // 12:34:56  01/01/2017 长度为20
                    {
                        String markTimeString = line.Substring(line.Length - 20, 20);
                        markTime = Convert.ToDateTime(markTimeString);
                        mark = line.Substring(0, line.Length - 20).Trim();
                    }
                    else
                    {
                        continue;
                    }
                    */

                    // 加入列表
                    MarkList.Add(new Mark(mark, markTime));
                    /*
                    // 标记信息和时间都可能重复，如有重复，则添加 "_i" 用来区分
                    if (!this._markInfoDic.ContainsKey(mark))
                    {
                        this._markInfoDic.Add(mark, markTime);
                        this._markInfoList.Add(mark);
                    }
                    else
                    {
                        int i = 2;
                        String tmp = mark + "_" + i;
                        while (this._markInfoDic.ContainsKey(tmp))
                        {
                            i++;
                            tmp = mark + "_" + i;
                        }
                        mark = tmp;
                        this._markInfoDic.Add(mark, markTime);
                        this._markInfoList.Add(mark);
                    }
                    */
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("标记信息初始化异常：" + ex.Message + "\n详情：" + ex.StackTrace);
            }
            finally
            {
                try
                {
                    if (fileStream != null) fileStream.Close();
                    if (reader != null) reader.Close();
                }
                catch { }
                finally
                {
                    fileStream = null;
                    reader = null;
                }
            }

            this.formMain.refreshMarkListBox();
        }

        public Action SaveMark;

        public void Dispose()
        {
            this.saveConfig();
        }

        private void saveConfig()
        {
            this._configFile.set("pixelsPerCM", this._pixelsPerCM.ToString());
            this._configFile.set("lowFreq", this._lowFreq.ToString());
            this._configFile.set("highFreq", this._highFreq.ToString());
            this._configFile.set("timeConstant", this._timeConstant.ToString());
            this._configFile.set("isSecondOrderRC", this._isSecondOrderRC.ToString());
            this._configFile.set("is50Hz", this._is50Hz.ToString());
            this._configFile.set("isFilter", this._isFilter.ToString());
            this._configFile.set("sensitivity", this._sensitivity.ToString());
            this._configFile.set("speed", this._speed.ToString());
            this._configFile.set("rate", this._rate.ToString());
            this._configFile.set("currentLeadConfigName", this._currentLeadConfigName);

            // 保存导联名称列表 和 各个导联内容
            Dictionary<String, List<String>>.Enumerator enumerator = this._leadConfigDic.GetEnumerator();
            StringBuilder leadConfigNameList = new StringBuilder();
            StringBuilder leadConfig = new StringBuilder();
            while (enumerator.MoveNext())
            {
                KeyValuePair<String, List<String>> kvPair = enumerator.Current;

                String leadConfigName = kvPair.Key;
                leadConfigNameList.Append(leadConfigName).Append(",");
                leadConfigName = "leadConfig_" + leadConfigName;

                leadConfig.Clear();
                foreach (String s in kvPair.Value)
                {
                    leadConfig.Append(s).Append(",");
                }
                this._configFile.set(leadConfigName, leadConfig.ToString(0, leadConfig.Length - 1));
            }
            this._configFile.set("leadConfigNameList", leadConfigNameList.ToString(0, leadConfigNameList.Length - 1));

            this._configFile.save();
        }

        /// <summary>
        /// 绘制波形图
        /// </summary>
        /// <param name="isFiltering">是否滤波</param>
        /// <param name="isUpdateToolBarInfo">是否刷新状态栏信息</param>
        public void drawEEG(Boolean isFiltering, Boolean isUpdateToolBarInfo)
        {
            if (this._replayFile == null) return;
            if (isFiltering) this.updateFilterDataWithThreadPool();
            if (isUpdateToolBarInfo) this.formMain.updateToolBarInfo();

            this.formMain.drawEEG();
        }

        private void updateFilterDataWithThreadPool()
        {
            if (this._replayFile == null) return;
            if (this._replayFile.MyDataRecords.Count <= 0) return;

            this._filterData = new double[this._replayFile.Header.Signals.Count][];
            List<ManualResetEvent> mreList = new List<ManualResetEvent>();

            #region edfx格式的滤波

            if (this._replayFile.MyDataRecords.Count > 0)
            {
                for (int i = 0; i < this._replayFile.Header.Signals.Count; i++)
                {
                    #region 多线程滤波会有问题，暂时用单线程滤

                    /*
                    ManualResetEvent mre = new ManualResetEvent(false);
                    mreList.Add(mre);
                    ThreadPool.QueueUserWorkItem(delegate (Object obj)
                    {
                        if (obj is Object[])
                        {
                            ManualResetEvent m = (ManualResetEvent)((Object[])obj)[0];
                            int index = (int)((Object[])obj)[1];

                            // 该信道总共有多少个SAMPLE点 线程内局部变量，避免线程错乱
                            int tempCount = this._replayFile.Header.NumberOfDataRecords * this._replayFile.Header.Signals[index].NumberOfSamplesPerDataRecord;

                            // 每秒有多少个SAMPLE
                            double numbers_of_samples_perSeconds = (this._replayFile.Header.Signals[0].NumberOfSamplesPerDataRecord / this._replayFile.Header.DurationOfDataRecordInSeconds);
                            // 当前页该信道有多少个采样点（如果满的话）
                            double samplesOfCurrentPage = (numbers_of_samples_perSeconds * windowSize / speed * 10);
                            int currentPosition = 1 + (int)((this.page - 1) * samplesOfCurrentPage);

                            double samples_remaind = 0; // 剩下要滤波的SAMPLE点的个数
                            if (samplesOfCurrentPage + currentPosition - 1 > tempCount)
                            {
                                samples_remaind = tempCount - currentPosition; // 若剩下的点不足一页，则samples_remaind等于剩下的点数
                            }
                            else
                            {
                                samples_remaind = (numbers_of_samples_perSeconds * windowSize / speed * 10); // 若剩下的点大于等于一页，则samples_remaind为一页的点数
                            }

                            // EKG不加点 20170626
                            if ((this.page < this.totalPage) &&
                                (this._replayFile.Header.Signals[index].Label.EndsWith("REF", StringComparison.InvariantCultureIgnoreCase) == false))
                            {
                                samples_remaind += 151;   // 非最后一页需要多画一个点       -- by lxl
                            }

                            filterData[index] = new double[(int)samples_remaind];
                            for (int k = 0; k < (int)samples_remaind; k++)
                            {
                                // if (index + 1 > edfFile.MyDataRecords[k + signalCurrentPosition - 1].Count) break;
                                double value = Convert.ToDouble(this._replayFile.MyDataRecords[k + currentPosition - 1][index]);
                                filterData[index][k] = value;
                            }

                            // 调用滤波算法
                            // EDF不过滤EKG 20170614
                            if (this._replayFile.isEDFXFile() ||
                                this._replayFile.Header.Signals[index].Label.EndsWith("REF", StringComparison.InvariantCultureIgnoreCase))
                            {
                                filterData[index] = firFilter.RCFilter(index, filterData[index], this.timeConstant, this.numOfSamplesPerSecond, this.isSecondOrderRC, 0, false);
                                filterData[index] = firFilter.BandpassFilterReplay(index, filterData[index], this.is50Hz, this.isFilter, this.lowFreq, this.highFreq, this.numOfSamplesPerSecond);
                            }

                            m.Set();
                        }
                    }, new Object[] { mre, i });
                    */

                    #endregion 多线程滤波会有问题，暂时用单线程滤

                    #region 单线程滤波

                    // 该信道总共有多少个SAMPLE点 线程内局部变量，避免线程错乱
                    int tempCount = this._replayFile.Header.NumberOfDataRecords * this._replayFile.Header.Signals[i].NumberOfSamplesPerDataRecord;

                    // 每秒有多少个SAMPLE
                    double numbers_of_samples_perSeconds = (this._replayFile.Header.Signals[i].NumberOfSamplesPerDataRecord / this._replayFile.Header.DurationOfDataRecordInSeconds);
                    // 当前页该信道有多少个采样点（如果满的话）
                    double samplesOfCurrentPage = (numbers_of_samples_perSeconds * windowSize / speed * 10);
                    int currentPosition = 1 + (int)((this.second - 1) * numbers_of_samples_perSeconds);

                    double samples_remaind = 0; // 剩下要滤波的SAMPLE点的个数
                    if (samplesOfCurrentPage + currentPosition - 1 > tempCount)
                    {
                        samples_remaind = tempCount - currentPosition; // 若剩下的点不足一页，则samples_remaind等于剩下的点数
                    }
                    else
                    {
                        samples_remaind = (numbers_of_samples_perSeconds * this.secondsPerPage); // 若剩下的点大于等于一页，则samples_remaind为一页的点数
                    }

                    /*
                    // EKG不加点 20170626
                    if ((this.duration - this.second) > (this.secondsPerPage + 151D / numbers_of_samples_perSeconds)
                        && (this.leadSource.ContainsValue(this._replayFile.Header.Signals[i].Label) == false))
                    {
                        samples_remaind += 151;
                    }
                     * */

                    // 所有通道都滤波。需要滤波的通道需要多拿151个点
                    samples_remaind += 151;

                    filterData[i] = new double[(int)samples_remaind];

                    if (this._replayFile.Header.Signals[i].Label.StartsWith("ZZZ"))
                    {
                        continue;
                    }

                    for (int k = 0; k < (int)samples_remaind; k++)
                    {
                        if ((k + currentPosition) > this._replayFile.MyDataRecords.Count) break;
                        if (i + 1 > this._replayFile.MyDataRecords[k + currentPosition - 1].Count) break;

                        double value = Convert.ToDouble(this._replayFile.MyDataRecords[k + currentPosition - 1][i]);
                        filterData[i][k] = value;
                    }

                    /*
                    if (!this._replayFile.Header.Signals[i].Label.Contains("EKG") && !(this._replayFile.Header.Signals[i].Label.IndexOfAny(new Char[] {'X', 'Y'}) >= 0))
                    {
                        filterData[i] = firFilter.RCFilter(i, filterData[i], this.timeConstant, this.numOfSamplesPerSecond, this.isSecondOrderRC, 0, false);
                        filterData[i] = firFilter.BandpassFilterReplay(i, filterData[i], this.is50Hz, this.isFilter, this.lowFreq, this.highFreq, this.numOfSamplesPerSecond);
                    } */

                    // EKG通道 固定1.6-100的滤波
                    if (this._replayFile.Header.Signals[i].Label.Contains("EKG") || this._replayFile.Header.Signals[i].Label.Contains("X1-Y1"))
                    {
                        filterData[i] = firFilter.RCFilter(i, filterData[i], this.timeConstant, this.numOfSamplesPerSecond, this.isSecondOrderRC, 0, false);
                        filterData[i] = firFilter.BandpassFilterReplay(i, filterData[i], this.is50Hz, this.isFilter, 1.6D, 70D, this.numOfSamplesPerSecond);
                    }
                    // EMG-LS EMG-RS通道 固定1.6-100的滤波
                    else if ((this._replayFile.Header.Signals[i].Label.Contains("EMG")) || (this._replayFile.Header.Signals[i].Label.IndexOfAny(new Char[] { 'X', 'Y' }) >= 0))
                    {
                        filterData[i] = firFilter.RCFilter(i, filterData[i], this.timeConstant, this.numOfSamplesPerSecond, this.isSecondOrderRC, 0, false);
                        filterData[i] = firFilter.BandpassFilterReplay(i, filterData[i], this.is50Hz, this.isFilter, 1.6D, 70D, this.numOfSamplesPerSecond);
                    }
                    // 其他通道 正常滤波
                    else
                    {
                        filterData[i] = firFilter.RCFilter(i, filterData[i], this.timeConstant, this.numOfSamplesPerSecond, this.isSecondOrderRC, 0, false);
                        filterData[i] = firFilter.BandpassFilterReplay(i, filterData[i], this.is50Hz, this.isFilter, this.lowFreq, this.highFreq, this.numOfSamplesPerSecond);
                    }

                    #endregion 单线程滤波
                }

                foreach (ManualResetEvent tmp in mreList)
                {
                    tmp.WaitOne();
                }
            }

            #endregion edfx格式的滤波

            /*
            this._filterData = new double[this._replayFile.Header.Signals.Count][];
            List<ManualResetEvent> mreList = new List<ManualResetEvent>();

            for (int i = 0; i < this._replayFile.Header.Signals.Count; i++)
            {
                ManualResetEvent mre = new ManualResetEvent(false);
                mreList.Add(mre);

                ThreadPool.QueueUserWorkItem(delegate (Object obj)
                {
                    Object[] param = (Object[])obj;
                    ManualResetEvent m = (ManualResetEvent)param[0];
                    int index = (int)param[1];

                    // 该信道总共有多少个SAMPLE点
                    int tempCount = this._replayFile.Header.NumberOfDataRecords * this._replayFile.Header.Signals[index].NumberOfSamplesPerDataRecord;
                    // 当前位置
                    int currentPosition = (this.page - 1) * this.numOfSamplesCurrentPage + 1;
                    // 剩下要滤波的点的个数
                    int numOfFilterSimple = this.numOfSamplesCurrentPage;
                    if (currentPosition + this.numOfSamplesCurrentPage > tempCount) numOfFilterSimple = tempCount - currentPosition;

                    this.filterData[index] = new double[numOfFilterSimple];
                    for (int k = 0; k < numOfFilterSimple; k++)
                    {
                        double value = Convert.ToDouble(this._replayFile.MyDataRecords[k + currentPosition - 1][index]);
                        filterData[index][k] = value;
                    }

                    // 调用滤波算法
                    // EDF不过滤EKG 20170614
                    if (this._replayFile.isEDFXFile() ||
                        this._replayFile.Header.Signals[index].Label.Equals("EKG") == false)
                    {
                        filterData[index] = firFilter.RCFilter(index, filterData[index], this.timeConstant, this.numOfSamplesPerSecond, isSecondOrderRC, 0, false);
                        filterData[index] = firFilter.BandpassFilterReplay(index, filterData[index], this.is50Hz, this.isFilter, this.lowFreq, this.highFreq, this.numOfSamplesPerSecond);
                    }

                    m.Set();
                }, new Object[] { mre, i });
            }

            foreach (ManualResetEvent tmp in mreList)
            {
                tmp.WaitOne();
            }
            */
        }

        /// <summary>
        /// 返回当前信道的单位与uv做比较的倍率
        /// -- by lxl
        /// </summary>
        public int generateDW(int i)
        {
            if (this._replayFile.Header.Signals[i].PhysicalDimension.ToLower().Trim() == "uv")
                return 1;
            else if (this._replayFile.Header.Signals[i].PhysicalDimension.ToLower().Trim() == "mv")
                return 1000;
            else if (this._replayFile.Header.Signals[i].PhysicalDimension.ToLower().Trim() == "v")
                return 1000000;
            else return 1;
        }

        /// <summary>
        /// 计算出在屏幕上的实际位置
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <returns></returns>
        public double generateDataOnChartViaData(double data)
        {
            // 1.该数据点的值有多少厘米
            data /= this._sensitivity;

            // 2.该数据点的值有多少像素
            data *= this._pixelsPerCM;

            return data;
        }
    }
}