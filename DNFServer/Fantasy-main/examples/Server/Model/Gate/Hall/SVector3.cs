using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class SVector3
    {
        public float x; 
        public float y; 
        public float z;
        public SVector3(float x,  float y,  float z)
        { 
            this.x = x; this.y = y; this.z = z;
        }

        public override string ToString()
        {
            return string.Format("x:{0} y:{1} z:{2}",x,y,z);
        }
    }
}
