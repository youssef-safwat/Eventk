﻿using Microsoft.EntityFrameworkCore;
using System;


namespace ServiceContracts.Helpers
{
    public class PaginatedList<T>
    {
        public PaginatedList()
        {
            
        }
        public PaginatedList(List<T> items, int pageNumber, int count, int pageSize)
        {
            Items = items;   
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize); 
            PageSize = pageSize;

        }
        public List<T> Items { get; private set; } 
        public int PageNumber { get; private set; } 
        public int TotalPages { get; private set; } 
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public int PageSize { get; private set; } 

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, pageNumber, count, pageSize);
        }
    }
}
