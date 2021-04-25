using System.Collections.Generic;
using System.Linq;

namespace lastooctree
{
  public class Octree
  {    
    public OctreeNode ParentNode { get; set; }

    public static int NumberOfNodes { get; set; }

    public static int DepthOfTree { get; set; }

    /// <summary>
    /// Initiating composition of Octree by defining first node
    /// </summary>
    /// <param name="pointDataRecords"></param>
    public void Compose(List<OcTreePointDataRecord> pointDataRecords)
    {       
        ParentNode = new OctreeNode();     
        ParentNode.NodeDepth=1;
        ParentNode.NodeParent=0;
        ParentNode.NodeNo=1;
        
        long minX = pointDataRecords.OrderByDescending(item => item.X).Last().X;
        long maxX = pointDataRecords.OrderByDescending(item => item.X).First().X;

        long minY = pointDataRecords.OrderByDescending(item => item.Y).Last().Y;
        long maxY = pointDataRecords.OrderByDescending(item => item.Y).First().Y;

        long minZ = pointDataRecords.OrderByDescending(item => item.Z).Last().Z;
        long maxZ = pointDataRecords.OrderByDescending(item => item.Z).First().Z;
        
        ParentNode.NodeCorners = new Point3D[8]{
            new Point3D { X=minX, Y=minY, Z=maxZ },
            new Point3D { X=maxX, Y=minY, Z=maxZ },
            new Point3D { X=maxX, Y=minY, Z=minZ },
            new Point3D { X=minX, Y=minY, Z=minZ },
            new Point3D { X=minX, Y=maxY, Z=maxZ },
            new Point3D { X=maxX, Y=maxY, Z=maxZ },
            new Point3D { X=maxX, Y=maxY, Z=minZ },
            new Point3D { X=minX, Y=maxY, Z=minZ }
        };

        ParentNode.NodePoints = pointDataRecords;
        
        Octree.DepthOfTree=ParentNode.NodeDepth;
        Octree.NumberOfNodes=ParentNode.NodeNo;
        ParentNode.Divide(ParentNode);            
    }
  }

}