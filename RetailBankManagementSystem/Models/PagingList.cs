using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.Models
{
    public class PagingList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; set; }
        public int NumItems { get; set; }

        public PagingList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            NumItems = count;
            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public int TotalPageNo
        {
            get
            {
                return TotalPages;
            }
        }

        public static PagingList<T> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PagingList<T>(items, count, pageIndex, pageSize);
        }

        //overloaded for cases when the number of items in a page is controlled separately from the page size
        public static PagingList<T> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize, int effectivePageSize, int count)
        {
            var query = source.Skip((pageIndex - 1) * pageSize).Take(effectivePageSize).ToList();
            
            return new PagingList<T>(query, count, pageIndex, pageSize);
        }

        public static PagingList<T> CreateEmpty()
        {
            return new PagingList<T>(new List<T>(), 0, 0, 1);

        }


    }
}  
