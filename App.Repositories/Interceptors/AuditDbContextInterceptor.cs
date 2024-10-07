using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Repositories.Interceptors
{
    internal class AuditDbContextInterceptor : SaveChangesInterceptor
    {
        private readonly static Dictionary<EntityState, Action<DbContext, IAuditEntity>> Behaviors = new()
        {
            {EntityState.Added, AddedBehavior },
            {EntityState.Modified, ModifiedBehavior },
        };
        private static void AddedBehavior(DbContext context, IAuditEntity auditEntity)
        {
            auditEntity.Created = DateTime.Now;
            context.Entry(auditEntity).Property(x => x.Updated).IsModified = false;
        }
        private static void ModifiedBehavior(DbContext context, IAuditEntity auditEntity)
        {
            context.Entry(auditEntity).Property(x => x.Created).IsModified = false;
            auditEntity.Updated = DateTime.Now;
            
        }
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result
            , CancellationToken cancellationToken = new CancellationToken())
        {


            foreach (var entityEntry in eventData.Context!.ChangeTracker.Entries().ToList())
            {

                if (entityEntry.Entity is not IAuditEntity auditEntity) continue;
                if(entityEntry.State is not(EntityState.Added or EntityState.Modified)) continue;
                Behaviors[entityEntry.State](eventData.Context, auditEntity);
                #region switch
                //switch (entityEntry.State)
                //{
                //    case EntityState.Added:




                //        AddedBehavior(eventData.Context, auditEntity);


                //        break;

                //    case EntityState.Modified:



                //        ModifiedBehavior(eventData.Context, auditEntity);


                //        break;
                //}
                #endregion

            }







            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
