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
            Group = parameter.Group,
            Key = parameter.Key,
            Name = parameter.Name,
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
            Group = parameter.Group,
            Key = parameter.Key,
            Name = parameter.Name,
            Type = parameter.Type,
            Value = parameter.Value
         };
      }
   }
}
