﻿using Ardalis.ApiEndpoints;
using BlazingTrails.Api.Persistance;
using BlazingTrails.Api.Persistance.Entities;
using BlazingTrails.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazingTrails.Api.Endpoints
{
    public class AddTrailEndpoint : BaseAsyncEndpoint<AddTrailRequest, bool>
    {
        private readonly BlazingTrailsContext _database;

        public AddTrailEndpoint(BlazingTrailsContext database)
        {
            _database = database;
        }

        [HttpPost(AddTrailRequest.RouteTemplate)]
        public override async Task<ActionResult<bool>> HandleAsync(AddTrailRequest request, CancellationToken cancellationToken = default)
        {
            // TODO: Check for image type?

            var trail = new Trail
            {
                Name = request.Name,
                Description = request.Description,
                Image = request.Image,
                Location = request.Location,
                TimeInMinutes = request.TimeInMinutes,
                Length = request.Length,
                IsFavourite = false
            };

            await _database.Trails.AddAsync(trail, cancellationToken);

            var routeInstructions = request.Route.Select(_ => new RouteInstruction
            {
                Stage = _.Stage,
                Description = _.Description,
                Trail = trail
            });

            await _database.RouteInstructions.AddRangeAsync(routeInstructions, cancellationToken);

            await _database.SaveChangesAsync(cancellationToken);

            return Ok(true);
        }
    }
}