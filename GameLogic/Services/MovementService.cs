using ObjectiveStrategy.GameModels.Instances;
using ObjectiveStrategy.GameModels.Map;
using System;
using System.Collections.Generic;

namespace ObjectiveStrategy.GameLogic.Services
{
    public class MovementService
    {
        public void Remove(Entity entity)
        {
            if (entity.Location != null)
            {
                entity.Location.Entity = null;
                entity.Location = null;
            }
        }

        public void Place(Entity entity, Cell location)
        {
            // TODO: check its empty first
            entity.Location = location;
            location.Entity = entity;
        }

        public void Move(Entity entity, Cell destination)
        {
            Remove(entity);
            Place(entity, destination);
        }
    }
}
