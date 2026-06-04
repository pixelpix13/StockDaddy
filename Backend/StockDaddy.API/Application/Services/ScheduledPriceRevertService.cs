

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;

namespace StockDaddy.Application.Services;

public class ScheduledPriceRevertService
{
    private readonly IScheduledPriceRevertRepository _repo;

    public ScheduledPriceRevertService(IScheduledPriceRevertRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ScheduledPriceRevert>> GetAllAsync() => await _repo.GetAllAsync();

    public async Task<ScheduledPriceRevert?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task<ScheduledPriceRevert> CreateAsync(CreateScheduledPriceRevertRequest request)
    {
        var entity = new ScheduledPriceRevert
        {
            Type = request.Type,
            RefId = request.RefId,
            OriginalPricesJson = request.OriginalPricesJson,
            RevertAt = request.RevertAt.ToUniversalTime(),
            CreatedBy = request.CreatedBy,
            BatchCriteria = request.BatchCriteria,
            CreatedAt = DateTime.UtcNow,
            IsCompleted = false
        };
        return await _repo.CreateAsync(entity);
    }

    public async Task<ScheduledPriceRevert?> UpdateAsync(int id, UpdateScheduledPriceRevertRequest request)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return null;
        if (request.RevertAt.HasValue) entity.RevertAt = request.RevertAt.Value;
        if (request.IsCompleted.HasValue) entity.IsCompleted = request.IsCompleted.Value;
        if (!string.IsNullOrEmpty(request.OriginalPricesJson)) entity.OriginalPricesJson = request.OriginalPricesJson;
        if (!string.IsNullOrEmpty(request.BatchCriteria)) entity.BatchCriteria = request.BatchCriteria;
        return await _repo.UpdateAsync(entity);
    }

    public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);

    /// <summary>
    /// Finds and returns all scheduled reverts that are due (RevertAt <= now, not completed)
    /// </summary>
    public async Task<List<ScheduledPriceRevert>> GetDueRevertsAsync(DateTime now)
    {
        var all = await _repo.GetAllAsync();
        return all.FindAll(x => !x.IsCompleted && x.RevertAt <= now);
    }

    /// <summary>
    /// Marks a scheduled revert as completed (after revert logic is applied)
    /// </summary>
    public async Task MarkAsCompletedAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity != null && !entity.IsCompleted)
        {
            entity.IsCompleted = true;
            await _repo.UpdateAsync(entity);
        }
    }
}
