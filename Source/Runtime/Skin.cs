using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AssetGenerator.Runtime
{
    class Skin
    {
        /// <summary>
        /// Inverse Bind Matrices used to bring skinned coordinates to the same space as each joint.
        /// </summary>
        public List<Matrix4x4> InverseBindMatrices { get; set; }

        /// <summary>
        /// The node used as a skeleton root.  When undefined, joints transforms resolve to scene root.
        /// </summary>
        public Runtime.Node Skeleton { get; set; }

        /// <summary>
        /// Skeleton nodes, used as joints in this skin.  
        /// </summary>
        public List<Runtime.Node> Joints { get; set; }


        public enum JointsComponentTypeEnum { UNSIGNED_BYTE, UNSIGNED_SHORT }
        
        public JointsComponentTypeEnum JointsComponentType { get; set; }

    }
}
