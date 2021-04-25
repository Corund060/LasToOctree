using System.Collections.Generic;
using System.Linq;

namespace lastooctree
{
    public class OctreeNode
    {
        /// <summary>
        /// Node number of current node
        /// </summary>
        public int NodeNo { get; set; }

        /// <summary>
        /// Node depth of current node
        /// </summary>
        public int NodeDepth { get; set; }

        /// <summary>
        /// List of all the points that are inside current node
        /// </summary>
        internal List<PointDataRecord> NodePoints { get; set; }

        /// <summary>
        /// Number of the node that is parent for the current node
        /// </summary>
        public int NodeParent { get; set; }

        /// <summary>
        /// X,Y,Z coordinates of the center of the current node
        /// </summary>
        internal Point3D NodeCenter { get; set; }

        /// <summary>
        /// List of 8 child nodes of current node
        /// </summary>
        public List<OctreeNode> ChildNodes { get; set; }

        /// <summary>
        /// Array of X,Y,Z coordinates for 8 corners of current node
        /// </summary>
        internal Point3D[] NodeCorners { get; set; }        

        /// <summary>
        /// Divide current node into 8 child nodes
        /// </summary>
        /// <param name="parentNode"></param>
        internal void Divide(OctreeNode parentNode)
        {
            // Divide untill parent node has less than 1000 points
            if (parentNode.NodePoints.Count<=1000)
            {
                return;
            }

            parentNode.NodeCenter = GetDivisionPoint(parentNode.NodeCorners);
            parentNode.ChildNodes = new List<OctreeNode>();
            for (int i = 1; i <= 8; i++)
            {
                var currentNodeCorners = GetNodeCorners(parentNode, i);
                Octree.NumberOfNodes++;
                if (Octree.DepthOfTree<parentNode.NodeDepth+1)
                {
                    Octree.DepthOfTree = parentNode.NodeDepth + 1;
                }
                parentNode.ChildNodes.Add(new OctreeNode
                {
                    NodeNo = Octree.NumberOfNodes,
                    NodeDepth = parentNode.NodeDepth + 1,
                    NodeParent = parentNode.NodeNo,
                    NodeCorners = currentNodeCorners,
                    NodePoints = GetNodePoints(parentNode, currentNodeCorners)
                });
                Divide(parentNode.ChildNodes.Last());
            }                       
        }

        /// <summary>
        /// Assign parent points that are inside child node to child node
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="nodeCorners"></param>
        /// <returns></returns>
        private List<PointDataRecord> GetNodePoints(OctreeNode parentNode, Point3D[] nodeCorners)
        {
            return parentNode.NodePoints.Where(
                                        p => p.X >= nodeCorners[0].X && p.X <= nodeCorners[1].X &&
                                             p.Y >= nodeCorners[3].Y && p.Y <= nodeCorners[4].Y &&
                                             p.Z >= nodeCorners[2].Z && p.Z <= nodeCorners[1].Z
                   ).ToList();
        }

        /// <summary>
        /// Get 8 corners of the child node according to its number
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childNo"></param>
        /// <returns></returns>
        private Point3D[] GetNodeCorners(OctreeNode parent, int childNo )
        {
            var parentCorners = parent.NodeCorners;
            switch (childNo)
            {
                //Tope near left
                case 1:
                    return new Point3D[8] {
                        new Point3D {                                           X=parentCorners[0].X, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2,                        Z=parentCorners[0].Z }, // Bottom near left
                        new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2,                        Z=parentCorners[0].Z }, // Bottom near right
                        parent.NodeCenter,                                                                                                                                     // Center
                        new Point3D {                                           X=parentCorners[0].X, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2, Z=parentCorners[7].Z+(parentCorners[4].Z-parentCorners[7].Z)/2 }, // Bottom far left
                        new Point3D {                                           X=parentCorners[4].X,                                           Y=parentCorners[4].Y,                         Z=parentCorners[4].Z}, // Top near left
                        new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2,                                           Y=parentCorners[4].Y,                         Z=parentCorners[4].Z}, // Top near right
                        new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2,                                           Y=parentCorners[4].Y, Z=parentCorners[7].Z+(parentCorners[4].Z-parentCorners[7].Z)/2 }, // Top far right
                        new Point3D {                                           X=parentCorners[0].X,                                           Y=parentCorners[4].Y, Z=parentCorners[7].Z+(parentCorners[4].Z-parentCorners[7].Z)/2 }  // Top far left
                    };                    
                //Top near right
                case 2:
                    return new Point3D[8] {
                        new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2,                        Z=parentCorners[0].Z },
                        new Point3D {                                            X=parentCorners[1].X, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2,                        Z=parentCorners[0].Z },                        
                        new Point3D {                                            X=parentCorners[1].X, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2, Z=parentCorners[7].Z+(parentCorners[0].Z-parentCorners[3].Z)/2 },
                        parent.NodeCenter,
                        new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2,                                            Y=parentCorners[4].Y,                         Z=parentCorners[4].Z},
                        new Point3D {                                            X=parentCorners[1].X,                                           Y=parentCorners[4].Y,                         Z=parentCorners[4].Z},
                        new Point3D {                                            X=parentCorners[1].X,                                           Y=parentCorners[4].Y, Z=parentCorners[7].Z+(parentCorners[4].Z-parentCorners[7].Z)/2 },
                        new Point3D {  X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2,                                           Y=parentCorners[4].Y, Z=parentCorners[7].Z+(parentCorners[4].Z-parentCorners[7].Z)/2 }
                    };                
                // Top far right
                case 3:
                    return new Point3D[8] {
                        parent.NodeCenter,                                                                                                                                                                                              // Center
                        new Point3D {                                           X=parentCorners[1].X, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2, Z=parentCorners[7].Z+(parentCorners[4].Z-parentCorners[7].Z)/2 }, // Bottom near right 
                        new Point3D {                                           X=parentCorners[1].X, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2,                                           Z=parentCorners[7].Z }, // Bottom far right 
                        new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2,                                            Z=parentCorners[7].Z}, // Bottom far left
                        new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2,                                           Y=parentCorners[4].Y,  Z=parentCorners[7].Z+(parentCorners[4].Z-parentCorners[7].Z)/2}, // Top near left
                        new Point3D {                                           X=parentCorners[1].X,                                           Y=parentCorners[4].Y,  Z=parentCorners[7].Z+(parentCorners[4].Z-parentCorners[7].Z)/2}, // Top near right                        
                        new Point3D {                                           X=parentCorners[1].X,                                           Y=parentCorners[4].Y,                                            Z=parentCorners[7].Z}, // Top far right                        
                        new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2,                                           Y=parentCorners[4].Y,                                            Z=parentCorners[7].Z} // Bottom far right                        
                    };                
                // Top far left
                case 4:
                    return new Point3D[8] {
                        new Point3D {                                           X=parentCorners[0].X, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2, Z=parentCorners[7].Z+(parentCorners[4].Z-parentCorners[7].Z)/2 }, // Botom near left
                        parent.NodeCenter,                                                                                                                                                                                              // Center
                        new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2,                        Z=parentCorners[7].Z }, // Bottom far right
                        new Point3D {                                           X=parentCorners[0].X, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2,                        Z=parentCorners[7].Z }, // Bottom far left
                        new Point3D {                                           X=parentCorners[0].X,                                           Y=parentCorners[4].Y, Z=parentCorners[7].Z+(parentCorners[4].Z-parentCorners[7].Z)/2 },  // Top near left                        
                        new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2,                                           Y=parentCorners[4].Y, Z=parentCorners[7].Z+(parentCorners[4].Z-parentCorners[7].Z)/2}, // Top near right
                        new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2,                                           Y=parentCorners[4].Y,                         Z=parentCorners[7].Z},  // Top far right
                        new Point3D {                                           X=parentCorners[0].X,                                           Y=parentCorners[4].Y,                         Z=parentCorners[7].Z}  // Top far left
                    };                    
                // Bottom near left
                case 5:
                    return new Point3D[8] {
                        new Point3D {                                           X=parentCorners[0].X,                                           Y=parentCorners[0].Y,                                            Z=parentCorners[0].Z}, // Bottom near left
                        new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2,                                           Y=parentCorners[0].Y,                                            Z=parentCorners[0].Z}, // Bottom near right
                        new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2,                                           Y=parentCorners[0].Y,  Z=parentCorners[7].Z+(parentCorners[4].Z-parentCorners[7].Z)/2}, // Bottom far right
                        new Point3D {                                           X=parentCorners[0].X,                                           Y=parentCorners[0].Y,  Z=parentCorners[7].Z+(parentCorners[4].Z-parentCorners[7].Z)/2}, // Bottom far left
                        new Point3D {                                           X=parentCorners[0].X, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2,                                           Z=parentCorners[0].Z }, // Top near left
                        new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2,                                           Z=parentCorners[0].Z }, // Yop near right
                        parent.NodeCenter,                                                                                                                                                                                              // Center
                        new Point3D {                                           X=parentCorners[0].X, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2, Z=parentCorners[7].Z+(parentCorners[0].Z-parentCorners[3].Z)/2 }  // Top far left
                    };                    
                // Bottom near right
                case 6:
                    return new Point3D[8] {
                        new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2,                                           Y=parentCorners[0].Y,                                           Z=parentCorners[0].Z }, // Bottom near left
                        new Point3D {                                           X=parentCorners[2].X,                                           Y=parentCorners[2].Y,                                            Z=parentCorners[0].Z}, // Bottom near right
                        new Point3D {                                           X=parentCorners[2].X,                                           Y=parentCorners[2].Y,  Z=parentCorners[7].Z+(parentCorners[4].Z-parentCorners[7].Z)/2}, // Bottom far right
                        new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2,                                           Y=parentCorners[2].Y,  Z=parentCorners[7].Z+(parentCorners[4].Z-parentCorners[7].Z)/2}, // Bottom far left
                        new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2,                                           Z=parentCorners[0].Z }, // Top left right
                        new Point3D {                                           X=parentCorners[2].X, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2,                                           Z=parentCorners[0].Z }, //Top near right
                        new Point3D {                                           X=parentCorners[2].X, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2, Z=parentCorners[7].Z+(parentCorners[4].Z-parentCorners[7].Z)/2 }, //Top far right
                        parent.NodeCenter                                                                                                                                                                                               // Center
                    };                    
                // Bottom far right
                case 7:
                    return new Point3D[8] {
                        new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2,                   Y=parentCorners[0].Y,  Z=parentCorners[0].Z }, //Bottom near left
                        new Point3D {                                           X=parentCorners[2].X,                                            Y=parentCorners[2].Y,  Z=parentCorners[7].Z+(parentCorners[4].Z-parentCorners[7].Z)/2}, // Bottom near right
                        new Point3D {                                           X=parentCorners[2].X,                                            Y=parentCorners[2].Y,                        Z=parentCorners[2].Z }, // Bottom far right
                        new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2,                                            Y=parentCorners[2].Y,                        Z=parentCorners[2].Z }, // Bottom far left
                        parent.NodeCenter,                                                                                                                                     // Center
                        new Point3D {                                           X=parentCorners[1].X, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2, Z=parentCorners[7].Z+(parentCorners[4].Z-parentCorners[7].Z)/2 }, // Top near right
                        new Point3D {                                           X=parentCorners[1].X, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2,                         Z=parentCorners[2].Z}, // Top far right                                                                                                                                   
                        new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2,                         Z=parentCorners[2].Z}, // Top far left
                    };                    
                // Bottom far left
                case 8:
                    return new Point3D[8] {
                          new Point3D {                                           X=parentCorners[0].X,                                           Y=parentCorners[0].Y, Z=parentCorners[7].Z+(parentCorners[4].Z-parentCorners[7].Z)/2 }, // Bottom near left
                          new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2,                                           Y=parentCorners[0].Y, Z=parentCorners[7].Z+(parentCorners[4].Z-parentCorners[7].Z)/2 }, // Bottom near right
                          new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2,                                           Y=parentCorners[2].Y,                                           Z=parentCorners[2].Z }, // Bottom far right
                          new Point3D {                                           X=parentCorners[3].X,                                           Y=parentCorners[3].Y,                                           Z=parentCorners[3].Z }, // Bottom far left
                          new Point3D {                                           X=parentCorners[0].X, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2, Z=parentCorners[7].Z+(parentCorners[0].Z-parentCorners[3].Z)/2 }, // Top far left
                          parent.NodeCenter,                                                                                                                                     // Center
                          new Point3D { X=parentCorners[0].X+(parentCorners[1].X-parentCorners[0].X)/2, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2,                                           Z=parentCorners[7].Z }, // Top far right
                          new Point3D {                                           X=parentCorners[0].X, Y=parentCorners[0].Y+(parentCorners[4].Y-parentCorners[0].Y)/2,                                           Z=parentCorners[7].Z }, // Top far left
                        };                        
                    default:
                        return null;                    
            }
            
        }

        /// <summary>
        /// Get division (center) point of the node 
        /// </summary>
        /// <param name="nodeCorners"></param>
        /// <returns></returns>
        private Point3D GetDivisionPoint(Point3D[] nodeCorners)
        {
            return new Point3D {
                X = nodeCorners[0].X+ (nodeCorners[1].X - nodeCorners[0].X) / 2,
                Y = nodeCorners[3].Y+ (nodeCorners[4].Y - nodeCorners[3].Y) / 2,
                Z = nodeCorners[2].Z+ (nodeCorners[1].Z - nodeCorners[2].Z) / 2
            };
        }        
    }
}
