namespace Dalapagos.Tunneling.Api.Mappers;

using Core.Model;
using Dto;

public class OrganizationsMapper : MapperBase<IList<Organization>, IList<OrganizationResponse>>
{
    public override IList<OrganizationResponse> Map(IList<Organization> source)
    {
        var responses = new List<OrganizationResponse>();
        foreach (var organization in source)
        {
            ArgumentNullException.ThrowIfNull(organization.Id, nameof(organization.Id));

            responses.Add(
                new OrganizationResponse
                {
                    OrganizationId = organization.Id.Value,
                    Name = organization.Name
                });
        }

        return responses;
    }
}
