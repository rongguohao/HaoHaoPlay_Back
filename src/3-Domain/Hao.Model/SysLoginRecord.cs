﻿using Hao.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using Hao.Core;

namespace Hao.Model
{
    public class SysLoginRecord: EntityBase<long>
    {
        public long Id { get; set; }

        public long? UserId { get; set; }

        public string IP { get; set; }

        public DateTime? Time { get; set; }
    }
}
