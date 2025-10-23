﻿using System;

namespace PracticalModules.GameResourceSystem.Models
{
    [Serializable]
    public class ResourceData
    {
        public GameResourceType resourceType;
        public int amount;
    }
}