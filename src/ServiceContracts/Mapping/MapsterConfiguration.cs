using Entites;
using Mapster;
using ServiceContracts.DTOs;
using ServiceContracts.DTOs.Authentication.SignUp;
using ServiceContracts.DTOs.Event;
using ServiceContracts.DTOs.Organization;

namespace ServiceContracts.Mapping
{
    public static class MapsterConfiguration
    {
        public static TypeAdapterConfig GetConfiguredMapping()
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<RegisterRequestDto, ApplicationUser>()
                .Map(dest => dest.EmailConfirmed, src => true);
            // <==
            config.NewConfig<Event, EventSummaryResponse>()
                .Map(dest=> dest.Longitude , src=> src.Location.Y)
                .Map(dest => dest.Latitude, src => src.Location.X);
            config.NewConfig<Category, CategoryResponse>();
            config.NewConfig<Organization, OrganizationSummaryResponse>();
            config.NewConfig<TicketType, TicketTypeResponse>()
                .Map(dest => dest.AvailableTickets, src => src.TotalTickets);
            config.Compile();
            return config;
        }
    }
}
