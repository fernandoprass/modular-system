using Shared.Domain.DTOs.Responses;
using Shared.Domain.Entities;

namespace Shared.Domain.Mappers
{
   public static class ParameterMapper
   {
      public static ParameterDto ToParameterDto(this Parameter parameter)
      {
         return new ParameterDto
         {
            Id = parameter.Id,
            Module = parameter.Module,
            Group = parameter.Group,
            Name = parameter.Name,
            Key = parameter.Key,
            Title = parameter.Title,
            Description = parameter.Description,
            Type = parameter.Type,
            Value = parameter.Value,
            ListItems = parameter.ListItems,
            ExternalListEndpoint = parameter.ExternalListEndpoint,
            OverrideType = parameter.OverrideType,
            IsVisible = parameter.IsVisible
         };
      }

      public static ParameterLiteDto ToParameterLiteDto(this Parameter parameter)
      {
         return new ParameterLiteDto
         {
            Id = parameter.Id,
            Key = parameter.Key,
            Title = parameter.Title,
            Description = parameter.Description,
            Type = parameter.Type,
            Value = parameter.Value
         };
      }
   }
}
