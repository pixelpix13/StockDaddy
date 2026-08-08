using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Helpers;

public static class UserStoreAssignmentHelper
{
    public static List<UserStoreAssignmentDto> Normalize(
        List<UserStoreAssignmentDto> assignments,
        List<int> storeIds,
        int roleId,
        int? defaultStoreId,
        int? storeId)
    {
        if (assignments.Count > 0)
        {
            return assignments;
        }

        var ids = storeIds.Where(id => id > 0).Distinct().ToList();
        if (ids.Count == 0 && storeId.HasValue)
        {
            ids = [storeId.Value];
        }

        if (ids.Count == 0)
        {
            return [];
        }

        var resolvedDefault = defaultStoreId.HasValue && ids.Contains(defaultStoreId.Value)
            ? defaultStoreId.Value
            : storeId.HasValue && ids.Contains(storeId.Value)
                ? storeId.Value
                : ids[0];

        return ids.Select(id => new UserStoreAssignmentDto
        {
            StoreId = id,
            RoleId = roleId,
            IsDefault = id == resolvedDefault
        }).ToList();
    }
}
