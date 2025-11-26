using wsahRecieveDelivary.Models;

namespace wsahRecieveDelivary.Extensions
{
    public static class QueryableExtensions
    {
        // ==========================================
        // FAST SEARCH - Searches across ALL fields
        // ==========================================
        public static IQueryable<WorkOrder> Search(
            this IQueryable<WorkOrder> query,
            string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var lowerSearchTerm = searchTerm.ToLower().Trim();

            return query.Where(w =>
                // Text fields
                w.WorkOrderNo.ToLower().Contains(lowerSearchTerm) ||
                w.Buyer.ToLower().Contains(lowerSearchTerm) ||
                w.StyleName.ToLower().Contains(lowerSearchTerm) ||
                w.Color.ToLower().Contains(lowerSearchTerm) ||
                w.WashType.ToLower().Contains(lowerSearchTerm) ||
                w.Factory.ToLower().Contains(lowerSearchTerm) ||
                w.Line.ToLower().Contains(lowerSearchTerm) ||
                w.Unit.ToLower().Contains(lowerSearchTerm) ||
                w.FastReactNo.ToLower().Contains(lowerSearchTerm) ||
                w.BuyerDepartment.ToLower().Contains(lowerSearchTerm) ||
                (w.Marks != null && w.Marks.ToLower().Contains(lowerSearchTerm)) ||

                // Number fields (convert to string for search)
                w.OrderQuantity.ToString().Contains(lowerSearchTerm) ||
                w.CutQty.ToString().Contains(lowerSearchTerm) ||
                w.TotalWashReceived.ToString().Contains(lowerSearchTerm) ||
                w.TotalWashDelivery.ToString().Contains(lowerSearchTerm) ||
                w.WashBalance.ToString().Contains(lowerSearchTerm)
            );
        }

        // ==========================================
        // ADVANCED FILTERS
        // ==========================================
        public static IQueryable<WorkOrder> ApplyFilters(
            this IQueryable<WorkOrder> query,
            string? factory,
            string? buyer,
            string? washType,
            string? line,
            string? unit,
            DateTime? fromDate,
            DateTime? toDate)
        {
            if (!string.IsNullOrWhiteSpace(factory))
                query = query.Where(w => w.Factory.ToLower() == factory.ToLower());

            if (!string.IsNullOrWhiteSpace(buyer))
                query = query.Where(w => w.Buyer.ToLower().Contains(buyer.ToLower()));

            if (!string.IsNullOrWhiteSpace(washType))
                query = query.Where(w => w.WashType.ToLower().Contains(washType.ToLower()));

            if (!string.IsNullOrWhiteSpace(line))
                query = query.Where(w => w.Line.ToLower().Contains(line.ToLower()));

            if (!string.IsNullOrWhiteSpace(unit))
                query = query.Where(w => w.Unit.ToLower().Contains(unit.ToLower()));

            if (fromDate.HasValue)
                query = query.Where(w => w.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(w => w.CreatedAt <= toDate.Value.AddDays(1).AddTicks(-1)); // End of day

            return query;
        }

        // ==========================================
        // DYNAMIC SORTING
        // ==========================================
        public static IQueryable<WorkOrder> ApplySort(
            this IQueryable<WorkOrder> query,
            string? sortBy,
            string sortOrder)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                sortBy = "CreatedAt";

            var isAscending = sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase);

            query = sortBy.ToLower() switch
            {
                "workorderno" => isAscending
                    ? query.OrderBy(w => w.WorkOrderNo)
                    : query.OrderByDescending(w => w.WorkOrderNo),

                "buyer" => isAscending
                    ? query.OrderBy(w => w.Buyer)
                    : query.OrderByDescending(w => w.Buyer),

                "stylename" => isAscending
                    ? query.OrderBy(w => w.StyleName)
                    : query.OrderByDescending(w => w.StyleName),

                "washtype" => isAscending
                    ? query.OrderBy(w => w.WashType)
                    : query.OrderByDescending(w => w.WashType),

                "washtargetdate" => isAscending
                    ? query.OrderBy(w => w.WashTargetDate)
                    : query.OrderByDescending(w => w.WashTargetDate),

                "orderquantity" => isAscending
                    ? query.OrderBy(w => w.OrderQuantity)
                    : query.OrderByDescending(w => w.OrderQuantity),

                "washbalance" => isAscending
                    ? query.OrderBy(w => w.WashBalance)
                    : query.OrderByDescending(w => w.WashBalance),

                "factory" => isAscending
                    ? query.OrderBy(w => w.Factory)
                    : query.OrderByDescending(w => w.Factory),

                "line" => isAscending
                    ? query.OrderBy(w => w.Line)
                    : query.OrderByDescending(w => w.Line),

                "updatedat" => isAscending
                    ? query.OrderBy(w => w.UpdatedAt)
                    : query.OrderByDescending(w => w.UpdatedAt),

                _ => isAscending
                    ? query.OrderBy(w => w.CreatedAt)
                    : query.OrderByDescending(w => w.CreatedAt)
            };

            return query;
        }
    }
}