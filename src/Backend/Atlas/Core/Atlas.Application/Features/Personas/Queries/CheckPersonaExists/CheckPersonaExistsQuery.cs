using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Personas.Queries.CheckPersonaExists;

public record CheckPersonaExistsQuery() : IRequest<ResponseModel<bool>>;