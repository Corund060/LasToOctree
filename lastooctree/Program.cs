using System.IO;
using System.Collections.Generic;

namespace lastooctree
{
    class Program
    {
        static void Main(string[] args)
        {           
            if (args.Length!=0 && File.Exists(args[0]))         
            {
                // Get LAS file path from CL argument
                var filePath = args[0];

                // Upload file data to object
                LidarData lidar = new LidarData();
                lidar.UploadFile(filePath);

                // Convert LAS file point records to  OcTree point records               
                List<OcTreePointDataRecord>  data=new List<OcTreePointDataRecord>();  
                lidar.PointDataRecords.ForEach(l => data.Add(new OcTreePointDataRecord(l.X, l.Y, l.Z)));          

                System.Console.Write("Composing octree: ");
                //Compose Octree object from data object
                Octree octree = new Octree();    
                octree.Compose(data);
                System.Console.WriteLine("DONE");
                System.Console.WriteLine($"Number of actree nodes: {Octree.NumberOfNodes}");
                System.Console.WriteLine($"Depth of octree: {Octree.DepthOfTree}");                
            }
            else 
            {
                System.Console.WriteLine("Error: LAS file not provided/not found");
            }
        }
    }
}
