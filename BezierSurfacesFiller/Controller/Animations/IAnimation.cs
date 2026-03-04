using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BezierSurfacesFiller.Controller.Animations
{
    public interface IAnimation
    {
        void Update();
        bool Enabled { get; set; }
    }
}
