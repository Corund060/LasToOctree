using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace lastooctree
{
    /// <summary>
    /// LAS file data object
    /// Format specifications (1.3v): https://liblas.org/_static/files/specifications/asprs_las_format_v13.pdf
    /// </summary>
    public class LidarData
    {
        private ulong OffsetDataPoints { get; set;}
        private ulong NumberOfPointRecords { get; set; }
        public List<LASPointDataRecord> PointDataRecords { get; set; }

        /// <summary>
        /// Upload data from LAS file to object
        /// </summary>
        /// <param name="filePath"></param>
        public void UploadFile(string filePath)
        {
            using FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(filePath).Length;
            byte[] buffer = br.ReadBytes((int)numBytes);
            ExtractPublicHeader(buffer);
            //ExtractVariableLegthRecords(buffer);  not needed atm
            ExtractDataPoints(buffer);            
        }
        
        /// <summary>
        /// Extract data point records from file
        /// </summary>
        /// <param name="data"></param>
        private void ExtractDataPoints(byte[] data)
        {
            PointDataRecords = new List<LASPointDataRecord>();
            Console.Write("Extracting {0} data points: ", NumberOfPointRecords);
            ulong currentPos = OffsetDataPoints;            
            for (ulong i = 1; i <= NumberOfPointRecords ; i++)
            {
                var bitArray = new BitArray(new byte[] { data[(int)currentPos + 14] });
                PointDataRecords.Add(new LASPointDataRecord
                {
                    X = BitConverter.ToUInt32(data, (int)currentPos),
                    Y = BitConverter.ToUInt32(data, (int)currentPos + 4),
                    Z = BitConverter.ToUInt32(data, (int)currentPos + 8),
                    Intensity = BitConverter.ToUInt16(data, (int)currentPos + 12),
                    ReturnNumber = new bool[3] { bitArray[0], bitArray[1], bitArray[2] },
                    NumberOfReturns = new bool[3] { bitArray[3], bitArray[4], bitArray[5] },
                    ScanDirectionFlag = bitArray[6],
                    EdgeOfFlight = bitArray[7],
                    Classification = Convert.ToChar(data[(int)currentPos + 13]),
                    ScanAngle = Convert.ToChar(data[(int)currentPos + 14]),
                    UserData = Convert.ToChar(data[(int)currentPos + 15]),
                    PointSourceID = BitConverter.ToUInt16(data, (int)currentPos + 16),
                    GPSTime = BitConverter.ToDouble(data, (int)currentPos +18)
                });                                
                currentPos += 28;
            }
            Console.WriteLine("DONE");
            Console.WriteLine("=====================================================");
        }                

        /// <summary>
        /// Extract public header info from file
        /// </summary>
        /// <param name="data"></param>
        public void ExtractPublicHeader(byte[] data)
        {
            ////////////////////////////////////////////////////////////////// LAS VERSION 1.3 PUBLIC HEADER
            Console.Write("Extracting Public Header info:");
            string signature = Encoding.UTF8.GetString(data, 0, 4);
            if (signature != "LASF")
            {
                Console.WriteLine("Invalid file format");
                return;
            }            
            ushort fileSourceId = BitConverter.ToUInt16(data, 4);
            ushort globalEncoding = BitConverter.ToUInt16(data, 6);           
            ulong projectID1 = BitConverter.ToUInt32(data, 8);
            ushort projectID2 = BitConverter.ToUInt16(data, 12);
            ushort projectID3 = BitConverter.ToUInt16(data, 14);
            string projectID4 = Encoding.UTF8.GetString(data, 16, 8);            
            byte versionMajor = data[24];            
            byte versionMinor = data[25];          
            string systemIdentifier = Encoding.UTF8.GetString(data, 26, 32);           
            string generatingSoftware = Encoding.UTF8.GetString(data, 58, 32);            
            ushort fileCrDayOfYear = BitConverter.ToUInt16(data, 90);            
            ushort fileCrYear = BitConverter.ToUInt16(data, 92);            
            ushort headerSize = BitConverter.ToUInt16(data, 94);            
            ulong offsetToPointData = BitConverter.ToUInt32(data, 96);         
            OffsetDataPoints = offsetToPointData;
            ulong numberOfVarLengthRecords = BitConverter.ToUInt32(data, 100);            
            byte pointDataRecFormat = data[104];            
            ushort pointDataRecordLength = BitConverter.ToUInt16(data, 105);            
            ulong legacyNumberOfPointRecords = BitConverter.ToUInt32(data, 107);
            NumberOfPointRecords = legacyNumberOfPointRecords;
            ulong[] legacyNumberOfPointByReturn = new ulong[5];
            for (int i = 0; i <= 4; i++)
            {
                legacyNumberOfPointByReturn[i] = BitConverter.ToUInt32(data, 111 + i * 4);                
            }
            double xScaleFactor = BitConverter.ToDouble(data, 131);
            double yScaleFactor = BitConverter.ToDouble(data, 139);
            double zScaleFactor = BitConverter.ToDouble(data, 147);            
            double xOffset = BitConverter.ToDouble(data, 155);
            double yOffset = BitConverter.ToDouble(data, 163);
            double zOffset = BitConverter.ToDouble(data, 171);           
            double maxX = BitConverter.ToDouble(data, 179);
            double minX = BitConverter.ToDouble(data, 187);           
            double maxY = BitConverter.ToDouble(data, 195);
            double minY = BitConverter.ToDouble(data, 203);            
            double maxZ = BitConverter.ToDouble(data, 211);
            double minZ = BitConverter.ToDouble(data, 219);            
            BigInteger startOfWaveformDatapacketRecord = BitConverter.ToUInt64(data, 227);
            Console.WriteLine("DONE");

            Console.WriteLine("========================Header info==================");
            Console.WriteLine("File signature: {0}", signature);
            Console.WriteLine("Las file format version: {0}.{1}", versionMajor, versionMinor);
            Console.WriteLine("File creation: {0}", fileCrYear);
            Console.WriteLine("X,Y,Z scale factors : {0}; {1}; {2}", xScaleFactor, yScaleFactor, zScaleFactor);
            Console.WriteLine("X,Y,Z offsets : {0}; {1}; {2}", xOffset, yOffset, zOffset);
            Console.WriteLine("Max X, Min X ------- {0} : {1}", Math.Round(maxX, 2), Math.Round(minX, 2));
            Console.WriteLine("Max Y, Min Y ------- {0} : {1}", Math.Round(maxY, 2), Math.Round(minY, 2));
            Console.WriteLine("Max Z, Min Z ------- {0} : {1}", Math.Round(maxZ, 2), Math.Round(minZ, 2));
            Console.WriteLine("=====================================================");
        }

        //private void ExtractVariableLegthRecords(byte[] data)
        //{
        //    Console.WriteLine("=============RECORDS================");
        //    string userID= Encoding.UTF8.GetString(data, 237, 16);
        //    Console.WriteLine("User ID: {0}", userID);

        //    ushort recordID = BitConverter.ToUInt16(data, 253);
        //    Console.WriteLine("Record ID: {0}", recordID);

        //    ushort recordLenghtAfterHeader= BitConverter.ToUInt16(data, 255);
        //    Console.WriteLine("Record lenght after header: {0}", recordLenghtAfterHeader);

        //    string description = Encoding.UTF8.GetString(data, 257, 32);
        //    Console.WriteLine("Description: {0}", description);    

        //    string record= Encoding.UTF8.GetString(data, 289, recordLenghtAfterHeader);
        //    Console.WriteLine("Record: {0}", record);
        //}
    }
}

