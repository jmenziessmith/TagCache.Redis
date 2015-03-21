using System;
using System.Collections.Generic;

namespace SampleMvcSite.Models
{
    [Serializable]
    public class SampleModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }

        public List<OtherModels> Other { get; set; } 


    }

    [Serializable]
    public class OtherModels
    {
        public int Foo { get; set; }
        public double Bar { get; set; }
    }
}