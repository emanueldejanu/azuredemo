using System;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Demo2.Search
{
    [SerializePropertyNamesAsCamelCase]
    public class PhotoSearchItem
    {
        [System.ComponentModel.DataAnnotations.Key]
        [IsFilterable]
        public Guid Id { get; set; }
        
        [IsSearchable]
        //[Analyzer(AnalyzerName.AsString.FrLucene)]
        public string Description { get; set; }

        [IsFilterable, IsSortable, IsSearchable]
        public DateTime Date { get; set; }
        
        [IsSortable]
        public DateTime ModifiedAt { get; set; }
    }
}