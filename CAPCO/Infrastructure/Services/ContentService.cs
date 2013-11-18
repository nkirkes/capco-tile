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
        IEnumerable<ContentSection> GetContentSections(string[] sectionNames);
    }

    public class ContentService : IContentService
    {
        private readonly IRepository<ContentSection> _ContentSectionRepository;
        /// <summary>
        /// Initializes a new instance of the ContentService class.
        /// </summary>
        public ContentService(IRepository<ContentSection> contentSectionRepository)
        {
            _ContentSectionRepository = contentSectionRepository;    
        }

        public ContentSection GetContentSection(string sectionName)
        {
            return _ContentSectionRepository.All.FirstOrDefault(x => x.SectionName == sectionName);
        }

        public IEnumerable<ContentSection> GetContentSections(string[] sectionNames)
        {
            // TODO: test this for speed; could try to make it only select the sections we want, but the table is tiny.
            var query = _ContentSectionRepository.All.ToList();
            foreach(var name in sectionNames)
            {
                yield return query.FirstOrDefault(x => x.SectionName == name);
            }
        }
    }
}