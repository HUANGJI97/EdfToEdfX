using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EDF;

namespace EEGReplay.model.edf
{
    class MyEDF
    {
        /// <summary>
        /// 头
        /// </summary>
        private MyEDFileHeader header;
        private MyEDFileHeader _header;

        
        /// <summary>
        /// 头长度
        /// </summary>
        private static int HEADER_LENGTH = 256;
        /// <summary>
        /// 信号长度
        /// </summary>
        private static int SIGNAL_LENGTH = 256;

        /// <summary>
        /// 下位机采集时放大的倍率，上位机显示数据时要除以此倍率  --by zt
        /// </summary>
        private int multiplyingPower = 12;

        /// <summary>
        /// 自定义文件数据
        /// </summary>
        private List<MyEDFDataRecord> _mydataRecords;

        /// <summary>
        /// 手环采集的edf文件标识，方便解析时做处理，同时兼容标准edf。
        /// 8位长度字符串
        /// </summary>
        public readonly string NEURO_BOND_FLAG = "bond    ";
        private MyEDFileHeader Header
        {
            get
            {
                return _header;
            }
        }

        /// <summary>
        /// EDF文件读取
        /// </summary>
        /// <param name="file_path">EDF文件路径</param>
        /// <param name="isOnlyReadHeader">是否只读取Header</param>
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

                        if (this.isEDFXFile()){
                            Console.WriteLine("以EDFX格式解析");
                            this.parseMyDataRecordStream(sr);
                        }
                        else
                        {
                            try
                            {
                                 //先以BIO-NODISPALY格式解析 如果异常抛出 则改用下面EDF格式解析
                                // 20170626 新的EDF解析逻辑（bio）
                                Console.WriteLine("以BIO-NODISPLAY格式解析");
                                this.parseDataRecordStreamByBIO(sr);

                            }
                            catch
                            {
                                Console.WriteLine("以EDF格式解析");
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
        /// 解析一个自定义(EDFX)文件EDFX转EDF
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
        /// 从String类型转换到HEX
        /// </summary>
        /// <param name="hexString">字符串</param>
        /// <returns></returns>
        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }


        /// <summary>
        /// 解析BIO-NoDisplay格式的数据
        /// </summary>
        /// <param name="sr"></param>
        private void parseDataRecordStreamByBIO(StreamReader sr)
        {
            // 先判断是否为NoDisplay格式
            if (this.header.Signals[this.header.Signals.Count - 1].Label.EndsWith("NoRef", StringComparison.InvariantCultureIgnoreCase) == false)
            {
                throw new Exception("格式错误，请选择NoDisplay格式的数据。");
            }

            // 跳过头文件
            sr.BaseStream.Seek((256 + this.header.NumberOfSignalsInDataRecord * 256), SeekOrigin.Begin);

            // TODO 20170626 暂时先用旧逻辑试试

            #region 旧逻辑

            // 计算每个信道的采样周期和总时长
            int dataRecordSize = 0;
            foreach (EDFSignal signal in this.header.Signals)
            {
                signal.SamplePeriodWithinDataRecord = (float)(this.header.DurationOfDataRecordInSeconds / signal.NumberOfSamplesPerDataRecord);
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
                foreach (EDFSignal signal in this.header.Signals)
                {
                    float refVoltage = signal.PhysicalMaximum;

                    // 一秒内一个通道内的数据
                    List<float> samples = new List<float>();
                    for (int l = 0; l < signal.NumberOfSamplesPerDataRecord; l++)
                    {
                        float value = (float)BitConverter.ToInt16(dataRecordBytes, (samplesWritten * 2));

                        if (this.header.Version.Equals(NEURO_BOND_FLAG))
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
                int samplingRate = this.header.Signals[0].NumberOfSamplesPerDataRecord; // 采样率，即每秒多少点
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

        /// <summary>
        /// 解析EDF数据到EDFX中 
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
            int count;
            Console.WriteLine("信号长度："+this.Header.Signals.Count());
            foreach (EDFSignal signal in this.Header.Signals)
            {
                //数据记录中的采样周期 =  数据记录秒的持续时间 / 每个数据记录的样本数
                signal.SamplePeriodWithinDataRecord = (float)(this.Header.DurationOfDataRecordInSeconds / signal.NumberOfSamplesPerDataRecord);
                //Console.WriteLine($"数据记录中的采样周期={signal.SamplePeriodWithinDataRecord}={this.Header.DurationOfDataRecordInSeconds}/{signal.NumberOfSamplesPerDataRecord}");
                //Console.WriteLine($"数据记录长度={dataRecordSize}+{signal.NumberOfSamplesPerDataRecord}={signal.NumberOfSamplesPerDataRecord+dataRecordSize}\n");


                dataRecordSize += signal.NumberOfSamplesPerDataRecord;
                //数据记录长度+=每条记录样本数
            }

            // 移除最后一个注解通道，不滤波和画图（放在此处移除，是为了解析每个数据块和每个通道的字节数正确） - add by lzy 20170502
            // TODO 移除后少个EKG通道 20170614
            // this.Header.Signals.RemoveAt(this.Header.Signals.Count - 1);
            //记录数据字节数组长度为 33*256*2
            ///<summary>
            ///j录数据字节数组长度
            ///</summary>
            byte[] dataRecordBytes = new byte[dataRecordSize * 2];
           


           //自定义数据列表
            List<MyEDFDataRecord> myDataRecordList = this._mydataRecords;
            

            //如果自定义数据列表为空
            if (myDataRecordList == null)
            {
                //实例化自定义记录数据
                myDataRecordList = new List<MyEDFDataRecord>();
                this._mydataRecords = myDataRecordList;
            }

            // 每次读出一秒内的20个通道的数据
            int a = 0;
            while (sr.BaseStream.Read(dataRecordBytes, 0, dataRecordSize * 2) > 0)
            {
                a++;
                //Console.Write($"[{a}]当前基础流位置:{sr.BaseStream.Read(dataRecordBytes, 0, dataRecordSize * 2)}");
                // 每个时间点的全部通道的数据
                MyEDFDataRecord myDataRecord = new MyEDFDataRecord();

                int signalIndex = 0;
                int samplesWritten = 0;
                // 一秒内的全部通道的数据，一维索引为通道数，二维索引为位置
                List<List<float>> samplesList = new List<List<float>>();
                String str = "";
                foreach (EDFSignal signal in this.Header.Signals)
                {


                    float refVoltage = signal.PhysicalMaximum;//物理信号最大量（用来计算值）

                    // 一秒内一个通道内的数据
                    List<float> samples = new List<float>();  //数据样本
                    str += $"[{a}]当前记录样本数[{signal.NumberOfSamplesPerDataRecord}]    ";

                    for (int l = 0; l < signal.NumberOfSamplesPerDataRecord; l++)
                    {
                        float value = (float)BitConverter.ToInt16(dataRecordBytes, (samplesWritten * 2));
                        //str += $"{samplesWritten * 2}  ";
                        
                       /*手环模式*/
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
                        //str += $"{value}  ";
                        samplesWritten++;
                    }
                    str += "\n";
                    signalIndex++;
                    samplesList.Add(samples);//往数据集里添加数据
                }
                str += "\n\n";
               
                //Console.WriteLine(str);
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
        /// 判断是否为EDFX文件 
        /// </summary>
        /// <returns>返回是或否</returns>
        public bool isEDFXFile() {
            return this._header.Signals[0].DigitalMaximum.Equals(Convert.ToInt32("7FFFFF", 16));
        }


        /// <summary>
        /// 解析文件头
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <param name="encoding">编码</param>
        private void parseHeader(FileStream fs, Encoding encoding)
        {
            // 读取文件头
            var buffer = new byte[HEADER_LENGTH];


            fs.Read(buffer, 0, buffer.Length);
            String str = "头文件输出:\n";
            int i = 0;
            //foreach (byte b in buffer)
            //{
            //    i++;
            //    str += b.ToString() + "  ";
            //    if (i % 16 == 0) str += "\n";

            //}
            
            byte[] version = new byte[8];
            byte[] patientInfo = new byte[80];
            byte[] recordingInfo = new byte[80];
            byte[] recordDate = new byte[8];
            byte[] recordTime = new byte[8];
            byte[] headerRecordByteLength = new byte[8];
            byte[] reserved = new byte[44];
            byte[] dataRecrdNumber = new byte[8];
            byte[] durationOfDataRecord = new byte[8];
            byte[] signalNumber = new byte[4];
            List<byte[]> headerInfos = new List<byte[]>();
            headerInfos.Add(version);
            headerInfos.Add(patientInfo);
            headerInfos.Add(recordingInfo);
            headerInfos.Add(recordDate);
            headerInfos.Add(recordTime);
            headerInfos.Add(headerRecordByteLength);
            headerInfos.Add(reserved);
            headerInfos.Add(dataRecrdNumber);
            headerInfos.Add(durationOfDataRecord);
            headerInfos.Add(signalNumber);



            int currentPosition = 0;

            for (int j = 0; j < headerInfos.Count; j++) {
                headerInfos[j] = buffer.Skip(currentPosition).Take(headerInfos[j].Length).ToArray();
                currentPosition += headerInfos[j].Length;
            }






            str += $"数据版本号:[ {Ascii2Str(headerInfos[0]).Trim()} ]\n"+
                   $"患者信息:[{Ascii2Str(headerInfos[1]).Trim()}]\n"+
                   $"记录信息:[{Ascii2Str(headerInfos[2]).Trim()}]\n"+
                   $"记录日期:[{Ascii2Str(headerInfos[3]).Trim()}]\n"+
                   $"记录时间:[{Ascii2Str(headerInfos[4]).Trim()}]\n"+
                   $"头记录字节长度:[{Ascii2Str(headerInfos[5]).Trim()}]\n"+
                   $"保留区:[{Ascii2Str(headerInfos[6]).Trim()}]\n"+
                   $"数据记录数:[{Ascii2Str(headerInfos[7]).Trim()}]\n"+
                   $"数据记录周期:[{Ascii2Str(headerInfos[8]).Trim()}]\n"+
                   $"信道数量:[{Ascii2Str(headerInfos[9]).Trim()}]\n";


            //for (int currentPosition = 0; currentPosition <buffer.Length ; currentPosition++) {


            //}
            Debug.WriteLine(str);
            MessageBox.Show(str);
            // 解析文件头
            this._header = new MyEDFileHeader(buffer, encoding);

        }


        public static string Ascii2Str(byte[] buf)
        {
            return System.Text.Encoding.ASCII.GetString(buf);
        }






        /// <summary>
        /// 获取文件编码
        /// </summary>
        /// <param name="file_path">文件路径</param>
        /// <returns></returns>
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
        /// 解析导联表
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <param name="encoding">编码</param>
        /// <summary>
        /// 解析导联列表
        /// </summary>
        private void parseSignals(FileStream fs, Encoding encoding)
        {
            // 读取导联列表
            var buffer = new byte[this._header.NumberOfSignalsInDataRecord * SIGNAL_LENGTH];
            //Debug.WriteLine("NumberOfSignalsInDataRecord长度" + this._header.NumberOfSignalsInDataRecord);
            /*
               新建字节数组 buffer  长度为信号数据记录数*信号长度(256)
             */

            fs.Read(buffer, 0, buffer.Length);//从文件里读出数据
                                              //string debugStr = $"导联表数据({buffer.Length}):\n";
                                              //for (var i = 0; i < buffer.Length; i++)
                                              //{
                                              //    debugStr += buffer[i] + " ";
                                              //    if (i % 16 == 0) debugStr += "\n";

            //}
            //Debug.WriteLine(debugStr);



            // 解析导联列表

            String str = "信道解析:\n";
            byte[] myLabel = new byte[16];//标签
            byte[] transducerType = new byte[80];//传感器类型
            byte[] physicalDimension = new byte[8];//物理量
            byte[] physicalMinimum = new byte[8];//最小物理量
            byte[] physicalMaximum = new byte[8];//最大物理量
            byte[] digitalMinimum = new byte[8];//最小数字量
            byte[] digitalMaximum = new byte[8];//最大数字量
            byte[] prefilterings = new byte[80];//滤波
            byte[] samplesDataRecord = new byte[8];//每条记录的样本
            byte[] reserved = new byte[32];//保留位

            List<byte[]> signalInfo = new List<byte[]>();
            signalInfo.Add(myLabel);
            signalInfo.Add(transducerType);
            signalInfo.Add(physicalDimension);
            signalInfo.Add(physicalMinimum);
            signalInfo.Add(physicalMaximum);
            signalInfo.Add(digitalMinimum);
            signalInfo.Add(digitalMaximum);
            signalInfo.Add(prefilterings);
            signalInfo.Add(samplesDataRecord);
            signalInfo.Add(reserved);


            int currentPoint = 0;
            for (int a = 0; a < signalInfo.Count; a++)
            {
                signalInfo[a] = buffer.Skip(currentPoint).Take(signalInfo[a].Length).ToArray();
                currentPoint += signalInfo[a].Length;
            }

            str += $"标签:[ {Ascii2Str(signalInfo[0]).Trim()} ]\n" +
                  $"传感器类型:[{Ascii2Str(signalInfo[1]).Trim()}]\n" +
                  $"物理量:[{Ascii2Str(signalInfo[2]).Trim()}]\n" +
                  $"最小物理量:[{Ascii2Str(signalInfo[3]).Trim()}]\n" +
                  $"最大物理量:[{Ascii2Str(signalInfo[4]).Trim()}]\n" +
                  $"最小数字量:[{Ascii2Str(signalInfo[5]).Trim()}]\n" +
                  $"最大数字量:[{Ascii2Str(signalInfo[6]).Trim()}]\n" +
                  $"滤波:[{Ascii2Str(signalInfo[7]).Trim()}]\n" +
                  $"每条记录的样本:[{Ascii2Str(signalInfo[8]).Trim()}]\n" +
                  $"保留位:[{Ascii2Str(signalInfo[9]).Trim()}]\n";


            MessageBox.Show(str);




            this._header.parseSignals(buffer, encoding);

        }



        /// <summary>
        /// 文件头
        /// </summary>
        class MyEDFileHeader
        {
            
           /// <summary>
           /// 数据版本
           /// </summary>
            private String dataVersion;
            /// <summary>
            /// 患者信息
            /// </summary>
            private String patientInfo;
            /// <summary>
            /// 记录日期
            /// </summary>
            private String recordDate;
            /// <summary>
            /// 记录时间
            /// </summary>
            private String recordTime;
            /// <summary>
            /// 头长度
            /// </summary>
            private String headLong;
            /// <summary>
            /// 保留位
            /// </summary>
            private String resverved;
            /// <summary>
            /// 总数据点数
            /// </summary>
            private String dataNumber;
            /// <summary>
            /// 信号数量
            /// </summary>
            private String signalNumber;
            /// <summary>
            /// 信号
            /// </summary>
            public List<EDFSignal> Signals;
            /// <summary>
            /// 默认版本号
            /// </summary>
            private static string VERSION_DEFAULT = "0       ";

            private bool _isEDFPlus = false;
            public bool IsEDFPlus
            {
                get
                {
                    return _isEDFPlus;
                }
            }

            //保留固定长度
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

            //以“年月日”的形式记录日期（xcg）
            public string StartDate { get; set; }

            public static int FixedLength_NumberOfDataRecords = 8;   //固定记录数据长度
            private string _NumberOfDataRecordsFixedLengthString = "0";//固定记录数据
            private int _NumberOfDataRecords = 0;   //数据记录数
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


            private StringBuilder _strHeader = new StringBuilder(string.Empty);
            //构造函数
            public MyEDFileHeader(byte[] header, Encoding encoding)
            {

                if (header.Length != 256)
                {
                    throw new ArgumentException("Header must be 256 characters"); //错误警报 文件头长度必须为256位
                }

                parseHeader(header, encoding);
            }


            /// <summary>
            /// 解析文件头
            /// </summary>
            private void parseHeader(byte[] header, Encoding encoding)
            {
                // 将转换后的内容写入缓存
                _strHeader.Append(encoding.GetChars(header));

                // 开始解析
                int _index = 0; //存放内容长度

                // 版本号
                byte[] version = getFixedLengthByteArrayFromHeader(header, _index, MyEDFileHeader.FixedLength_Version);
                this.Version = new string(encoding.GetChars(version));          
                _index += FixedLength_Version;

                // 病人ID
                byte[] localPatientIdentification = getFixedLengthByteArrayFromHeader(header, _index, MyEDFileHeader.FixedLength_LocalPatientIdentification);
                _index += MyEDFileHeader.FixedLength_LocalPatientIdentification;

                // 记录ID
                byte[] localRecordingIdentification = getFixedLengthByteArrayFromHeader(header, _index, MyEDFileHeader.FixedLength_LocalRecordingIdentifiaction);
                _index += MyEDFileHeader.FixedLength_LocalRecordingIdentifiaction;

                // 开始日期
                byte[] startDate = getFixedLengthByteArrayFromHeader(header, _index, MyEDFileHeader.FixedLength_StartDateEDF);
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


            public static int FixedLength_NumberOfBytes = 8; //字节固定长度
            private string _NumberOfBytesFixedLengthString = "0";//固定长度字节串
            private int _NumberOfBytes = 0;          //字节数
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
            }              //get，set方法

            public static int FixedLength_StartTimeEDF = 8;//存放开始时间长度
            public string StartTimeEDF { get; set; } //删掉private
            public DateTime EndTimeEDF { get; set; } //添加结束时间 zt
          

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


            public static int FixedLength_StartDateEDF = 8; //存放日期长度
            public string StartDateEDF { get; set; } //删掉private

            public static int FixedLength_LocalRecordingIdentifiaction = 80;//记录信息长度
            private EDFLocalRecordingIdentification _RecordingInformation;  //记录信息
            public EDFLocalRecordingIdentification RecordingIdentification   //记录信息 set get方法
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


            public static int FixedLength_LocalPatientIdentification = 80;  //病人信息长度
            private EDFLocalPatientIdentification _PatientInformation;     //病人信息
            public EDFLocalPatientIdentification PatientIdentification     //病人信息set get 方法
            {
                get
                {
                    return _PatientInformation;
                }
                set
                {
                    if (value.ToString().Length != FixedLength_LocalPatientIdentification)//值的长度不等于80
                    {
                        throw new FormatException("Patient Information must be " + FixedLength_LocalPatientIdentification + " characters fixed length");//报错
                    }
                    _PatientInformation = value;
                }
            }


            //固定长度字符数据记录的信号量
            public static int FixedLength_NumberOfSignalsInDataRecord = 4;
            private string _NumberOfSignalsInDataRecordFixedLengthString = "0";
            private int _NumberOfSignalsInDataRecord = 0;
            public int NumberOfSignalsInDataRecord    // 信号数据记录数
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

            //每秒读取数据长度
            public static int FixedLength_DuraitonOfDataRecordInSeconds = 8;
            private string _DurationOfDataRecordInSecondsFixedLengthString = "0";
            private double _DurationOfDataRecordInSeconds = 0; //修改int为double
            /// <summary>
            /// 数据记录秒的持续时间
            /// </summary>
            public double DurationOfDataRecordInSeconds {
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

            private static int FixedLength_Version = 8; //版本固定长度
            private string _Version = VERSION_DEFAULT;   //赋值为0
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


            /// <summary>
            /// 获取固定长度字符串
            /// </summary>
            /// <param name="input">字符串</param>
            /// <param name="length">长度</param>
            /// <returns></returns>
            private string getFixedLengthString(string input, int length)
            {
                return (input ?? "").Length > length ? (input ?? "").Substring(0, length) : (input ?? "").PadRight(length);
                /*
                    ??运算符  合并运算符
                    int  a=null;
                    c=a??100;//c=100
                    如果 input==null 则赋值为""  

                 */
            }

            /// <summary>
            /// 解析导联列表
            /// </summary>
            public void parseSignals(byte[] signals, Encoding encoding)
            {
                //将转换后的内容写入缓存
                _strHeader.Append(encoding.GetChars(signals));

                // 解析导联列表。
                this.Signals = new List<EDFSignal>();
                // TODO 各部分字节数应该写成常量
                EDFSignal edf_signal;
                int _index;

                //遍历33个信道
                for (int i = 0; i < this.NumberOfSignalsInDataRecord; i++){
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
           
            /// <summary>
            /// 从头文件获取固定长度字节
            /// </summary>
            /// <param name="header">头字节</param>
            /// <param name="offset">偏移量</param>
            /// <param name="count">获取数量</param>
            /// <returns></returns>
            private byte[] getFixedLengthByteArrayFromHeader(byte[] header, int offset, int count)
            {
                var bytes = new byte[count];

                Buffer.BlockCopy(header, offset, bytes, 0, count);

                return bytes;
            }

        }

        /// <summary>
        /// EDF信号类
        /// </summary>
        class EDFSignal
        {
            public EDFSignal()
            {
                //empty constructor
            }

            private StringBuilder _strSignal = new StringBuilder(String.Empty);
            public int IndexNumber { get; set; }
            public string IndexNumberWithLabel
            {
                get
                {
                    return this.IndexNumber + "." + this.Label;
                }
            }
            public string Label { get; set; }
            public string LabelType { get; set; }
            public string LabelSpecification { get; set; }
            public string TransducerType { get; set; } // equal to the lower and upper bounds
            public string PhysicalDimension { get; set; }
            public string PhysicalDimensionPrefix { get; set; }
            public string PhysicalDimensionBasic { get; set; }
            public float PhysicalMinimum { get; set; }
            /// <summary>
            /// 物理信号最大量
            /// </summary>
            public float PhysicalMaximum { get; set; }
            public float DigitalMinimum { get; set; }
            public float DigitalMaximum { get; set; }
            public string Prefiltering { get; set; }
            private int _NumberOfSamplesPerDataRecord;
            /// <summary>
            /// 每个数据记录的样本数
            /// </summary>
            public int NumberOfSamplesPerDataRecord
            {
                get
                {
                    if (_NumberOfSamplesPerDataRecord > 0)
                    {
                        return _NumberOfSamplesPerDataRecord;
                    }
                    else
                    {
                        throw new InvalidOperationException("Must provide the NumberOfSamplesPerDataRecord before accessing this Property");
                    }
                }
                set
                {
                    if (value > 0)
                    {
                        _NumberOfSamplesPerDataRecord = value;
                    }
                    else
                    {
                        throw new ArgumentException("NumberOfSamplesPerDataRecord must be set to greater than 0");
                    }
                }
            }

            /**
             * I don't understand the name of this parameter, yet.  It is used in getting the value out of the 2-byte integer, and was called
             * "sense" in the C sample code I learned the format from.
             */
            public float AmplifierGain //http://en.wikipedia.org/wiki/Gain 
            {
                get
                {
                    return (this.PhysicalMaximum - this.PhysicalMinimum) / (this.DigitalMaximum - this.DigitalMinimum);
                }
            }
            /**
             * This is used in getting the value of the sample out of the DataRecord.  
             */
            public float Offset
            {
                get
                {
                    return ((this.PhysicalMaximum / this.AmplifierGain) - this.DigitalMaximum);
                }
            }
            /// <summary>
            /// 数据记录中的采样周期
            /// </summary>
            public float SamplePeriodWithinDataRecord { get; set; }
            public override string ToString()
            {
                return this.IndexNumberWithLabel;
            }

            public static implicit operator EDFSignal(List<EDFSignal> v)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 数据记录类
        /// </summary>
        public class MyEDFDataRecord : List<float>
        {
            //a datarecord is a SortedList where the key is the channel/signal and the value is the List of Samples (floats) within the datarecord
        }
       
    }
}
