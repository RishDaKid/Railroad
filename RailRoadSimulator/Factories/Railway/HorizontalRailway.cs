﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drawing.Railway
{
    class HorizontalRaleway : IRailway
    {
        public HorizontalRaleway()
        {
           model = Image.FromFile(@"..\..\Assets\tracks.png");
        }
    }
}
