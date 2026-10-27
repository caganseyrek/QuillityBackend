using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Quillity.API;

public class PrefixConvention : IApplicationModelConvention
{
    private string Prefix { get; set; }
    private string Version { get; set; }

    public PrefixConvention(IConfiguration configuration)
    {
        Prefix = configuration["API:Prefix"] ?? throw new Exception("API prefix value is invalid");
        Version = configuration["API:Version"] ?? throw new Exception("API version value is invalid");
    }

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            foreach (var selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel != null)
                {
                    selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                        new AttributeRouteModel(new RouteAttribute($"{Prefix}/{Version}")),
                        selector.AttributeRouteModel);
                }
            }
        }
    }
}
