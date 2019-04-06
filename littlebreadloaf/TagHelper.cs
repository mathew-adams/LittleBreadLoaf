using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using littlebreadloaf.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace littlebreadloaf
{
    public static class TagHelper
    {
        public static async Task<OkResult> AddTags(ProductContext context,
                                                   Guid? sourceID,
                                                   string sourceArea,
                                                   string sourceURL,
                                                   IEnumerable<string> tags)
        {
            List<Tag> allTags = context.Tag.ToList();

            List<string> transformedTags = new List<string>(tags);
            transformedTags = transformedTags.ConvertAll(s => s.Replace(" ", "").ToUpper());
                
            foreach (string newTag in transformedTags)
            {
                //Store all input tags
                if (!allTags.Any(w => w.Name.Equals(newTag)))
                {
                    Tag addTag = new Tag()
                    {
                        Name = newTag,
                        TagID = Guid.NewGuid()
                    };
                    context.Tag.Add(addTag);
                    allTags.Add(addTag);
                }
            }
                
            context.SourceToTag.RemoveRange(context.SourceToTag.Where(w => w.SourceID == sourceID.Value));

            if(transformedTags.Count > 0)
            {
                context.SourceToTag.AddRange(allTags
                                        .Where(w => transformedTags.Contains(w.Name))
                                        .Select(s => new SourceToTag()
                                        {
                                            SourceTagID = Guid.NewGuid(),
                                            TagID = s.TagID.Value,
                                            SourceArea = sourceArea,
                                            SourceID = sourceID.Value,
                                            SourceURL = sourceURL
                                        }));
            }
        
            return new OkResult(); 
        }

    }
}
