

namespace lastooctree
{
    /// <summary>
    /// LAS file data for point record
    /// Format specifications (1.3v): https://liblas.org/_static/files/specifications/asprs_las_format_v13.pdf
    /// </summary>
    public class OcTreePointDataRecord
    {
        public OcTreePointDataRecord(long x, long y, long z)
        {
            X=x;
            Y=y;
            Z=z;
        }
        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }
        public ushort Intensity { get; set; }
        public bool[] ReturnNumber { get; set; }
        public bool[] NumberOfReturns { get; set; }
        public bool ScanDirectionFlag { get; set; }
        public bool EdgeOfFlight { get; set; }
        public char Classification { get; set; }
        public char ScanAngle { get; set; }
        public char UserData { get; set; }
        public ushort PointSourceID { get; set; }
        public double GPSTime { get; set; }
    }
}
