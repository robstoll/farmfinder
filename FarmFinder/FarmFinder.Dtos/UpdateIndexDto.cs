﻿using System;

namespace CH.Tutteli.FarmFinder.Dtos
{
    public class UpdateIndexDto
    {
        public int FarmId { get; set; }

        public EUpdateMethod UpdateMethod { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
