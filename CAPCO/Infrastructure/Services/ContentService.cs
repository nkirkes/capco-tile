using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CAPCO.Infrastructure.Data;
using CAPCO.Infrastructure.Domain;

namespace CAPCO.Infrastructure.Services
{
    public enum ContentSectionNames
    {
        AboutUs,
        AboutTile,
        AboutStone,
        AboutSlab,
        AboutHandPainting,
        AboutEmployment,
        WhoWeAre,
        WhatWeDo,
        Welcome,
        Slabs,
        Footer,
        Locations,
        Contact,
        Links,
        Privacy,
        WhyRegister
    }

    public interface IContentService
    {
        ContentSection GetContentSection(string sectionName);
    }

    public class ContentService : IContentService
    {
        private readonly IContentSectionRepository _ContentSectionRepository;
        /// <summary>
        /// Initializes a new instance of the ContentService class.
        /// </summary>
        public ContentService(IContentSectionRepository contentSectionRepository)
        {
            _ContentSectionRepository = contentSectionRepository;            
        }

        public ContentSection GetContentSection(string sectionName)
        {
            return _ContentSectionRepository.All.FirstOrDefault(x => x.SectionName == sectionName);
        }
    }
}