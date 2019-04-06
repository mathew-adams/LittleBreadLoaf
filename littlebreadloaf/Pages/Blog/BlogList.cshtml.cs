using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using littlebreadloaf.Data;

namespace littlebreadloaf.Pages.Blog
{

    public class BlogList
    {
        public littlebreadloaf.Data.Blog Blog { get; set; }
        public List<Tag> Tags { get; set; }
        public BlogCategory Category { get; set; }
    }

    public class BlogListModel : PageModel
    {
        private List<littlebreadloaf.Data.Blog> Blogs { get; set; }
        private List<Tag> Tags { get; set; }
        private readonly ProductContext _context;

        public BlogListModel(ProductContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<BlogCategory> BlogCategories { get; set; }

        [BindProperty]
        public List<BlogList> BlogList { get; set; }

        [BindProperty]
        public List<Tag> AllTags { get; set; }

        [BindProperty]
        public List<DateTime?> AllDates { get; set; }

        [BindProperty(SupportsGet = true)]
        public string BlogSearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FilterTagID { get; set; }

        [BindProperty(SupportsGet = true)]
        public int FilterYear { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FilterMonth { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FilterCategoryID { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!string.IsNullOrEmpty(FilterTagID) && Guid.TryParse(FilterTagID, out Guid parsedTagID))
            {
                Blogs = await GetBlogsByFilteredTag(parsedTagID);
            }
            else if (!string.IsNullOrEmpty(BlogSearchTerm))
            {
                Blogs = await GetBlogsBySearchTerm();
            }
            else if(FilterYear > 0 && !string.IsNullOrEmpty(FilterMonth))
            {
                Blogs = await GetBlogsByYearAndMonth();
            }
            else if(!string.IsNullOrEmpty(FilterCategoryID) && Guid.TryParse(FilterCategoryID, out Guid parsedCategoryID))
            {
                Blogs = await GetBlogsFilteredByCategory(parsedCategoryID);
            }
            else
            {
                Blogs = await GetAllBlogs();
            }
            
            Blogs = Blogs.OrderByDescending(d => d.Created).ToList();

            var allBlogs = await GetAllBlogs();
            AllDates = allBlogs
                       .Select(s => s.Created)
                       .OrderByDescending(o => o.Value)
                       .ToList();

            var categoryList = await _context
                                    .BlogCategory
                                    .AsNoTracking()
                                    .Join(_context.CategoryToBlog,
                                          b => b.BlogCategoryID,
                                          ct => ct.BlogCategoryID, (b, ct) => new { b.BlogCategoryID, b.Name, ct.BlogID, ct.CategoryToBlogID })
                                    .Where(w => allBlogs.Any(a => a.BlogID == w.BlogID))
                                    .Select(s => new
                                    {
                                        s.Name,
                                        s.BlogCategoryID,
                                        s.BlogID
                                    })
                                    .ToListAsync();

            var listTags = await _context
                                .Tag
                                .AsNoTracking()
                                .Join(_context.SourceToTag,
                                      t => t.TagID,
                                      s => s.TagID,
                                      (t, s) => new { t.TagID, t.Name, s.SourceID, s.SourceArea })
                                .Where(w => w.SourceArea == "Blog")
                                .Select(s => new
                                {
                                    s.Name,
                                    s.SourceID,
                                    s.TagID
                                })
                                .ToListAsync();

            AllTags =  listTags
                        .Where(w => allBlogs.Any(a => a.BlogID == w.SourceID))
                        .GroupBy(g => g.Name)
                        .Select(x => x.First())
                        .Select(s => new Tag
                        {
                            Name = s.Name,
                            TagID = s.TagID
                        })
                        .OrderBy(o => o.Name)
                        .ToList();

            BlogCategories = categoryList
                            .GroupBy(g => g.Name)
                            .Select(x => x.First())
                            .Select(s => new BlogCategory
                            {
                                BlogCategoryID= s.BlogCategoryID,
                                Name = s.Name
                            })
                            .ToList();

            BlogList = new List<BlogList>();
            foreach (var blog in Blogs)
            {
                BlogList.Add(new BlogList()
                {
                    Blog = blog,
                    Tags = listTags
                           .Where(w => w.SourceID == blog.BlogID)
                           .Select(s => new Tag()
                           {
                               Name = s.Name,
                               TagID = s.TagID
                           })
                           .ToList(),
                    Category = categoryList
                               .Where(w => w.BlogID == blog.BlogID)
                               .Select(s => new BlogCategory
                               {
                                   BlogCategoryID = s.BlogCategoryID,
                                   Name = s.Name
                               })
                               .FirstOrDefault()
                });
            }
            return Page();
        }

        private async Task<List<littlebreadloaf.Data.Blog>> GetBlogsByFilteredTag(Guid filterTagID)
        {
            return FilterAuthenticated
                    (
                        await _context
                                .Blog
                                .AsNoTracking()
                                .Join(_context.SourceToTag,
                                        b => b.BlogID,
                                        st => st.SourceID,
                                        (b, st) => new { b.BlogID, b.Title, b.Description, b.Created, b.Published, st.SourceID, st.SourceArea, st.TagID })
                                .Where(w => w.SourceArea == "Blog" && w.TagID == filterTagID)
                                .Select(s => new littlebreadloaf.Data.Blog
                                {
                                    BlogID = s.BlogID,
                                    Title = s.Title,
                                    Description = s.Description,
                                    Created = s.Created,
                                    Published = s.Published,
                                    Content = "" //Clear content on list
                                })
                                .ToListAsync()
                   );
        }

        private async Task<List<littlebreadloaf.Data.Blog>> GetBlogsFilteredByCategory(Guid fliterCategoryID)
        {
            return FilterAuthenticated
                    (
                        await _context
                                .Blog
                                .AsNoTracking()
                                .Join(_context.CategoryToBlog,
                                        b => b.BlogID,
                                        ct => ct.BlogID,
                                        (b, ct) => new { b.BlogID, b.Title, b.Description, b.Created, b.Published, ct.BlogCategoryID})
                                .Where(w => w.BlogCategoryID == fliterCategoryID)
                                .Select(s => new littlebreadloaf.Data.Blog
                                {
                                    BlogID = s.BlogID,
                                    Title = s.Title,
                                    Description = s.Description,
                                    Created = s.Created,
                                    Published = s.Published,
                                    Content = "" //Clear content on list
                                })
                                .ToListAsync()
                   );
        }

        private async Task<List<littlebreadloaf.Data.Blog>> GetBlogsBySearchTerm()
        {
            BlogSearchTerm = BlogSearchTerm.Trim();
            return  FilterAuthenticated
                    (
                        await _context
                               .Blog
                               .AsNoTracking()
                               .Where(w => w.Title.Contains(BlogSearchTerm, StringComparison.InvariantCultureIgnoreCase)
                                           || w.Description.Contains(BlogSearchTerm, StringComparison.InvariantCultureIgnoreCase)
                                           || w.Content.Contains(BlogSearchTerm, StringComparison.InvariantCultureIgnoreCase))
                               .Select(s => new littlebreadloaf.Data.Blog
                               {
                                   BlogID = s.BlogID,
                                   Title = s.Title,
                                   Description = s.Description,
                                   Created = s.Created,
                                   Published = s.Published,
                                   Content = "" //Clear content on list
                                   })
                               .ToListAsync()
                   );
        }

        private async Task<List<littlebreadloaf.Data.Blog>> GetBlogsByYearAndMonth()
        {
            int month;
            switch(FilterMonth)
            {
                case "January":
                    month = 1;
                    break;
                case "February":
                    month = 2;
                    break;
                case "March":
                    month = 3;
                    break;
                case "April":
                    month = 4;
                    break;
                case "May":
                    month = 5;
                    break;
                case "June":
                    month = 6;
                    break;
                case "July":
                    month = 7;
                    break;
                case "August":
                    month = 8;
                    break;
                case "September":
                    month = 9;
                    break;
                case "October":
                    month = 10;
                    break;
                case "November":
                    month = 11;
                    break;
                case "December":
                    month = 12;
                    break;
                default:
                    month = 0;
                    break;
            }
            
            if(FilterYear > 0 && month > 0)
            {
                return FilterAuthenticated
                        (
                            await _context
                                    .Blog
                                    .AsNoTracking()
                                    .Where(w => w.Created.Value.Year == FilterYear && w.Created.Value.Month == month)
                                    .Select(s => new littlebreadloaf.Data.Blog
                                    {
                                        BlogID = s.BlogID,
                                        Title = s.Title,
                                        Description = s.Description,
                                        Created = s.Created,
                                        Published = s.Published,
                                        Content = "" //Clear content on list
                                    })
                                    .ToListAsync()
                         );
            }

            return FilterAuthenticated(await GetAllBlogs());
        }

        private async Task<List<littlebreadloaf.Data.Blog>> GetAllBlogs()
        {
            return  FilterAuthenticated
                    (
                       await  _context
                                .Blog
                                .AsNoTracking()
                                .Select(s => new littlebreadloaf.Data.Blog
                                {
                                    BlogID = s.BlogID,
                                    Title = s.Title,
                                    Description = s.Description,
                                    Created = s.Created,
                                    Published = s.Published,
                                    Content = "" //Clear content on list
                                })
                                .ToListAsync()
                    );
        }

        private List<littlebreadloaf.Data.Blog> FilterAuthenticated(List<littlebreadloaf.Data.Blog> blogs)
        {
            if (!User.Identity.IsAuthenticated) //TODO: Add administrator
            {
                return  blogs
                        .Where(w => w.Published < new DateTime(9999, 12, 31))
                        .Select(s => new littlebreadloaf.Data.Blog
                        {
                            BlogID = s.BlogID,
                            Title = s.Title,
                            Description = s.Description,
                            Created = s.Created,
                            Published = s.Published,
                            Content = "" //Clear content on list
                        })
                        .ToList();
            }
            else
            {
                return blogs;
            }
        }

    }
}