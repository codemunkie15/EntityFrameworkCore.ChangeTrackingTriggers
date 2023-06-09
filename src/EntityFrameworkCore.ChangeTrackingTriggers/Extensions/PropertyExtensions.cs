﻿using EntityFrameworkCore.ChangeTrackingTriggers.Constants;
using EntityFrameworkCore.ChangeTrackingTriggers.Migrations.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.ChangeTrackingTriggers.Extensions
{
    internal static class PropertyExtensions
    {
        public static ChangeContextColumn AsChangeContextColumn(this IProperty property)
        {
            return new ChangeContextColumn(property.GetColumnName(), property.GetColumnType());
        }

        public static bool IsChangeContextProperty(this IProperty entityType)
        {
            return entityType.FindAnnotation(AnnotationConstants.IsChangeContextColumn) != null ||
                entityType.GetContainingForeignKeys().Any(fk => fk.FindAnnotation(AnnotationConstants.IsChangeContextColumn) != null);
        }

        public static bool IsChangedByProperty(this IProperty entityType)
        {
            return entityType.FindAnnotation(AnnotationConstants.IsChangedByColumn) != null ||
                entityType.GetContainingForeignKeys().Any(fk => fk.FindAnnotation(AnnotationConstants.IsChangedByColumn) != null);
        }

        public static bool IsOperationTypeProperty(this IProperty entityType)
        {
            return entityType.FindAnnotation(AnnotationConstants.IsOperationTypeColumn) != null;
        }

        public static bool IsChangeSourceProperty(this IProperty entityType)
        {
            return entityType.FindAnnotation(AnnotationConstants.IsChangeSourceColumn) != null ||
                entityType.GetContainingForeignKeys().Any(fk => fk.FindAnnotation(AnnotationConstants.IsChangeSourceColumn) != null);
        }

        public static bool IsChangedAtProperty(this IProperty entityType)
        {
            return entityType.FindAnnotation(AnnotationConstants.IsChangedAtColumn) != null;
        }
    }
}
