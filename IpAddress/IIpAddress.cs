﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Xamarinme
{
    public interface IIpAddress
    {
        string Get();

        IEnumerable<string> GetAll();
    }
}