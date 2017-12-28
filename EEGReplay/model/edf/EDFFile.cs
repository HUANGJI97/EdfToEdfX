using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections;
using EEGReplay;

namespace EDF
{
    /// <summary>
    /// EDFFile类
    /// </summary>
    public class EDFFile
    {
        public EDFFile()
        {
            //initialize EDFHeader as part of constructor
            _header = new EDFHeader();
            //initialize EDFDataRecord List as part of constructor
            _dataRecords = new List<EDFDataRecord>();

            //解析自定义格式数据时用到  --by zt
            _mydataRecords = new List<MyEDFDataRecord>();

            //保存数据时用到  --by zt
            _dataHex = new StringBuilder();
        }

        #region 20170613 常量

        private static int HEADER_LENGTH = 256;//头长度
        private static int SIGNAL_LENGTH = 256;//信号长度

        #endregion 20170613 常量

        #region 变量

        /// <summary>
        /// 头文件
        /// </summary>
        private EDFHeader _header;

        /// <summary>
        /// 保存数据时用到  --by zt
        /// </summary>
        private StringBuilder _dataHex;

        /// <summary>
        /// dataRecord数据
        /// </summary>
        private List<EDFDataRecord> _dataRecords;

        /// <summary>
        /// 自定义文件数据
        /// </summary>
        private List<MyEDFDataRecord> _mydataRecords;

        /// <summary>
        /// 下位机采集时放大的倍率，上位机显示数据时要除以此倍率  --by zt
        /// </summary>
        private int multiplyingPower = 12;

        /// <summary>
        /// 手环采集的edf文件标识，方便解析时做处理，同时兼容标准edf。
        /// 8位长度字符串
        /// </summary>
        public readonly string NEURO_BOND_FLAG = "bond    ";

        #endregion 变量

        #region 访问器

        public EDFHeader Header
        {
            get
            {
                return _header;
            }
        }

        public List<EDFDataRecord> DataRecords
        {
            set
            {
                _dataRecords = value;
            }
            get
            {
                return _dataRecords;
            }
        }

        public StringBuilder DataHex
        {
            get
            {
                return _dataHex;
            }
        }

        public List<MyEDFDataRecord> MyDataRecords
        {
            get
            {
                return _mydataRecords;
            }
        }

        #endregion 访问器

        #region 20170613 重写解析逻辑，旧方法使用char取头，如果含有中文则取值范围错误

        #region 旧方法

        /*
        public void readFile(string file_path)
        {
            //open the file to read the header
            FileStream file = new FileStream(file_path, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(file, GetEncoding(file_path));
            readStream(sr);
            file.Close();
            sr.Close();
            file.Dispose();
            sr.Dispose();
        }

        /// <summary>
        /// 只解析头文件  --by zt
        /// </summary>
        /// <param name="file_path"></param>
        public void readFileHeader(string file_path)
        {
            //open the file to read the header
            FileStream file = new FileStream(file_path, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(file, GetEncoding(file_path));
            parseHeaderStream(sr);
            file.Close();
            sr.Close();
            file.Dispose();
            sr.Dispose();
        }

        /// <summary>
        /// 解析edf文件
        /// </summary>
        /// <param name="sr"></param>
        public void readStream(StreamReader sr)
        {
            parseHeaderStream(sr);

            if (this.isEDFXFile())
            {
                this.parseMyDataRecordStream(sr);
            }
            else
            {
                // this.parseDataRecordStream(sr);
                this.parseDataRecordStream2(sr);
            }
        }

        /// <summary>
        /// 解析头文件
        /// </summary>
        /// <param name="sr"></param>
        private void parseHeaderStream(StreamReader sr)
        {
            //parse the header to get the number of Signals (size of the Singal Header)
            char[] header = new char[256];
            sr.ReadBlock(header, 0, 256);
            this._header = new EDFHeader(header);

            //parse the signals within the header
            char[] signals = new char[this.Header.NumberOfSignalsInDataRecord * 256];
            sr.ReadBlock(signals, 0, this.Header.NumberOfSignalsInDataRecord * 256);
            this.Header.parseSignals(signals);
        }
        */

        #endregion 旧方法

            /// <summary>
        /// 解析文件
        /// </summary>
        /// <param name="file_path">文件路径</param>
        /// <param name="isOnlyReadHeader">是否只解析头部</param>
        public void readFile(string file_path, bool isOnlyReadHeader = false)
        {
            using (FileStream fs = new FileStream(file_path, FileMode.Open, FileAccess.Read))
            {
                // 获取文件编码
                Encoding encoding = GetEncoding(file_path);
                // encoding = Encoding.BigEndianUnicode;

                // 解析文件头
                parseHeader(fs, encoding);

                // 解析导联列表
                parseSignals(fs, encoding);

                // 解析文件内容
             
                if (isOnlyReadHeader.Equals(false))
                {
                    
                    using (StreamReader sr = new StreamReader(fs, encoding))
                    {
                        if (this.isEDFXFile())
                        {
                            this.parseMyDataRecordStream(sr);
                        }
                        else
                        {
                            try
                            {
                                // 20170626 新的EDF解析逻辑（bio）
                                this.parseDataRecordStreamByBIO(sr);
                            }
                            catch
                            {
                                this.parseDataRecordStream2(sr);
                            }
                            // this.parseDataRecordStream(sr);
                            // this.parseDataRecordStream2(sr);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 解析文件头
        /// </summary>
        /// <param name="file_path">文件路径</param>
        public void readFileHeader(string file_path)
        {
            readFile(file_path, true);
        }

        /// <summary>
        /// 获取文件编码方法
        /// </summary>
        private Encoding GetEncoding(string file_path)
        {
            Encoding encoding = Encoding.Default;

            try
            {
                using (FileStream file = new FileStream(file_path, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sReader = new StreamReader(file, true))
                    {
                        char[] chArray = new char[1];

                        sReader.Read(chArray, 0, 1);

                        encoding = sReader.CurrentEncoding;

                        sReader.BaseStream.Position = 0;

                        if (encoding.Equals(Encoding.UTF8))
                        {
                            byte[] encodedbuffer = encoding.GetPreamble();

                            if (file.Length >= encodedbuffer.Length)
                            {
                                byte[] buffer = new byte[encodedbuffer.Length];

                                file.Read(buffer, 0, buffer.Length);

                                for (int i = 0; i < buffer.Length; i++)
                                {
                                    if (buffer[i] != encodedbuffer[i])
                                    {
                                        encoding = Encoding.Default;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                encoding = Encoding.Default;
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            return encoding;
        }

        /// <summary>
        /// 解析文件头
        /// </summary>
        private void parseHeader(FileStream fs, Encoding encoding)
        {
            // 读取文件头
            var buffer = new byte[HEADER_LENGTH];
     

            fs.Read(buffer, 0, buffer.Length);
            String str="";
            int i = 0;
            foreach (byte b in buffer) {
                i++;
                str += b.ToString()+"  ";
                if (i % 16 == 0) str += "\n";
                
            }
            Debug.WriteLine(str);

            // 解析文件头
            this._header = new EDFHeader(buffer, encoding);
        
        }

        /// <summary>
        /// 解析导联列表
        /// </summary>
        private void parseSignals(FileStream fs, Encoding encoding)
        {
            // 读取导联列表
            var buffer = new byte[this._header.NumberOfSignalsInDataRecord * SIGNAL_LENGTH];
            Debug.WriteLine("NumberOfSignalsInDataRecord长度"+this._header.NumberOfSignalsInDataRecord);
            /*
               新建字节数组 buffer  长度为信号数据记录数*信号长度(256)
             */

            fs.Read(buffer, 0, buffer.Length);//从文件里读出数据
            string debugStr = $"导联表数据({buffer.Length}):\n";
            for (var i = 0; i < buffer.Length; i++) {
               debugStr+=buffer[i]+" ";
                if (i % 16 == 0) debugStr+="\n";

            }
            Debug.WriteLine(debugStr);



            // 解析导联列表
            this._header.parseSignals(buffer, encoding);

        }

        /// <summary>
        /// 解析BIO-NoDisplay格式的数据
        /// </summary>
        /*
             笔记 StreamReader  以一种特定的编码输入字符
         
             */
        private void parseDataRecordStreamByBIO(StreamReader sr)
        {
            // 先判断是否为NoDisplay格式
            if (this._header.Signals[this._header.Signals.Count - 1].Label.EndsWith("NoRef", StringComparison.InvariantCultureIgnoreCase) == false)
            {
                throw new Exception("格式错误，请选择NoDisplay格式的数据。");
            }

            // 跳过头文件
            sr.BaseStream.Seek((256 + this.Header.NumberOfSignalsInDataRecord * 256), SeekOrigin.Begin);

            // TODO 20170626 暂时先用旧逻辑试试

            #region 旧逻辑

            // 计算每个信道的采样周期和总时长
            int dataRecordSize = 0;
            foreach (EDFSignal signal in this.Header.Signals)
            {
                signal.SamplePeriodWithinDataRecord = (float)(this.Header.DurationOfDataRecordInSeconds / signal.NumberOfSamplesPerDataRecord);
                dataRecordSize += signal.NumberOfSamplesPerDataRecord;
            }

            // 移除最后一个注解通道，不滤波和画图（放在此处移除，是为了解析每个数据块和每个通道的字节数正确） - add by lzy 20170502
            // TODO 移除后少个EKG通道 20170614
            // this.Header.Signals.RemoveAt(this.Header.Signals.Count - 1);

            byte[] dataRecordBytes = new byte[dataRecordSize * 2];

            List<MyEDFDataRecord> myDataRecordList = this._mydataRecords;
            if (myDataRecordList == null)
            {
                myDataRecordList = new List<MyEDFDataRecord>();
                this._mydataRecords = myDataRecordList;
            }

            // 每次读出一秒内的20个通道的数据
            while (sr.BaseStream.Read(dataRecordBytes, 0, dataRecordSize * 2) > 0)
            {
                // 每个时间点的全部通道的数据
                MyEDFDataRecord myDataRecord = new MyEDFDataRecord();

                int signalIndex = 0;
                int samplesWritten = 0;
                // 一秒内的全部通道的数据，一维索引为通道数，二维索引为位置
                List<List<float>> samplesList = new List<List<float>>();
                foreach (EDFSignal signal in this.Header.Signals)
                {
                    float refVoltage = signal.PhysicalMaximum;

                    // 一秒内一个通道内的数据
                    List<float> samples = new List<float>();
                    for (int l = 0; l < signal.NumberOfSamplesPerDataRecord; l++)
                    {
                        float value = (float)BitConverter.ToInt16(dataRecordBytes, (samplesWritten * 2));

                        if (this.Header.Version.Equals(NEURO_BOND_FLAG))
                        {
                            if (value >= 0 && value <= Math.Pow(2, 15) - 1)
                                value = refVoltage * value / (float)(Math.Pow(2, 15) - 1);
                            else
                                //value = refVoltage * ((value - (float)Math.Pow(2, 20)) / (float)(Math.Pow(2, 19) - 1));
                                value = refVoltage * value / (float)Math.Pow(2, 15);
                        }

                        // value /= multiplyingPower;
                        samples.Add(value);
                        samplesWritten++;
                    }
                    signalIndex++;
                    samplesList.Add(samples);
                }

                // 把一秒内的全部通道的数据存入myDataRecordList
                int samplingRate = this.Header.Signals[0].NumberOfSamplesPerDataRecord; // 采样率，即每秒多少点
                for (int i = 0; i < samplingRate; i++)
                {
                    MyEDFDataRecord myEDFDataRecord = new MyEDFDataRecord();
                    for (int j = 0; j < samplesList.Count; j++)
                    {
                        if (i > samplesList[j].Count - 1) continue;
                        float value = samplesList[j][i];
                        myEDFDataRecord.Add(value);
                    }
                    this._mydataRecords.Add(myEDFDataRecord);
                }
            }

            #endregion 旧逻辑
        }

        #endregion 20170613 重写解析逻辑，旧方法使用char取头，如果含有中文则取值范围错误

        public byte[] getEDFFileBytes()
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = encoding.GetBytes(this.Header.ToString().ToCharArray());
            List<byte> byteList = new List<byte>(byteArray);
            byteList.AddRange(getCompressedDataRecordsBytes());
            return byteList.ToArray();
        }

        /// <summary>
        /// 将DataRecords数据转为byteList，保存时用到
        /// </summary>
        /// <returns></returns>
        public List<byte> getCompressedDataRecordsBytes()
        {
            List<byte> byteList = new List<byte>();
            byte float00 = HexToByte("00")[0];
            //byteList.Add(float00);
            byte[] byteArraySample = new byte[2];
            /*
            foreach (EDFDataRecord dataRecord in this.DataRecords)
            {
                foreach (EDFSignal signal in this.Header.Signals)
                {
                    foreach (float sample in dataRecord[signal.IndexNumberWithLabel])
                    {
                        byteArraySample = BitConverter.GetBytes(Convert.ToInt16((sample / signal.AmplifierGain) - signal.Offset));
                        byteList.Add(byteArraySample[0]);
                        byteList.Add(byteArraySample[1]);
                    }
                }
            }
             */

            // 修改 开始 李忠洋
            if (this._header.Signals[0].DigitalMaximum.Equals(Convert.ToInt32("7FFFFF", 16)))
            {
                foreach (MyEDFDataRecord myDataRecord in this.MyDataRecords)
                {
                    foreach (EDFSignal signal in this.Header.Signals)
                    {
                        // foreach (float sample in myDataRecord[this.Header.Signals.IndexOf(signal)])
                        float sample = myDataRecord[this.Header.Signals.IndexOf(signal)];

                        // 将数据按读取的方式还原
                        float refVoltage = signal.PhysicalMaximum;
                        sample = sample * multiplyingPower;
                        if (sample >= 0)
                        {
                            sample = sample * (float)(Math.Pow(2, 23) - 1) / refVoltage;
                        }
                        else
                        {
                            // value = refVoltage * ((value - (float)Math.Pow(2, 24)) / (float)(Math.Pow(2, 23) - 1));
                            sample = sample / refVoltage * (float)(Math.Pow(2, 23) - 1) + (float)Math.Pow(2, 24);
                        }

                        //byteArraySample = BitConverter.GetBytes(Convert.ToInt32((sample / signal.AmplifierGain) - signal.Offset));
                        byteArraySample = BitConverter.GetBytes(Convert.ToInt32(sample));
                        byteList.Add(byteArraySample[2]);
                        byteList.Add(byteArraySample[1]);
                        byteList.Add(byteArraySample[0]);
                        //byteList.Add(byteArraySample[3]);
                    }
                }
            }
            else
            {
                foreach (EDFDataRecord dataRecord in this.DataRecords)
                {
                    foreach (EDFSignal signal in this.Header.Signals)
                    {
                        if (!dataRecord.ContainsKey(signal.IndexNumberWithLabel)) continue;

                        foreach (float sample in dataRecord[signal.IndexNumberWithLabel])
                        {
                            // byteArraySample = BitConverter.GetBytes(Convert.ToInt16((sample / signal.AmplifierGain) - signal.Offset));
                            byteArraySample = BitConverter.GetBytes(Convert.ToInt16(sample));
                            byteList.Add(byteArraySample[0]);
                            byteList.Add(byteArraySample[1]);
                        }
                    }
                }
            }
            // 修改 结束 李忠洋

            return byteList;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="file_path"></param>
        public void saveFile(string file_path)
        {
            if (File.Exists(file_path))
            {
                File.Delete(file_path);
            }
            FileStream newFile = new FileStream(file_path, FileMode.CreateNew, FileAccess.Write);

            StreamWriter sw = new StreamWriter(newFile);

            /*
             * 保存文件临时修改，待确认  李忠洋
             */
            //this.Header.NumberOfDataRecords = this.DataRecords.Count;
            if (this._header.Signals[0].DigitalMaximum.Equals(Convert.ToInt32("7FFFFF", 16)))
                this.Header.NumberOfDataRecords = this.MyDataRecords.Count;
            else
                this.Header.NumberOfDataRecords = this.DataRecords.Count;
            //this.Header.NumberOfDataRecords = this.DataRecords.Count;

            char[] headerCharArray = this.Header.ToString().ToCharArray();
            sw.Write(headerCharArray, 0, headerCharArray.Length);
            sw.Flush();

            newFile.Seek((256 + this.Header.NumberOfSignalsInDataRecord * 256), SeekOrigin.Begin);
            BinaryWriter bw = new BinaryWriter(newFile);

            /*
             * 保存的record不对，待确认  李忠洋
             */
            byte[] byteList = getCompressedDataRecordsBytes().ToArray();

            bw.Write(byteList, 0, byteList.Length);
            bw.Flush();
            sw.Close();
            bw.Close();
            newFile.Close();
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="sr"></param>
        private void parseDataRecordStream(StreamReader sr)
        {
            //set the seek position in the file stream to the beginning of the data records.
            sr.BaseStream.Seek((256 + this.Header.NumberOfSignalsInDataRecord * 256), SeekOrigin.Begin);

            int dataRecordSize = 0;
            foreach (EDFSignal signal in this.Header.Signals)
            {
                signal.SamplePeriodWithinDataRecord = (float)(this.Header.DurationOfDataRecordInSeconds / signal.NumberOfSamplesPerDataRecord);
                dataRecordSize += signal.NumberOfSamplesPerDataRecord;
            }

            byte[] dataRecordBytes = new byte[dataRecordSize * 2];

            while (sr.BaseStream.Read(dataRecordBytes, 0, dataRecordSize * 2) > 0)
            {
                EDFDataRecord dataRecord = new EDFDataRecord();
                int j = 0;
                int samplesWritten = 0;
                foreach (EDFSignal signal in this.Header.Signals)
                {
                    float refVoltage = signal.PhysicalMaximum;

                    List<float> samples = new List<float>();
                    for (int l = 0; l < signal.NumberOfSamplesPerDataRecord; l++)
                    {
                        // float value = (float)(((BitConverter.ToInt16(dataRecordBytes, (samplesWritten * 2)) + (int)signal.Offset)) * signal.AmplifierGain);
                        float value = (float)BitConverter.ToInt16(dataRecordBytes, (samplesWritten * 2));

                        if (this.Header.Version.Equals(NEURO_BOND_FLAG))
                        {
                            if (value >= 0 && value <= Math.Pow(2, 19) - 1)
                                value = refVoltage * value / (float)(Math.Pow(2, 19) - 1);
                            else
                                //value = refVoltage * ((value - (float)Math.Pow(2, 20)) / (float)(Math.Pow(2, 19) - 1));
                                value = refVoltage * value / (float)Math.Pow(2, 19);
                        }

                        value /= multiplyingPower;
                        samples.Add(value);
                        samplesWritten++;
                    }
                    dataRecord.Add(signal.IndexNumberWithLabel, samples);
                    j++;
                }
                _dataRecords.Add(dataRecord);
            }
        }

        /// <summary>
        /// 解析edf数据到edfx中 add by lzy 20170427
        /// </summary>
        /// <param name="sr"></param>
        private void parseDataRecordStream2(StreamReader sr)
        {
            //set the seek position in the file stream to the beginning of the data records.
            //设置当前文件流 开始读取的位置 
            /*sr.BaseStream.Seek(偏移量,开始位置)*/
            sr.BaseStream.Seek((256 + this.Header.NumberOfSignalsInDataRecord * 256), SeekOrigin.Begin);

            int dataRecordSize = 0;
            //遍历头文件信号
            foreach (EDFSignal signal in this.Header.Signals)
            {
                  //数据采样周期=  采样频率 / 每个记录的样本数
                signal.SamplePeriodWithinDataRecord = (float)(this.Header.DurationOfDataRecordInSeconds / signal.NumberOfSamplesPerDataRecord);
                dataRecordSize += signal.NumberOfSamplesPerDataRecord;
                //数据记录长度+=每条记录样本数
            }

            // 移除最后一个注解通道，不滤波和画图（放在此处移除，是为了解析每个数据块和每个通道的字节数正确） - add by lzy 20170502
            // TODO 移除后少个EKG通道 20170614
            // this.Header.Signals.RemoveAt(this.Header.Signals.Count - 1);

            byte[] dataRecordBytes = new byte[dataRecordSize * 2];

            List<MyEDFDataRecord> myDataRecordList = this._mydataRecords;
            if (myDataRecordList == null)
            {
                myDataRecordList = new List<MyEDFDataRecord>();
                this._mydataRecords = myDataRecordList;
            }

            // 每次读出一秒内的20个通道的数据
            while (sr.BaseStream.Read(dataRecordBytes, 0, dataRecordSize * 2) > 0)
            {
                // 每个时间点的全部通道的数据
                MyEDFDataRecord myDataRecord = new MyEDFDataRecord();

                int signalIndex = 0;
                int samplesWritten = 0;
                // 一秒内的全部通道的数据，一维索引为通道数，二维索引为位置
                List<List<float>> samplesList = new List<List<float>>();
               
                foreach (EDFSignal signal in this.Header.Signals)
                {


                    float refVoltage = signal.PhysicalMaximum;//物理信号最大量

                    // 一秒内一个通道内的数据
                    List<float> samples = new List<float>();
                    for (int l = 0; l < signal.NumberOfSamplesPerDataRecord; l++)
                    {
                        float value = (float)BitConverter.ToInt16(dataRecordBytes, (samplesWritten * 2));

                        if (this.Header.Version.Equals(NEURO_BOND_FLAG))
                        {
                            if (value >= 0 && value <= Math.Pow(2, 15) - 1)
                                value = refVoltage * value / (float)(Math.Pow(2, 15) - 1);
                            else
                                //value = refVoltage * ((value - (float)Math.Pow(2, 20)) / (float)(Math.Pow(2, 19) - 1));
                                value = refVoltage * value / (float)Math.Pow(2, 15);
                        }

                        // value /= multiplyingPower;
                        samples.Add(value);
                        samplesWritten++;
                    }
                    signalIndex++;
                    samplesList.Add(samples);
                }

                // 把一秒内的全部通道的数据存入myDataRecordList
                int samplingRate = this.Header.Signals[0].NumberOfSamplesPerDataRecord; // 采样率，即每秒多少点
                for (int i = 0; i < samplingRate; i++)
                {
                    MyEDFDataRecord myEDFDataRecord = new MyEDFDataRecord();
                    for (int j = 0; j < samplesList.Count; j++)
                    {
                        if (i > samplesList[j].Count - 1) continue;
                        float value = samplesList[j][i];
                        myEDFDataRecord.Add(value);
                    }
                    this._mydataRecords.Add(myEDFDataRecord);
                }
            }
        }

        /// <summary>
        /// 解析自定义的文件  --by zt
        /// </summary>
        /// <param name="sr"></param>
        private void parseMyDataRecordStream(StreamReader sr)
        {
            //set the seek position in the file stream to the beginning of the data records.
            sr.BaseStream.Seek((256 + this.Header.NumberOfSignalsInDataRecord * 256), SeekOrigin.Begin);

            int dataRecordSize = 0;
            foreach (EDFSignal signal in this.Header.Signals)
            {
                signal.SamplePeriodWithinDataRecord = (float)(this.Header.DurationOfDataRecordInSeconds / signal.NumberOfSamplesPerDataRecord);
                dataRecordSize += signal.NumberOfSamplesPerDataRecord;
            }

            byte[] dataRecordBytes = new byte[dataRecordSize * 3];
            byte first00 = HexToByte("00")[0];
            while (sr.BaseStream.Read(dataRecordBytes, 0, dataRecordSize * 3) > 0)
            {
                //EDFDataRecord dataRecord = new EDFDataRecord();
                MyEDFDataRecord dataRecord = new MyEDFDataRecord();
                int j = 0;
                int samplesWritten = 0;
                List<EDFSignal> signals = this.Header.Signals;

                #region 解析与edf格式相似的方式，以后可能会用到  --by zt

                //for (int i = 0; i < signals.Count; i++)
                //{
                //    List<float> samples = new List<float>();
                //    for (int l = 0; l < signals[i].NumberOfSamplesPerDataRecord; l++)
                //    {
                //        float refVoltage = signals[i].PhysicalMaximum;
                //        //从dataRecordBytes中取3个字节

                //        byte[] temp = new byte[4] { dataRecordBytes[samplesWritten * 3 + 2], dataRecordBytes[samplesWritten * 3 + 1], dataRecordBytes[samplesWritten * 3], first00 };

                //        float value = (float)(BitConverter.ToInt32(temp, 0));
                //        if (value >= 0 && value <= Math.Pow(2, 23) - 1)
                //            value = refVoltage * value / (float)(Math.Pow(2, 23) - 1);
                //        else
                //            value = refVoltage * ((value - (float)Math.Pow(2, 24)) / (float)(Math.Pow(2, 23) - 1));
                //        value = value / multiplyingPower;
                //        samples.Add(value);
                //        samplesWritten++;
                //    }
                //    dataRecord.Add(i.ToString(), samples);
                //    //dataRecord.Add(value);
                //    //dataRecord.Add(signal.IndexNumberWithLabel, value);
                //    j++;
                //}

                #endregion 解析与edf格式相似的方式，以后可能会用到  --by zt

                foreach (EDFSignal signal in this.Header.Signals)
                {
                    //List<float> samples = new List<float>();
                    //for (int l = 0; l < signal.NumberOfSamplesPerDataRecord; l++)
                    //{
                    float refVoltage = signal.PhysicalMaximum;
                    //从dataRecordBytes中取3个字节

                    byte[] temp = new byte[4] { dataRecordBytes[samplesWritten * 3 + 2], dataRecordBytes[samplesWritten * 3 + 1], dataRecordBytes[samplesWritten * 3], first00 };

                    float value = (float)(BitConverter.ToInt32(temp, 0));
                    if (value >= 0 && value <= Math.Pow(2, 23) - 1)
                        value = refVoltage * value / (float)(Math.Pow(2, 23) - 1);
                    else
                        value = refVoltage * ((value - (float)Math.Pow(2, 24)) / (float)(Math.Pow(2, 23) - 1));
                    value = value / multiplyingPower;
                    //samples.Add(value);
                    samplesWritten++;
                    //}
                    //dataRecord.Add(signal.IndexNumberWithLabel, samples);
                    dataRecord.Add(value);
                    //dataRecord.Add(signal.IndexNumberWithLabel, value);
                    j++;
                }
                _mydataRecords.Add(dataRecord);
                //_dataRecords.Add(dataRecord);
            }
        }

        /// <summary>
        /// 删除一个信道的数据
        /// </summary>
        /// <param name="signal_to_delete"></param>
        public void deleteSignal(EDFSignal signal_to_delete)
        {
            if (this.Header.Signals.Contains(signal_to_delete))
            {
                //Remove Signal DataRecords
                foreach (EDFDataRecord dr in this.DataRecords)
                {
                    foreach (EDFSignal signal in this.Header.Signals)
                    {
                        if (signal.IndexNumberWithLabel.Equals(signal_to_delete.IndexNumberWithLabel))
                        {
                            dr.Remove(signal_to_delete.IndexNumberWithLabel);
                        }
                    }
                }
                //After removing the DataRecords then Remove the Signal from the Header
                this.Header.Signals.Remove(signal_to_delete);

                //Finally Decrement the NumberOfSignals in the Header by 1
                this.Header.NumberOfSignalsInDataRecord = this.Header.NumberOfSignalsInDataRecord - 1;

                //Change the Number Of Bytes in the Header.
                this.Header.NumberOfBytes = (256) + (256 * this.Header.Signals.Count);
            }
        }

        /// <summary>
        /// 删除一个点的所有信道的记录，文件格式edfx
        /// </summary>
        /// <param name="index"></param>
        public void deleteMyEDFDataRecord(int index)
        {
            if (index >= 0 && index < this.MyDataRecords.Count)
            {
                this.MyDataRecords.RemoveAt(index);
                this.Header.NumberOfDataRecords -= 1;
                foreach (EDFSignal signal in this.Header.Signals)
                {
                    //signal.NumberOfSamplesPerDataRecord -= 1;
                }
            }
            else throw new ArgumentException("deleteMyEDFDataRecord error: out of index, index=" + index);
        }

        /// <summary>
        /// 删除一个点的所有信道的记录，文件格式edf
        /// </summary>
        /// <param name="index"></param>
        public void deleteEDFDataRecord(int index)
        {
            if (index >= 0 && index < this.DataRecords.Count)
            {
                this.DataRecords.RemoveAt(index);
                this.Header.NumberOfDataRecords -= 1;
            }
            else throw new ArgumentException("deleteEDFDataRecord error: out of index, index=" + index);
        }

        /// <summary>
        /// 添加一个信道的数据
        /// </summary>
        /// <param name="signal_to_add"></param>
        /// <param name="sampleValues"></param>
        public void addSignal(EDFSignal signal_to_add, List<float> sampleValues)
        {
            if (this.Header.Signals.Contains(signal_to_add))
            {
                this.deleteSignal(signal_to_add);
            }

            //Remove Signal DataRecords
            int index = 0;
            foreach (EDFDataRecord dr in this.DataRecords)
            {
                dr.Add(signal_to_add.IndexNumberWithLabel, sampleValues.GetRange(index * signal_to_add.NumberOfSamplesPerDataRecord, signal_to_add.NumberOfSamplesPerDataRecord));
                index++;
            }
            //After removing the DataRecords then Remove the Signal from the Header
            this.Header.Signals.Add(signal_to_add);

            //Finally Decrement the NumberOfSignals in the Header by 1
            this.Header.NumberOfSignalsInDataRecord = this.Header.NumberOfSignalsInDataRecord + 1;

            //Change the Number Of Bytes in the Header.
            this.Header.NumberOfBytes = (256) + (256 * this.Header.Signals.Count);
        }

        public List<float> retrieveSignalSampleValues(EDFSignal signal_to_retrieve)
        {
            List<float> signalSampleValues = new List<float>();

            if (this.Header.Signals.Contains(signal_to_retrieve))
            {
                //Remove Signal DataRecords
                foreach (EDFDataRecord dr in this.DataRecords)
                {
                    foreach (EDFSignal signal in this.Header.Signals)
                    {
                        if (signal.IndexNumberWithLabel.Equals(signal_to_retrieve.IndexNumberWithLabel))
                        {
                            foreach (float value in dr[signal.IndexNumberWithLabel])
                            {
                                signalSampleValues.Add(value);
                            }
                        }
                    }
                }
            }
            return signalSampleValues;
        }

        public void exportAsCompumedics(string file_path)
        {
            foreach (EDFSignal signal in this.Header.Signals)
            {
                string signal_name = this.Header.StartDateTime.ToString("MMddyyyy_HHmm") + "_" + signal.Label;
                string new_path = string.Empty;
                if (file_path.LastIndexOf('/') == file_path.Length)
                {
                    new_path = file_path + signal_name.Replace(' ', '_');
                }
                else
                {
                    new_path = file_path + '/' + signal_name.Replace(' ', '_');
                }

                if (File.Exists(new_path))
                {
                    File.Delete(new_path);
                }
                FileStream newFile = new FileStream(new_path, FileMode.CreateNew, FileAccess.Write);

                StreamWriter sw = new StreamWriter(newFile);

                if (signal.NumberOfSamplesPerDataRecord <= 0)
                {
                    //need to pad it to be sampled every second.
                    sw.WriteLine(signal.Label + " " + "RATE:1.0Hz");
                }
                else
                {
                    sw.WriteLine(signal.Label + " " + "RATE:" + Math.Round((double)(signal.NumberOfSamplesPerDataRecord / this.Header.DurationOfDataRecordInSeconds), 2) + "Hz");
                }

                foreach (EDFDataRecord dataRecord in this.DataRecords)
                {
                    foreach (float sample in dataRecord[signal.IndexNumberWithLabel])
                    {
                        sw.WriteLine(sample);
                    }
                }
                sw.Flush();
            }
        }

        /// <summary>
        /// 保存新定义格式的数据—— by zt
        /// </summary>
        /// <param name="file_Path"></param>
        public bool saveNewFile(string file_path)
        {
            lock (this.DataHex)
            {
                //LogHelper.WriteLog(typeof(EDFFile), "saveNewFile() begin, file_path = " + file_path);
                DateTime startTime = DateTime.Now;

                this.Header.NumberOfDataRecords = this.DataHex.Length / (this.Header.NumberOfSignalsInDataRecord * 6);

                //LogHelper.WriteLog(typeof(EDFFile), "this.Header.NumberOfDataRecords = " + this.Header.NumberOfDataRecords);

                //无脑电数据时不保存结果
                if (this.Header.NumberOfDataRecords < 100)
                {
                    //LogHelper.WriteLog(typeof(EDFFile), "无脑电数据时不保存结果 this.Header.NumberOfDataRecords = " + this.Header.NumberOfDataRecords);
                    //LogHelper.WriteLog(typeof(EDFFile), "saveNewFile() end");
                    return false;
                }

                this.Header.DurationOfDataRecordInSeconds = (double)((this.Header.EndTimeEDF.Ticks - this.Header.StartDateTime.Ticks) / Math.Pow(10, 7)) / (this.Header.NumberOfDataRecords);

                //LogHelper.WriteLog(typeof(EDFFile), "this.Header.DurationOfDataRecordInSeconds = " + this.Header.DurationOfDataRecordInSeconds);

                if (File.Exists(file_path))
                {
                    File.Delete(file_path);
                }
                FileStream newFile = new FileStream(file_path, FileMode.Append, FileAccess.Write);

                StreamWriter sw = new StreamWriter(newFile);

                // 保存Header
                char[] headerCharArray = this.Header.ToString().ToCharArray();
                //LogHelper.WriteLog(typeof(EDFFile), "保存Header headerCharArray.Length = " + headerCharArray.Length);
                sw.Write(headerCharArray);
                //sw.Write(headerCharArray, 0, headerCharArray.Length);
                sw.Flush();
                headerCharArray = null;

                newFile.Seek((256 + this.Header.NumberOfSignalsInDataRecord * 256), SeekOrigin.Begin);
                BinaryWriter bw = new BinaryWriter(newFile);

                // 保存DataHex
                string dataHexString = "";
                byte[] dataByteArray;
                int i = 0;
                try
                {
                    //LogHelper.WriteLog(typeof(EDFFile), "保存DataHex begin");

                    int len = 1000000;
                    int c = this.DataHex.Length / len + 1;

                    //LogHelper.WriteLog(typeof(EDFFile), "保存DataHex this.DataHex.Length = " + this.DataHex.Length + ", 需要循环保存次数:" + c);

                    for (i = 0; i < c; i++)
                    {
                        int startIndex = i * len;
                        int len2 = startIndex + len > this.DataHex.Length ? this.DataHex.Length - startIndex : len;
                        dataHexString = this.DataHex.ToString(startIndex, len2);

                        dataByteArray = HexToByte(dataHexString);
                        bw.Write(dataByteArray, 0, dataByteArray.Length);
                        bw.Flush();

                        if (i % 100 == 0)
                        {
                            //LogHelper.WriteLog(typeof(EDFFile), "保存DataHex 第" + i + "次保存成功");
                        }
                    }
                    //LogHelper.WriteLog(typeof(EDFFile), "保存DataHex end");
                    /*
                    dataHexString = this.DataHex.ToString();
                    byte[] dataByteArray = HexToByte(dataHexString);
                    bw.Write(dataByteArray, 0, dataByteArray.Length);
                    dataHexString = null;
                     * */
                }
                catch (Exception ex)
                {
                    //LogHelper.WriteLog(typeof(EDFFile), "第" + i + "次写入文件DataHex时数据转换出错：ex.Message:" + ex.Message + "\nex.StackTrace:" + ex.StackTrace + "\n数据长度为：" + dataHexString.Length);
                    // LogHelper.WriteLog(typeof(EDFFile), "此时的数据为:" + dataHexString);
                    //LogHelper.WriteLog(typeof(EDFFile), "saveNewFile() Exception end");
                    return false;
                }
                finally
                {
                    dataHexString = null;
                    dataByteArray = null;
                    //LogHelper.WriteLog(typeof(EDFFile), "保存DataHex finally end");
                }

                //byte[] dataByteArray = HexToByte(this.DataHex.ToString());

                //bw.Write(byteList, 0, byteList.Length);

                bw.Flush();
                sw.Close();
                bw.Close();
                newFile.Close();

                DateTime endTime = DateTime.Now;

                //LogHelper.WriteLog(typeof(EDFFile), "保存文件" + file_path);
                //LogHelper.WriteLog(typeof(EDFFile), "保存文件" + file_path + ", 保存文件耗时" + (endTime - startTime).ToString());

                //LogHelper.WriteLog(typeof(EDFFile), "saveNewFile() end");

                return true;
            }
        }

        public void clearDataHex()
        {
            if (this._dataHex == null) return;

            this._dataHex = null;
            this._dataHex = new StringBuilder();
        }

        /// <summary>
        /// 从string类型转换到hex
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// 是否是EDFX文件
        /// </summary>
        /// <returns></returns>
        public bool isEDFXFile()
        {
            return this._header.Signals[0].DigitalMaximum.Equals(Convert.ToInt32("7FFFFF", 16));
        }
    }
}