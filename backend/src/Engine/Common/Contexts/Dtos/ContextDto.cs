using System;
using System.Collections.Generic;

namespace DailyLifeMate.Engine.Common.Contexts.Dtos;

public record ContextDto(
    Guid Id,
    string Name,
    string Description,
    bool IsArchived,
    List<DashboardItemSummaryDto> Items);

