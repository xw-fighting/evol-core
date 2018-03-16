﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Evol.Domain.Models
{
    public interface IAggregateRoot
    {
        Guid Id { get; set; }
    }

    public interface IAggregateRoot<TKey>
    {
        TKey Id { get; set; }
    }
}
