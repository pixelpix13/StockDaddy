using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Helpers;

namespace StockDaddy.Infrastructure.Persistence.Repositories
{
    public class ScheduledPriceRevertRepository : IScheduledPriceRevertRepository
    {
        private readonly ApplicationDbContext _context;

        public ScheduledPriceRevertRepository(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<PagedResult<ScheduledPriceRevert>> GetPagedAsync(PagedQuery query)
        {
            var q = RepositoryPaging.Normalize(query);
            var baseQuery = _context.ScheduledPriceReverts.AsQueryable();

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = $"%{q.Search}%";
            baseQuery = baseQuery.Where(x => EF.Functions.ILike(x.Type, pattern) || EF.Functions.ILike(x.BatchCriteria, pattern));
        }

            baseQuery = ApplySort(baseQuery, q);
            return await RepositoryPaging.ExecuteAsync(baseQuery, q);
        }

        private static IQueryable<ScheduledPriceRevert> ApplySort(IQueryable<ScheduledPriceRevert> query, PagedQuery q) =>
            (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
            {
            ("type", true) => query.OrderByDescending(x => x.Type),
            ("type", false) => query.OrderBy(x => x.Type),
            ("createdat", true) => query.OrderByDescending(x => x.CreatedAt),
            ("createdat", false) => query.OrderBy(x => x.CreatedAt),
            (_, true) => query.OrderByDescending(x => x.Id),
            _ => query.OrderBy(x => x.Id),
            };

        public async Task<List<ScheduledPriceRevert>> GetAllAsync()
            => await _context.ScheduledPriceReverts.ToListAsync();

        /// <summary>
        /// Returns all scheduled price reverts that are not completed and due for revert (RevertAt <= now)
        /// </summary>
        public async Task<List<ScheduledPriceRevert>> GetPendingRevertsAsync(DateTime now)
        {
            return await _context.ScheduledPriceReverts
                .Where(x => !x.IsCompleted && x.RevertAt.ToUniversalTime() <= now)
                .ToListAsync();
        }

        public async Task<ScheduledPriceRevert?> GetByIdAsync(int id)
            => await _context.ScheduledPriceReverts.FindAsync(id);

        public async Task<ScheduledPriceRevert> CreateAsync(ScheduledPriceRevert entity)
        {
            _context.ScheduledPriceReverts.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ScheduledPriceRevert?> UpdateAsync(ScheduledPriceRevert entity)
        {
            _context.ScheduledPriceReverts.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.ScheduledPriceReverts.FindAsync(id);
            if (entity == null) return false;
            _context.ScheduledPriceReverts.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
