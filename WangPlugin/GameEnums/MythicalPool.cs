﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WangPlugin
{
    internal class MythicalPool
    {
        public static bool MythicalFlag(int Species)
        {
            if (Species is 151 or 251 or 385 or 489 or 490 or 492 or 493 or
                 494 or 648 or 649 or 719 or 720 or 721 or 801 or
                 802 or 808 or 386 or 491 or 647 or 807 or 809 or
                 893 )
                return true;
            else
                return false;
        }
    }
}
