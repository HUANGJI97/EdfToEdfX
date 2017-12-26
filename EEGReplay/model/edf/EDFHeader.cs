using System;
using System.Collections.Generic;
using System.Text;

namespace EDF
{
    /// <summary>
    /// 头文件类
    /// </summary>
    public class EDFHeader
    {
        public static string EDFContinuous = "EDF+C";
        public static string EDFDiscontinuous = "EDF+D";
        private static string VERSION_DEFAULT = "0       ";
        private bool _isEDFPlus = false;
        public bool IsEDFPlus
        {
            get
            {
                return _isEDFPlus;
            }
        }

        public EDFHeader()
        {
            initializeEDFHeader();

        }
        public EDFHeader(bool isEDFPlus)
        {
            this._isEDFPlus = isEDFPlus;
            initializeEDFHeader();
        }

        #region 20170613 重写解析逻辑，旧方法使用char取头，如果含有中文则取值范围错误

        #region 旧方法
        /*
        public EDFHeader(char[] header)
        {
            if (header.Length != 256)
            {
                throw new ArgumentException("Header must be 256 characters");
            }
            parseHeader(header);
        }
        */
        #endregion

        public EDFHeader(byte[] header, Encoding encoding)
        {
            if (header.Length != 256)
            {
                throw new ArgumentException("Header must be 256 characters");
            }

            parseHeader(header, encoding);
        }

        #endregion

        private void initializeEDFHeader()
        {
            this.Signals = new List<EDFSignal>();
            this.Version = string.Empty;
            this._PatientInformation = new EDFLocalPatientIdentification(getFixedLengthString(string.Empty, EDFHeader.FixedLength_LocalPatientIdentification).ToCharArray());
            this._RecordingInformation = new EDFLocalRecordingIdentification(getFixedLengthString(string.Empty, EDFHeader.FixedLength_LocalRecordingIdentifiaction).ToCharArray());
            this.StartDateEDF = DateTime.MinValue.ToString("dd.MM.yy");
            this.StartTimeEDF = DateTime.MinValue.ToString("hh.mm.ss");
            this.EndTimeEDF = DateTime.MinValue;//文件结束时间  --by zt
            this.StartDate = DateTime.MinValue.ToString("yyyy-MM-dd"); //xcg
            this.NumberOfBytes = 0;
            this.NumberOfDataRecords = 0;
            this.DurationOfDataRecordInSeconds = 0;
            //this.NumberOfSignalsInDataRecord = this.Signals.Count;
            this.NumberOfSignalsInDataRecord = 20;
            this.Reserved = string.Empty;
        }
        private static int FixedLength_Version = 8;
        private string _Version = VERSION_DEFAULT;
        public string Version
        {
            get
            {
                return _Version;
            }
            set
            {
                _Version = getFixedLengthString(value, FixedLength_Version);
            }
        }

        public static int FixedLength_LocalPatientIdentification = 80;
        private EDFLocalPatientIdentification _PatientInformation;
        public EDFLocalPatientIdentification PatientIdentification
        {
            get
            {
                return _PatientInformation;
            }
            set
            {
                if (value.ToString().Length != FixedLength_LocalPatientIdentification)
                {
                    throw new FormatException("Patient Information must be " + FixedLength_LocalPatientIdentification + " characters fixed length");
                }
                _PatientInformation = value;
            }
        }

        public static int FixedLength_LocalRecordingIdentifiaction = 80;
        private EDFLocalRecordingIdentification _RecordingInformation;
        public EDFLocalRecordingIdentification RecordingIdentification
        {
            get
            {
                return _RecordingInformation;
            }
            set
            {
                if (value.ToString().Length != EDFHeader.FixedLength_LocalRecordingIdentifiaction)
                {
                    throw new FormatException("Recording Information must be " + EDFHeader.FixedLength_LocalRecordingIdentifiaction + " characters fixed length");
                }
                _RecordingInformation = value;
            }
        }

        public static int FixedLength_StartDateEDF = 8;
        public string StartDateEDF { get; set; } //删掉private

        public static int FixedLength_StartTimeEDF = 8;
        public string StartTimeEDF { get; set; } //删掉private
        public DateTime EndTimeEDF { get; set; } //添加结束时间 zt

        //以“年月日”的形式记录日期（xcg）
        public string StartDate { get; set; }

        private DateTime _StartDateTime;
        public DateTime StartDateTime
        {
            get { return _StartDateTime; }
            set
            {
                this.StartDateEDF = value.ToString("dd.MM.yy");
                this.StartTimeEDF = value.ToString("HH.mm.ss");


                //为StartDate赋值（xcg）
                this.StartDate = value.ToString("yyyy-MM-dd");

                _StartDateTime = value;
            }
        }

        public static int FixedLength_NumberOfBytes = 8;
        private string _NumberOfBytesFixedLengthString = "0";
        private int _NumberOfBytes = 0;
        public int NumberOfBytes
        {
            get
            {
                return _NumberOfBytes;
            }
            set
            {
                _NumberOfBytes = value;
                _NumberOfBytesFixedLengthString = getFixedLengthString(Convert.ToString(value), FixedLength_NumberOfBytes);
            }
        }

        public static int FixedLength_NumberOfDataRecords = 8;
        private string _NumberOfDataRecordsFixedLengthString = "0";
        private int _NumberOfDataRecords = 0;
        public int NumberOfDataRecords
        {
            get
            {
                return _NumberOfDataRecords;
            }
            set
            {
                _NumberOfDataRecords = value;
                _NumberOfDataRecordsFixedLengthString = getFixedLengthString(Convert.ToString(value), FixedLength_NumberOfDataRecords);
            }
        }


        public static int FixedLength_DuraitonOfDataRecordInSeconds = 8;
        private string _DurationOfDataRecordInSecondsFixedLengthString = "0";
        private double _DurationOfDataRecordInSeconds = 0; //修改int为double
        public double DurationOfDataRecordInSeconds
        {
            get
            {
                return _DurationOfDataRecordInSeconds;
            }
            set
            {
                _DurationOfDataRecordInSeconds = value;
                _DurationOfDataRecordInSecondsFixedLengthString = getFixedLengthString(Convert.ToString(value), FixedLength_DuraitonOfDataRecordInSeconds);
            }
        }

        public static int FixedLength_NumberOfSignalsInDataRecord = 4;
        private string _NumberOfSignalsInDataRecordFixedLengthString = "0";
        private int _NumberOfSignalsInDataRecord = 0;
        public int NumberOfSignalsInDataRecord
        {
            get
            {
                return _NumberOfSignalsInDataRecord;
            }
            set
            {
                _NumberOfSignalsInDataRecord = value;
                _NumberOfSignalsInDataRecordFixedLengthString = getFixedLengthString(Convert.ToString(value), FixedLength_NumberOfSignalsInDataRecord);
            }
        }

        public static int FixedLength_Reserved = 44;
        private string _Reserved;
        public string Reserved
        {
            get
            {
                return _Reserved;
            }
            set
            {
                _Reserved = getFixedLengthString(value, FixedLength_Reserved);
            }
        }


        public List<EDFSignal> Signals { get; set; }
        private StringBuilder _strHeader = new StringBuilder(string.Empty);

        #region 20170613 重写解析逻辑，旧方法使用char取头，如果含有中文则取值范围错误

        #region 旧方法
        //private void parseHeader(char[] header)
        //{
        //    /**
        //     * replace nulls with space.
        //     */
        //    int i = 0;
        //    foreach (char c in header)
        //    {
        //        if (header[i] == (char)0)
        //        {
        //            header[i] = (char)32;
        //        }
        //        i++;
        //    }

        //    _strHeader.Append(header);

        //    int fileIndex = 0;

        //    char[] version = getFixedLengthCharArrayFromHeader(header, fileIndex, EDFHeader.FixedLength_Version);
        //    this.Version = new string(version);
        //    fileIndex += FixedLength_Version;

        //    char[] localPatientIdentification = getFixedLengthCharArrayFromHeader(header, fileIndex, EDFHeader.FixedLength_LocalPatientIdentification);
        //    fileIndex += EDFHeader.FixedLength_LocalPatientIdentification;

        //    char[] localRecordingIdentification = getFixedLengthCharArrayFromHeader(header, fileIndex, EDFHeader.FixedLength_LocalRecordingIdentifiaction);
        //    fileIndex += EDFHeader.FixedLength_LocalRecordingIdentifiaction;

        //    char[] startDate = getFixedLengthCharArrayFromHeader(header, fileIndex, EDFHeader.FixedLength_StartDateEDF);
        //    this.StartDateEDF = new string(startDate);
        //    fileIndex += EDFHeader.FixedLength_StartDateEDF;

        //    char[] startTime = getFixedLengthCharArrayFromHeader(header, fileIndex, EDFHeader.FixedLength_StartTimeEDF);
        //    this.StartTimeEDF = new string(startTime);
        //    fileIndex += EDFHeader.FixedLength_StartTimeEDF;

        //    char[] numberOfBytesInHeaderRow = getFixedLengthCharArrayFromHeader(header, fileIndex, EDFHeader.FixedLength_NumberOfBytes);
        //    this.NumberOfBytes = int.Parse(new string(numberOfBytesInHeaderRow).Trim());
        //    fileIndex += EDFHeader.FixedLength_NumberOfBytes;

        //    char[] reserved = getFixedLengthCharArrayFromHeader(header, fileIndex, EDFHeader.FixedLength_Reserved);
        //    if (new string(reserved).StartsWith(EDFHeader.EDFContinuous) || new string(reserved).StartsWith(EDFHeader.EDFDiscontinuous))
        //    {
        //        this._isEDFPlus = true;
        //    }
        //    this.Reserved = new string(reserved);
        //    fileIndex += EDFHeader.FixedLength_Reserved;

        //    char[] numberOfDataRecords = getFixedLengthCharArrayFromHeader(header, fileIndex, EDFHeader.FixedLength_NumberOfDataRecords);
        //    this.NumberOfDataRecords = (int.Parse(new string(numberOfDataRecords).Trim()));
        //    fileIndex += EDFHeader.FixedLength_NumberOfDataRecords;

        //    char[] durationOfDataRecord = getFixedLengthCharArrayFromHeader(header, fileIndex, EDFHeader.FixedLength_DuraitonOfDataRecordInSeconds);
        //    this.DurationOfDataRecordInSeconds = double.Parse(new string(durationOfDataRecord).Trim());
        //    fileIndex += EDFHeader.FixedLength_DuraitonOfDataRecordInSeconds;

        //    char[] numberOfSignals = getFixedLengthCharArrayFromHeader(header, fileIndex, EDFHeader.FixedLength_NumberOfSignalsInDataRecord);
        //    this.NumberOfSignalsInDataRecord = int.Parse(new string(numberOfSignals).Trim());
        //    if (this.NumberOfSignalsInDataRecord < 1 || this.NumberOfSignalsInDataRecord > 256)
        //    {
        //        throw new ArgumentException("EDF File has " + this.NumberOfSignalsInDataRecord + " Signals; Number of Signals must be >1 and <=256");
        //    }
        //    fileIndex += EDFHeader.FixedLength_NumberOfSignalsInDataRecord;

        //    this.PatientIdentification = new EDFLocalPatientIdentification(localPatientIdentification);
        //    this.RecordingIdentification = new EDFLocalRecordingIdentification(localRecordingIdentification);

        //    this.StartDateTime = DateTime.ParseExact(this.StartDateEDF + " " + this.StartTimeEDF, "dd.MM.yy HH.mm.ss", System.Globalization.CultureInfo.InvariantCulture);
        //    //if (this.IsEDFPlus)
        //    //{
        //    //    if (!this.StartDateTime.Date.Equals(this.RecordingIdentification.RecordingStartDate))
        //    //    {
        //    //        throw new ArgumentException("Header StartDateTime does not equal Header.RecordingIdentification StartDate!");
        //    //    }
        //    //    else
        //    //    {
        //    this.RecordingIdentification.RecordingStartDate = this.StartDateTime;
        //    //    }
        //    //}


        //}

        //public void parseSignals(char[] signals)
        //{
        //    this._strHeader.Append(signals);

        //    this.Signals = new List<EDFSignal>();

        //    /**
        //     * replace nulls with space.
        //     */
        //    int h = 0;
        //    foreach (char c in signals)
        //    {
        //        if (signals[h] == (char)0)
        //        {
        //            signals[h] = (char)32;
        //        }
        //        h++;
        //    }

        //    for (int i = 0; i < this.NumberOfSignalsInDataRecord; i++)
        //    {
        //        EDFSignal edf_signal = new EDFSignal();

        //        int charIndex = 0;

        //        char[] label = getFixedLengthCharArrayFromHeader(signals, (i * 16) + (this.NumberOfSignalsInDataRecord * charIndex), 16);
        //        edf_signal.Label = new string(label);
        //        charIndex += 16;

        //         if (edf_signal.Label.IndexOf("Annotations") > 0) continue;

        //        edf_signal.IndexNumber = (i + 1);

        //        char[] transducer_type = getFixedLengthCharArrayFromHeader(signals, (i * 80) + (this.NumberOfSignalsInDataRecord * charIndex), 80);
        //        edf_signal.TransducerType = new string(transducer_type);
        //        charIndex += 80;

        //        char[] physical_dimension = getFixedLengthCharArrayFromHeader(signals, (i * 8) + (this.NumberOfSignalsInDataRecord * charIndex), 8);
        //        edf_signal.PhysicalDimension = new string(physical_dimension);
        //        charIndex += 8;

        //        char[] physical_min = getFixedLengthCharArrayFromHeader(signals, (i * 8) + (this.NumberOfSignalsInDataRecord * charIndex), 8);
        //        edf_signal.PhysicalMinimum = float.Parse(new string(physical_min).Trim());
        //        charIndex += 8;

        //        char[] physical_max = getFixedLengthCharArrayFromHeader(signals, (i * 8) + (this.NumberOfSignalsInDataRecord * charIndex), 8);
        //        edf_signal.PhysicalMaximum = float.Parse(new string(physical_max).Trim());
        //        charIndex += 8;

        //        char[] digital_min = getFixedLengthCharArrayFromHeader(signals, (i * 8) + (this.NumberOfSignalsInDataRecord * charIndex), 8);
        //        edf_signal.DigitalMinimum = float.Parse(new string(digital_min).Trim());
        //        charIndex += 8;

        //        char[] digital_max = getFixedLengthCharArrayFromHeader(signals, (i * 8) + (this.NumberOfSignalsInDataRecord * charIndex), 8);
        //        edf_signal.DigitalMaximum = float.Parse(new string(digital_max).Trim());
        //        charIndex += 8;

        //        char[] prefiltering = getFixedLengthCharArrayFromHeader(signals, (i * 80) + (this.NumberOfSignalsInDataRecord * charIndex), 80);
        //        edf_signal.Prefiltering = new string(prefiltering);
        //        charIndex += 80;

        //        char[] samples_each_datarecord = getFixedLengthCharArrayFromHeader(signals, (i * 8) + (this.NumberOfSignalsInDataRecord * charIndex), 8);
        //        edf_signal.NumberOfSamplesPerDataRecord = int.Parse(new string(samples_each_datarecord).Trim());
        //        charIndex += 8;

        //        this.Signals.Add(edf_signal);

        //    }

        //}

        #endregion

        /// <summary>
        /// 解析文件头
        /// </summary>
        private void parseHeader(byte[] header, Encoding encoding)
        {
            // 将转换后的内容写入缓存
            _strHeader.Append(encoding.GetChars(header));

            // 开始解析
            int _index = 0;

            // 版本号
            byte[] version = getFixedLengthByteArrayFromHeader(header, _index, EDFHeader.FixedLength_Version);
            this.Version = new string(encoding.GetChars(version));
            _index += FixedLength_Version;

            // 病人ID
            byte[] localPatientIdentification = getFixedLengthByteArrayFromHeader(header, _index, EDFHeader.FixedLength_LocalPatientIdentification);
            _index += EDFHeader.FixedLength_LocalPatientIdentification;

            // 记录ID
            byte[] localRecordingIdentification = getFixedLengthByteArrayFromHeader(header, _index, EDFHeader.FixedLength_LocalRecordingIdentifiaction);
            _index += EDFHeader.FixedLength_LocalRecordingIdentifiaction;

            // 开始日期
            byte[] startDate = getFixedLengthByteArrayFromHeader(header, _index, EDFHeader.FixedLength_StartDateEDF);
            this.StartDateEDF = new string(encoding.GetChars(startDate));
            _index += EDFHeader.FixedLength_StartDateEDF;

            // 开始时间
            byte[] startTime = getFixedLengthByteArrayFromHeader(header, _index, EDFHeader.FixedLength_StartTimeEDF);
            this.StartTimeEDF = new string(encoding.GetChars(startTime));
            _index += EDFHeader.FixedLength_StartTimeEDF;

            // 字节数
            byte[] numberOfBytesInHeaderRow = getFixedLengthByteArrayFromHeader(header, _index, EDFHeader.FixedLength_NumberOfBytes);
            this.NumberOfBytes = int.Parse(new string(encoding.GetChars(numberOfBytesInHeaderRow)).Trim());
            _index += EDFHeader.FixedLength_NumberOfBytes;

            // 专用格式
            byte[] reserved = getFixedLengthByteArrayFromHeader(header, _index, EDFHeader.FixedLength_Reserved);
            string reservedStr = new string(encoding.GetChars(reserved));
            if (reservedStr.StartsWith(EDFHeader.EDFContinuous) || reservedStr.StartsWith(EDFHeader.EDFDiscontinuous))
            {
                this._isEDFPlus = true;
            }
            this.Reserved = reservedStr;
            _index += EDFHeader.FixedLength_Reserved;

            // 记录数
            byte[] numberOfDataRecords = getFixedLengthByteArrayFromHeader(header, _index, EDFHeader.FixedLength_NumberOfDataRecords);
            this.NumberOfDataRecords = (int.Parse(new string(encoding.GetChars(numberOfDataRecords)).Trim()));
            _index += EDFHeader.FixedLength_NumberOfDataRecords;

            // 采样间隔
            byte[] durationOfDataRecord = getFixedLengthByteArrayFromHeader(header, _index, EDFHeader.FixedLength_DuraitonOfDataRecordInSeconds);
            this.DurationOfDataRecordInSeconds = double.Parse(new string(encoding.GetChars(durationOfDataRecord)).Trim());
            _index += EDFHeader.FixedLength_DuraitonOfDataRecordInSeconds;

            // 导联数
            byte[] numberOfSignals = getFixedLengthByteArrayFromHeader(header, _index, EDFHeader.FixedLength_NumberOfSignalsInDataRecord);
            this.NumberOfSignalsInDataRecord = int.Parse(new string(encoding.GetChars(numberOfSignals)).Trim());
            if (this.NumberOfSignalsInDataRecord < 1 || this.NumberOfSignalsInDataRecord > 256)
            {
                throw new ArgumentException("EDF File has " + this.NumberOfSignalsInDataRecord + " Signals; Number of Signals must be >1 and <=256");
            }
            _index += EDFHeader.FixedLength_NumberOfSignalsInDataRecord;

            this.PatientIdentification = new EDFLocalPatientIdentification(encoding.GetChars(localPatientIdentification));
            this.RecordingIdentification = new EDFLocalRecordingIdentification(encoding.GetChars(localRecordingIdentification));

            this.StartDateTime = DateTime.ParseExact(this.StartDateEDF + " " + this.StartTimeEDF, "dd.MM.yy HH.mm.ss", System.Globalization.CultureInfo.InvariantCulture);
            //if (this.IsEDFPlus)
            //{
            //    if (!this.StartDateTime.Date.Equals(this.RecordingIdentification.RecordingStartDate))
            //    {
            //        throw new ArgumentException("Header StartDateTime does not equal Header.RecordingIdentification StartDate!");
            //    }
            //    else
            //    {
            this.RecordingIdentification.RecordingStartDate = this.StartDateTime;
            //    }
            //}
        }

        /// <summary>
        /// 解析导联列表
        /// </summary>
        public void parseSignals(byte[] signals, Encoding encoding)
        {
            // 将转换后的内容写入缓存
            _strHeader.Append(encoding.GetChars(signals));

            // 解析导联列表
            this.Signals = new List<EDFSignal>();

            // TODO 各部分字节数应该写成常量
            EDFSignal edf_signal;
            int _index;
            for (int i = 0; i < this.NumberOfSignalsInDataRecord; i++)
            {
                edf_signal = new EDFSignal();

                _index = 0;

                // 标签名
                var _labelLength = 16;
                byte[] label = getFixedLengthByteArrayFromHeader(signals, (i * _labelLength) + (this.NumberOfSignalsInDataRecord * _index), _labelLength);
                edf_signal.Label = new string(encoding.GetChars(label)).Trim();
                _index += _labelLength;

                // if (edf_signal.Label.IndexOf("Annotations") > 0) continue;

                // 序号
                edf_signal.IndexNumber = (i + 1);

                // 传感器类型
                var _transducerTypeLength = 80;
                byte[] transducer_type = getFixedLengthByteArrayFromHeader(signals, (i * _transducerTypeLength) + (this.NumberOfSignalsInDataRecord * _index), _transducerTypeLength);
                edf_signal.TransducerType = new string(encoding.GetChars(transducer_type));
                _index += _transducerTypeLength;

                // 
                var _physicalDimensionLength = 8;
                byte[] physical_dimension = getFixedLengthByteArrayFromHeader(signals, (i * _physicalDimensionLength) + (this.NumberOfSignalsInDataRecord * _index), _physicalDimensionLength);
                edf_signal.PhysicalDimension = new string(encoding.GetChars(physical_dimension));
                _index += _physicalDimensionLength;

                //
                var _physicalMinLength = 8;
                byte[] physical_min = getFixedLengthByteArrayFromHeader(signals, (i * _physicalMinLength) + (this.NumberOfSignalsInDataRecord * _index), _physicalMinLength);
                edf_signal.PhysicalMinimum = float.Parse(new string(encoding.GetChars(physical_min)).Trim());
                _index += _physicalMinLength;

                //
                var _physicalMaxLength = 8;
                byte[] physical_max = getFixedLengthByteArrayFromHeader(signals, (i * _physicalMaxLength) + (this.NumberOfSignalsInDataRecord * _index), _physicalMaxLength);
                edf_signal.PhysicalMaximum = float.Parse(new string(encoding.GetChars(physical_max)).Trim());
                _index += _physicalMaxLength;

                //
                var _digitalMinLength = 8;
                byte[] digital_min = getFixedLengthByteArrayFromHeader(signals, (i * _digitalMinLength) + (this.NumberOfSignalsInDataRecord * _index), _digitalMinLength);
                edf_signal.DigitalMinimum = float.Parse(new string(encoding.GetChars(digital_min)).Trim());
                _index += _digitalMinLength;

                //
                var _digitalMaxLength = 8;
                byte[] digital_max = getFixedLengthByteArrayFromHeader(signals, (i * _digitalMaxLength) + (this.NumberOfSignalsInDataRecord * _index), _digitalMaxLength);
                edf_signal.DigitalMaximum = float.Parse(new string(encoding.GetChars(digital_max)).Trim());
                _index += _digitalMaxLength;

                // 
                var _prefilteringLength = 80;
                byte[] prefiltering = getFixedLengthByteArrayFromHeader(signals, (i * _prefilteringLength) + (this.NumberOfSignalsInDataRecord * _index), _prefilteringLength);
                edf_signal.Prefiltering = new string(encoding.GetChars(prefiltering));
                _index += _prefilteringLength;

                // 
                var _samplesEachDatarecordLength = 8;
                byte[] samples_each_datarecord = getFixedLengthByteArrayFromHeader(signals, (i * _samplesEachDatarecordLength) + (this.NumberOfSignalsInDataRecord * _index), _samplesEachDatarecordLength);
                edf_signal.NumberOfSamplesPerDataRecord = int.Parse(new string(encoding.GetChars(samples_each_datarecord)).Trim());
                _index += _samplesEachDatarecordLength;


                this.Signals.Add(edf_signal);
            }
        }

        private byte[] getFixedLengthByteArrayFromHeader(byte[] header, int offset, int count)
        {
            var bytes = new byte[count];

            Buffer.BlockCopy(header, offset, bytes, 0, count);

            return bytes;
        }

        #endregion

        private string getFixedLengthString(string input, int length)
        {
            return (input ?? "").Length > length ? (input ?? "").Substring(0, length) : (input ?? "").PadRight(length);
        }
        private char[] getFixedLengthCharArrayFromHeader(char[] header, int startPoint, int length)
        {
            char[] ch = new char[length];
            Array.Copy(header, startPoint, ch, 0, length);
            return ch;

        }
        public override string ToString()
        {
            StringBuilder _strHeaderBuilder = new StringBuilder(string.Empty);
            _strHeaderBuilder.Append(getFixedLengthString(this.Version, EDFHeader.FixedLength_Version));
            _strHeaderBuilder.Append(this.PatientIdentification.ToString());
            _strHeaderBuilder.Append(this.RecordingIdentification.ToString());
            _strHeaderBuilder.Append(this.StartDateEDF);
            _strHeaderBuilder.Append(this.StartTimeEDF);
            _strHeaderBuilder.Append(getFixedLengthString(Convert.ToString(this.NumberOfBytes), EDFHeader.FixedLength_NumberOfBytes));
            _strHeaderBuilder.Append(this.Reserved);
            _strHeaderBuilder.Append(getFixedLengthString(Convert.ToString(this.NumberOfDataRecords), EDFHeader.FixedLength_NumberOfDataRecords));
            _strHeaderBuilder.Append(getFixedLengthString(Convert.ToString(this.DurationOfDataRecordInSeconds), EDFHeader.FixedLength_DuraitonOfDataRecordInSeconds));
            _strHeaderBuilder.Append(getFixedLengthString(Convert.ToString(this.NumberOfSignalsInDataRecord), EDFHeader.FixedLength_NumberOfSignalsInDataRecord));
            foreach (EDFSignal s in this.Signals)
                _strHeaderBuilder.Append(getFixedLengthString(s.Label, 16));
            foreach (EDFSignal s in this.Signals)
                _strHeaderBuilder.Append(getFixedLengthString(s.TransducerType, 80));
            foreach (EDFSignal s in this.Signals)
                _strHeaderBuilder.Append(getFixedLengthString(s.PhysicalDimension, 8));
            foreach (EDFSignal s in this.Signals)
                _strHeaderBuilder.Append(getFixedLengthString(Convert.ToString(s.PhysicalMinimum), 8));
            foreach (EDFSignal s in this.Signals)
                _strHeaderBuilder.Append(getFixedLengthString(Convert.ToString(s.PhysicalMaximum), 8));
            foreach (EDFSignal s in this.Signals)
                _strHeaderBuilder.Append(getFixedLengthString(Convert.ToString(s.DigitalMinimum), 8));
            foreach (EDFSignal s in this.Signals)
                _strHeaderBuilder.Append(getFixedLengthString(Convert.ToString(s.DigitalMaximum), 8));
            foreach (EDFSignal s in this.Signals)
                _strHeaderBuilder.Append(getFixedLengthString(s.Prefiltering, 80));
            foreach (EDFSignal s in this.Signals)
                _strHeaderBuilder.Append(getFixedLengthString(Convert.ToString(s.NumberOfSamplesPerDataRecord), 8));
            foreach (EDFSignal s in this.Signals)
                _strHeaderBuilder.Append(getFixedLengthString("", 32));

            if (_strHeaderBuilder.ToString().ToCharArray().Length != (256 + (this.Signals.Count * 256)))
            {
                throw new InvalidOperationException("Header Length must be equal to (256 characters + (number of signals) * 256 ).  Header length=" + _strHeaderBuilder.ToString().ToCharArray().Length + "  Header=" + _strHeaderBuilder.ToString());
            }
            _strHeader = _strHeaderBuilder;
            return _strHeaderBuilder.ToString();
        }

        /// <summary>
        /// 导联标签兼容性处理
        /// </summary>
        /// <param name="oldLabel"></param>
        /// <returns></returns>
        private String labelCompatible(String oldLabel)
        {
            String newLabel = oldLabel.ToUpper();

            if (newLabel.StartsWith("EEG "))
            {
                newLabel = newLabel.Replace("EEG ", "");
            }

            return newLabel;
        }

    }
}
