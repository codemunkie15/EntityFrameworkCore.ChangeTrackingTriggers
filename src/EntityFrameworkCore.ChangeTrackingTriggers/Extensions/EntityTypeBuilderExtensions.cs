﻿using EntityFrameworkCore.ChangeTrackingTriggers.Abstractions;
using EntityFrameworkCore.ChangeTrackingTriggers.Configuration;
using EntityFrameworkCore.ChangeTrackingTriggers.Constants;
using EntityFrameworkCore.ChangeTrackingTriggers.Exceptions;
using EntityFrameworkCore.ChangeTrackingTriggers.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EntityFrameworkCore.ChangeTrackingTriggers.Extensions
{
    public static class EntityTypeBuilderExtensions
    {
        public static EntityTypeBuilder<TTrackedEntity> HasChangeTrackingTrigger<TTrackedEntity, TChangeEntity, TChangeId>(
            this EntityTypeBuilder<TTrackedEntity> builder,
            Action<ChangeTrackingTriggerOptions>? optionsBuilder = null)
            where TTrackedEntity : class, ITracked<TChangeEntity>
            where TChangeEntity: class, IChange<TTrackedEntity, TChangeId>
        {
            var options = ChangeTrackingTriggerOptions.Create(optionsBuilder);

            var trackedEntityType = builder.Metadata.Model.FindEntityType(typeof(TTrackedEntity))!;
            var changeEntityType = builder.Metadata.Model.FindEntityType(typeof(TChangeEntity))!;

            builder.HasAnnotation(AnnotationConstants.UseChangeTrackingTriggers, true);
            builder.HasAnnotation(AnnotationConstants.ChangeEntityTypeName, changeEntityType!.Name);

            // https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/breaking-changes#sqlserver-tables-with-triggers
            builder.ToTable(t => t.HasTrigger("ChangeTrackingTrigger"));

            // Configure relationship between TTracked and TChange using primary key as foreign key
            var trackedTablePrimaryKey = trackedEntityType.FindPrimaryKey();

            if (trackedTablePrimaryKey == null)
            {
                throw new ChangeTrackingTriggersConfigurationException(
                    $"Tracked entity type '{trackedEntityType.Name}' must have a primary key configured to use change tracking triggers.");
            }

            builder
                .HasMany(e => e.Changes)
                .WithOne(e => e.TrackedEntity)
                .HasForeignKey(trackedTablePrimaryKey.Properties.Select(p => p.Name).ToArray())
                .IsRequired(false) // FK column must be nullable to force left join as source record may have been
                .HasNoCheck(); // Must be NOCHECK so the FK reference can exist even if the source record is deleted

            return builder
                .ConfigureChangeTrackingTrigger(optionsBuilder);
        }

        public static EntityTypeBuilder<TTrackedEntity> ConfigureChangeTrackingTrigger<TTrackedEntity>(
            this EntityTypeBuilder<TTrackedEntity> builder,
            Action<ChangeTrackingTriggerOptions>? optionsBuilder = null)
            where TTrackedEntity : class
        {
            var options = ChangeTrackingTriggerOptions.Create(optionsBuilder);

            if (options.TriggerNameFactory != null)
            {
                var trackedEntityType = builder.Metadata.Model.FindEntityType(typeof(TTrackedEntity))!;
                var triggerNameFormat = TriggerNameFormatHelper.GetTriggerNameFormat(options.TriggerNameFactory, trackedEntityType);
                builder.HasAnnotation(AnnotationConstants.TriggerNameFormat, triggerNameFormat);
            }

            return builder;
        }

        public static EntityTypeBuilder<TChangeEntity> IsChangeTrackingTable<TTrackedEntity, TChangeEntity, TChangeId, TChangedBy, TChangeSource>(
            this EntityTypeBuilder<TChangeEntity> builder,
            Action<ChangeTrackingTableOptions<TChangeSource>>? optionsBuilder = null)
            where TTrackedEntity : class, ITracked<TChangeEntity>
            where TChangeEntity : class, IChange<TTrackedEntity, TChangeId>, IHasChangedBy<TChangedBy>, IHasChangeSource<TChangeSource>
        {
            return builder
                .IsChangeTrackingTable<TTrackedEntity, TChangeEntity, TChangeId>()
                .HasChangedBy<TTrackedEntity, TChangeEntity, TChangeId, TChangedBy>()
                .HasChangeSource<TTrackedEntity, TChangeEntity, TChangeId, TChangeSource>(optionsBuilder);
        }

        public static EntityTypeBuilder<TChangeEntity> IsChangeTrackingTable<TTrackedEntity, TChangeEntity, TChangeId>(
            this EntityTypeBuilder<TChangeEntity> builder)
            where TTrackedEntity : class, ITracked<TChangeEntity>
            where TChangeEntity : class, IChange<TTrackedEntity, TChangeId>
        {
            builder.HasKey(e => e.ChangeId);

            builder.Property(e => e.OperationType)
                .HasColumnName("OperationTypeId")
                .IsOperationTypeProperty();

            builder.Property(e => e.ChangedAt)
                .IsChangedAtProperty();

            return builder;
        }

        public static EntityTypeBuilder<TChangeEntity> IsChangeTrackingTable<TTrackedEntity, TChangeEntity, TChangeId, TChangedBy>(
            this EntityTypeBuilder<TChangeEntity> builder)
            where TTrackedEntity : class, ITracked<TChangeEntity>
            where TChangeEntity : class, IChange<TTrackedEntity, TChangeId>, IHasChangedBy<TChangedBy>
        {
            return builder
                .IsChangeTrackingTable<TTrackedEntity, TChangeEntity, TChangeId>()
                .HasChangedBy<TTrackedEntity, TChangeEntity, TChangeId, TChangedBy>();
        }

        public static EntityTypeBuilder<TChangeEntity> IsChangeTrackingTable<TTrackedEntity, TChangeEntity, TChangeId, TChangeSource>(
            this EntityTypeBuilder<TChangeEntity> builder,
            Action<ChangeTrackingTableOptions<TChangeSource>>? optionsBuilder = null)
            where TTrackedEntity : class, ITracked<TChangeEntity>
            where TChangeEntity : class, IChange<TTrackedEntity, TChangeId>, IHasChangeSource<TChangeSource>
        {
            return builder
                .IsChangeTrackingTable<TTrackedEntity, TChangeEntity, TChangeId>()
                .HasChangeSource<TTrackedEntity, TChangeEntity, TChangeId, TChangeSource>(optionsBuilder);
        }

        private static EntityTypeBuilder<TChangeEntity> HasChangedBy<TTrackedEntity, TChangeEntity, TChangeId, TChangedBy>(
            this EntityTypeBuilder<TChangeEntity> builder)
            where TTrackedEntity : class, ITracked<TChangeEntity>
            where TChangeEntity : class, IChange<TTrackedEntity, TChangeId>, IHasChangedBy<TChangedBy>
        {
            if (builder.Metadata.Model.FindEntityType(typeof(TChangedBy)) != null)
            {
                // Configure ChangedBy as a foreign key with navigation
                builder
                    .HasOne(typeof(TChangedBy), nameof(IHasChangedBy<TChangedBy>.ChangedBy)) // Can't use a navigation expression because TChangedBy isn't constrained to a class
                    .WithMany()
                    .IsChangedByForeignKey() // Used to find the column type for the trigger
                    .IsRequired(false) // FK column must be nullable to force left join as ChangedBy record may have been deleted
                    .HasNoCheck(); // Must be NOCHECK so the FK reference can exist even if the ChangedBy record has been deleted
            }
            else
            {
                // Configure ChangedBy as a scalar property
                builder.Property(e => e.ChangedBy)
                    .IsChangedByProperty();
            }

            return builder;
        }

        private static EntityTypeBuilder<TChangeEntity> HasChangeSource<TTrackedEntity, TChangeEntity, TChangeId, TChangeSource>(
            this EntityTypeBuilder<TChangeEntity> builder,
            Action<ChangeTrackingTableOptions<TChangeSource>>? optionsBuilder = null)
            where TTrackedEntity : class, ITracked<TChangeEntity>
            where TChangeEntity : class, IChange<TTrackedEntity, TChangeId>, IHasChangeSource<TChangeSource>
        {
            if (builder.Metadata.Model.FindEntityType(typeof(TChangeSource)) != null)
            {
                // Configure ChangeSource as a foreign key with navigation
                builder
                    .HasOne(typeof(TChangeSource), nameof(IHasChangeSource<TChangeSource>.ChangeSource)) // Can't use a navigation expression because TChangeSource isn't constrained to a class
                    .WithMany()
                    .IsChangeSourceForeignKey();
            }
            else
            {
                // Configure ChangedBy as a scalar property
                builder.Property(e => e.ChangeSource)
                .IsChangeSourceProperty();
            }

            return builder;
        }
    }
}