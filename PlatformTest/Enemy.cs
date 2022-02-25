using System;
using System.Collections.Generic;
using System.Text;

namespace PlatformTest
{
    public class Enemy : Entity
    {
        public bool CanKill { get; set; }
        public bool CanBeKilled { get; set; }
    }
}
