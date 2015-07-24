﻿using System;

namespace Shoy.Backgrounder
{
    public interface IWorkItem
    {
        long Id { get; set; }
        DateTime Started { get; set; }
        DateTime? Completed { get; set; }
    }
}
