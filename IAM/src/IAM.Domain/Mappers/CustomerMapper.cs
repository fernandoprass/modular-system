using IAM.Domain.DTOs.Responses;
using IAM.Domain.Entities;

namespace IAM.Domain.Mappers;

public static class CustomerMapper
{
   public static CustomerDto ToCustomerDto(this Customer customer)
   {
      return new CustomerDto
      {
         Id = customer.Id,
         Name = customer.Name,
         Description = customer.Description,
         CreatedAt = customer.CreatedAt,
         UpdatedAt = customer.UpdatedAt
      };
   }
}
