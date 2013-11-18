using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using CAPCO.Infrastructure.Domain;

namespace CAPCO.Models
{
    public class HomePageViewModel
    {
        public HomePageViewModel()
        {
            Sliders = new List<SliderImage>();
        }

        public List<SliderImage> Sliders { get; set; }
        public ContentSection WhoWeAreSection { get; set; }
        public ContentSection WhatWeDoSection { get; set; }
        public ContentSection WelcomeSection { get; set; }
    }
}