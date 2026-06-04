using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Infrastructure.Persistence.Repositories
{
    public class ScheduledPriceRevertRepository : IScheduledPriceRevertRepository
    {
        private readonly ApplicationDbContext _context;

        public ScheduledPriceRevertRepository(ApplicationDbContext context)
        {
            _context = context;
        }


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
