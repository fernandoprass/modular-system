using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;

namespace IAM.Domain.Mappers
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
            IsCustomerEditable = parameter.IsCustomerEditable,
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
